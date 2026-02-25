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

    private EventBinding<DeliveryShopOnClickEvent> _eventBinding;

    private ProductData _productData = null;
    private int _productAmount = 1;

    private Coroutine _deliveryRequestCooldownCor = null;

    private void OnDisable()
    {
        EventBus<DeliveryShopOnClickEvent>.Deregister(_eventBinding);
        _eventBinding = null;
    }

    public void Initialize()
    {
        if (_eventBinding != null)
        {
            EventBus<DeliveryShopOnClickEvent>.Deregister(_eventBinding);
            _eventBinding = null;
        }

        _titleText.text = "Выберите товар";
        _descriptionText.text = "";
        _amountText.text = "-";

        _eventBinding = new EventBinding<DeliveryShopOnClickEvent>(HandleProductClick);
        EventBus<DeliveryShopOnClickEvent>.Register(_eventBinding);
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

    private void HandleProductClick(DeliveryShopOnClickEvent eventData) => SetText(eventData.ProductData); 

    private void SetText(ProductData data)
    {
        _productAmount = 1;
        _productData = data;

        _titleText.text = data.TitleName;
        _descriptionText.text = $"{data.Description}" +
            $"\n\nЦена за шт.:{data.Price}";
        _amountText.text = _productAmount.ToString();
    }

    private IEnumerator DeliveryRequestCooldown(int time)
    {
        string defaultText = _deliveryRequestButtonText.text;

        _deliveryRequestButton.interactable = false;
        _deliveryRequestButtonText.text = "<i>Ожидайте...</i>";

        yield return new WaitForSeconds(time);

        _deliveryRequestButtonText.text = defaultText;
        _deliveryRequestButton.interactable = true;
    }
}
