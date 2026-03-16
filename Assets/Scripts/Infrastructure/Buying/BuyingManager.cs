using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BuyingManager : MonoBehaviour, IInitializeable
{
    #region Fields
    [Header("Components")]
    [SerializeField] private Interactable _interactableStateSwitcher;
    [SerializeField] private UIBuyingManager _ui;
    [SerializeField] private ProductGenerator _productGenerator;
    [Inject] private IMoneyBalance _moneyBalance;


    [Header("Settings")]
    [SerializeField] private List<ProductData> _buyableProductDatas = new();

    [Header("Runtime State")]
    private List<ProductData> _productsData = new();
    private int _currentTotalPrice = 0;
    private int _currentRealTotalPrice = 0;

    private EventBinding<PaymentRequestEvent> _paymentRequestBinding;
    private EventBinding<UIPaymentCardOperation> _paymentOperationBinding;
    #endregion

    #region Initialization
    public void Initialize()
    {
        _paymentRequestBinding = new EventBinding<PaymentRequestEvent>(HandlePaymentRequest);
        EventBus<PaymentRequestEvent>.Register(_paymentRequestBinding);

        _paymentOperationBinding = new EventBinding<UIPaymentCardOperation>(HandlePaymentOperation);
        EventBus<UIPaymentCardOperation>.Register(_paymentOperationBinding);

        _interactableStateSwitcher.enabled = false;
        _ui.Initialize(_buyableProductDatas.ToArray());
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
        int price = (int)(eventData.Price * GlobalStatsBridge.Instance.GetPricingMod());
        _currentTotalPrice += eventData.isPlus ? price : -price;
        _ui.SetPriceText(_currentTotalPrice);
    }
    #endregion

    #region Internal Logic
    public void TryBuy()
    {
        _currentRealTotalPrice = _productGenerator.GetRealTotalPrice();

        int difference = _currentTotalPrice - _currentRealTotalPrice;

        _productGenerator.DestroyAllGenerated();
        if (difference == 0)
        {
            _moneyBalance.AddMoney(_currentTotalPrice, "Продажа товара");
        }
        else if(difference < 0)
        {
            _moneyBalance.AddMoney(_currentTotalPrice, "Продажа товара(Ошибка чека)");
        }
        else
        {
            _moneyBalance.RemoveMoney(_currentRealTotalPrice, "Продажа товара(Попытка воровства)");
        }

            EventBus<PaymentResponseEvent>.Raise(new PaymentResponseEvent { });

        _interactableStateSwitcher.SetActiveState(false);
    }

    

    private void OnDisable()
    {
        EventBus<PaymentRequestEvent>.Deregister(_paymentRequestBinding);
        EventBus<UIPaymentCardOperation>.Deregister(_paymentOperationBinding);
    }
    #endregion
}