using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BuyingManager : MonoBehaviour, IInitializeable
{
    #region Fields

    [Header("Components")]
    [Tooltip("Interactable that toggles payment UI.")]
    [SerializeField] private Interactable _interactableStateSwitcher;
    [Tooltip("UI manager for the buying process.")]
    [SerializeField] private UIBuyingManager _ui;
    [Tooltip("Generator for spawning products at checkout.")]
    [SerializeField] private ProductGenerator _productGenerator;

    [Inject] private IMoneyBalance _moneyBalance;
    [Inject] private IRatingManager _ratingManager;

    [Header("Settings")]
    [Tooltip("List of products that can be sold.")]
    [SerializeField] private List<ProductData> _buyableProductDatas = new();

    [Header("Runtime State")]
    private List<ProductData> _productsData = new();
    private int _currentTotalPrice = 0;
    private int _currentRealTotalPrice = 0;

    private EventBinding<PaymentRequestEvent> _paymentRequestBinding;
    private EventBinding<UIPaymentCardOperation> _paymentOperationBinding;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        _paymentRequestBinding = new EventBinding<PaymentRequestEvent>(HandlePaymentRequest);
        EventBus<PaymentRequestEvent>.Register(_paymentRequestBinding);

        _paymentOperationBinding = new EventBinding<UIPaymentCardOperation>(HandlePaymentOperation);
        EventBus<UIPaymentCardOperation>.Register(_paymentOperationBinding);

        _interactableStateSwitcher.SetActiveState(false);
        _ui.Initialize(_buyableProductDatas.ToArray());
    }

    public void TryBuy()
    {
        _currentRealTotalPrice = _productGenerator.GetRealTotalPrice();

        int difference = _currentTotalPrice - _currentRealTotalPrice;

        _productGenerator.DestroyAllGenerated();
        if (difference == 0)
        {
            _moneyBalance.AddMoney(_currentTotalPrice, "Sale");
            _ratingManager.AddRating(0.1f);
            _ratingManager.AddFeedback("All good, I liked it.");
        }
        else if (difference < 0)
        {
            _moneyBalance.AddMoney(_currentTotalPrice, "Sale (Receipt error)");
            _ratingManager.AddRating(0.025f);
            _ratingManager.AddFeedback("Cashier is a nice guy, miscalculated the receipt.");
        }
        else
        {
            _moneyBalance.RemoveMoney(_currentRealTotalPrice, "Sale (Theft attempt)");
            _ratingManager.ReduceRating(0.12f);
            _ratingManager.AddFeedback("I was robbed!");
        }

        EventBus<PaymentResponseEvent>.Raise(new PaymentResponseEvent { });

        _interactableStateSwitcher.SetActiveState(false);
    }

    #endregion


    #region Event Handlers

    private void HandlePaymentRequest(PaymentRequestEvent eventData)
    {
        _interactableStateSwitcher.SetActiveState(true);
        _currentTotalPrice = 0;
        _ui.SetPriceText(0);

        _productsData.Clear();
        foreach (GameObject obj in eventData.Products)
        {
            if (obj.TryGetComponent(out ItemObject item))
            {
                _productsData.Add(item.GetProductData());
            }
        }

        _productGenerator.SpawnBuyingProducts(eventData.Products);
    }

    private void HandlePaymentOperation(UIPaymentCardOperation eventData)
    {
        if (eventData.IsPlus == false && eventData.Price == 1234)
        {
            _ui.SetPriceText(0);
            _currentTotalPrice = 0;
            return;
        }

        int price = (int)(eventData.Price * GlobalStatsBridge.Instance.GetPricingMod());
        _currentTotalPrice += eventData.IsPlus ? price : -price;
        _ui.SetPriceText(_currentTotalPrice);
    }

    #endregion


    #region Unity Methods

    private void OnDisable()
    {
        EventBus<PaymentRequestEvent>.Deregister(_paymentRequestBinding);
        EventBus<UIPaymentCardOperation>.Deregister(_paymentOperationBinding);
    }

    #endregion
}