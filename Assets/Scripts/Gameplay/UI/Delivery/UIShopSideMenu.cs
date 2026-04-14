using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIShopSideMenu : MonoBehaviour, Unity.VisualScripting.IInitializable
{
    #region Fields

    [Header("Services")]
    [Tooltip("Money balance service.")]
    [SerializeField] private MoneyBalance _moneyBalance;

    [Header("UI Elements")]
    [Tooltip("Text for product title.")]
    [SerializeField] private TMP_Text _titleText;
    [Tooltip("Text for product description.")]
    [SerializeField] private TMP_Text _descriptionText;
    [Tooltip("Text for amount.")]
    [SerializeField] private TMP_Text _amountText;

    [Header("Delivery Button")]
    [Tooltip("Button to request delivery.")]
    [SerializeField] private Button _deliveryRequestButton;
    [Tooltip("Text on delivery button.")]
    [SerializeField] private TMP_Text _deliveryRequestButtonText;

    [Header("Settings")]
    [Tooltip("Default button text.")]
    [SerializeField] private string _defaultButtonText;

    private EventBinding<DeliveryShopOnClickEvent> _onClickEventBinding;
    private EventBinding<OnDayStateChangeEvent> _onDayStateChangeBinding;

    private ProductData _productData = null;
    private int _productAmount = 1;

    private Coroutine _deliveryRequestCooldownCor = null;
    private bool _dayState = true;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        _onClickEventBinding = new EventBinding<DeliveryShopOnClickEvent>(HandleProductClick);
        EventBus<DeliveryShopOnClickEvent>.Register(_onClickEventBinding);

        _onDayStateChangeBinding = new EventBinding<OnDayStateChangeEvent>(HandleDayCycleChange);
        EventBus<OnDayStateChangeEvent>.Register(_onDayStateChangeBinding);

        ResetMenu();
    }

    public void RequestDelivery()
    {
        if (_productData == null)
        {
            return;
        }

        Debug.Log($"DELIVERY REQUESTED | {this.name} by {gameObject.name}");

        if (_deliveryRequestCooldownCor != null)
        {
            StopCoroutine(_deliveryRequestCooldownCor);
            _deliveryRequestCooldownCor = null;
        }

        if (_moneyBalance.GetPriceAvailability(_productData.Price * _productAmount) == false)
        {
            Debug.Log("Not enough money for delivery | UIShopSideMenu");
            return;
        }

        _deliveryRequestCooldownCor = StartCoroutine(DeliveryRequestCooldown(_productAmount + 1));
        EventBus<DeliveryRequestingEvent>.Raise(new DeliveryRequestingEvent { Amount = _productAmount, ProductData = _productData });

        ResetMenu();
    }

    public void AddProductAmount()
    {
        if (_productData == null)
        {
            return;
        }

        _productAmount++;
        _amountText.text = _productAmount.ToString();
    }

    public void SubtractProductAmount()
    {
        if (_productData == null)
        {
            return;
        }

        if (_productAmount > 1)
        {
            _productAmount--;
        }
        _amountText.text = _productAmount.ToString();
    }

    #endregion


    #region Private Methods

    private void ResetMenu()
    {
        _productData = null;
        _productAmount = 1;

        _titleText.text = "Select product";
        _descriptionText.text = "";
        _amountText.text = "-";
        _deliveryRequestButton.interactable = false;
    }

    private void SetText(ProductData data)
    {
        _productAmount = 1;
        _productData = data;

        _titleText.text = data.TitleName;
        _descriptionText.text = $"{data.Description}\n\nPrice per item: {data.Price}";
        _amountText.text = _productAmount.ToString();

        if (_deliveryRequestCooldownCor == null && _dayState)
        {
            _deliveryRequestButton.interactable = true;
        }
    }

    #endregion


    #region Event Handlers

    private void HandleProductClick(DeliveryShopOnClickEvent eventData) => SetText(eventData.ProductData);

    private void HandleDayCycleChange(OnDayStateChangeEvent eventData)
    {
        if (eventData.IsDay == true)
        {
            _deliveryRequestButton.interactable = true;
            _deliveryRequestButtonText.text = _defaultButtonText;
            _dayState = true;
            return;
        }

        if (_deliveryRequestCooldownCor != null)
        {
            StopCoroutine(_deliveryRequestCooldownCor);
            _deliveryRequestCooldownCor = null;
        }

        _dayState = false;
        _deliveryRequestButton.interactable = false;
        _deliveryRequestButtonText.text = "<i>Come back tomorrow!</i>";
    }

    #endregion


    #region Coroutines

    private IEnumerator DeliveryRequestCooldown(int time)
    {
        _deliveryRequestButton.interactable = false;
        _deliveryRequestButtonText.text = "<i>Waiting...</i>";

        yield return new WaitForSeconds(time);

        _deliveryRequestButtonText.text = _defaultButtonText;
        _deliveryRequestButton.interactable = true;
        _deliveryRequestCooldownCor = null;
    }

    #endregion


    #region Unity Methods

    private void OnDisable()
    {
        EventBus<DeliveryShopOnClickEvent>.Deregister(_onClickEventBinding);
        EventBus<OnDayStateChangeEvent>.Deregister(_onDayStateChangeBinding);

        _onClickEventBinding = null;
    }

    #endregion
}