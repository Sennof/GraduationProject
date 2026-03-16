using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIShopSideMenu : MonoBehaviour, Unity.VisualScripting.IInitializable
{
    [SerializeField] private MoneyBalance _moneyBalance;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _amountText;

    [SerializeField] private Button _deliveryRequestButton;
    [SerializeField] private TMP_Text _deliveryRequestButtonText;

    [SerializeField] private string _defaultButtonText;

    private EventBinding<DeliveryShopOnClickEvent> _onClickEventBinding;
    private EventBinding<OnDayStateChangeEvent> _onDayStateChangeBinding;

    private ProductData _productData = null;
    private int _productAmount = 1;

    private Coroutine _deliveryRequestCooldownCor = null;
    private bool _dayState = true;

    private void OnDisable()
    {
        EventBus<DeliveryShopOnClickEvent>.Deregister(_onClickEventBinding);
        EventBus<OnDayStateChangeEvent>.Deregister(_onDayStateChangeBinding);

        _onClickEventBinding = null;
    }

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
        //invoke event for spawning objects
        //data to transfer:
        //1. amount
        //2. productData

        if (_productData == null)
            return;

        Debug.Log($"DELIVERY REQUESTED | {this.name} by {gameObject.name}");

        if(_deliveryRequestCooldownCor != null)
        {
            StopCoroutine( _deliveryRequestCooldownCor );
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
        if (_productData == null) return; 

        _productAmount++;
        _amountText.text = _productAmount.ToString();
    }

    public void SubtractProductAmount()
    {
        if (_productData == null) return;

        if (_productAmount > 1) _productAmount--;
        _amountText.text = _productAmount.ToString();
    }

    private void ResetMenu()
    {
        _productData = null;
        _productAmount = 1;

        _titleText.text = "Выберите товар";
        _descriptionText.text = "";
        _amountText.text = "-";
        _deliveryRequestButton.interactable = false;
    }

    private void HandleProductClick(DeliveryShopOnClickEvent eventData) => SetText(eventData.ProductData); 

    private void SetText(ProductData data)
    {
        _productAmount = 1;
        _productData = data;

        _titleText.text = data.TitleName;
        _descriptionText.text = $"{data.Description}" +
            $"\n\nЦена за шт.:{data.Price}";
        _amountText.text = _productAmount.ToString();
        
        if(_deliveryRequestCooldownCor == null && _dayState)
        {
            _deliveryRequestButton.interactable = true;
        }
    }

    private IEnumerator DeliveryRequestCooldown(int time)
    {
        _deliveryRequestButton.interactable = false;
        _deliveryRequestButtonText.text = "<i>Ожидайте...</i>";

        yield return new WaitForSeconds(time);

        _deliveryRequestButtonText.text = _defaultButtonText;
        _deliveryRequestButton.interactable = true;
        _deliveryRequestCooldownCor = null;
    }

    private void HandleDayCycleChange(OnDayStateChangeEvent eventData)
    {
        if(eventData.isDay == true)
        {
            _deliveryRequestButton.interactable = true;
            _deliveryRequestButtonText.text = _defaultButtonText;
            _dayState = true;
            return;
        }
        
        if(_deliveryRequestCooldownCor != null)
        {
            StopCoroutine(_deliveryRequestCooldownCor);
            _deliveryRequestCooldownCor = null;
        }

        _dayState = false;
        _deliveryRequestButton.interactable = false;
        _deliveryRequestButtonText.text = "<i>Возвращайтесь завтра!</i>";
    }
}
