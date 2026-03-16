using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBuyingCard : MonoBehaviour
{
    #region Fields
    [Header("UI Elements")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _amountText;
    [SerializeField] private Image _iconImage;

    private int _amount = 0;
    private ProductData _productData;
    private UIBuyingManager _manager;

    private EventBinding<PaymentResponseEvent> _binding;
    #endregion

    #region Logic
    public void Initialize(ProductData data, UIBuyingManager manager)
    {
        _binding = new EventBinding<PaymentResponseEvent>(ResetAmount);
        EventBus<PaymentResponseEvent>.Register(_binding);

        _productData = data;
        _manager = manager;
        _titleText.text = data.TitleName;
        _iconImage.sprite = data.Icon;

        ResetAmount();
    }

    private void OnDisable()
    {
        ResetAmount();
        EventBus<PaymentResponseEvent>.Deregister(_binding);
    }

    public void AddAmount()
    {
        _amount++;
        UpdateAmountText();
        EventBus<UIPaymentCardOperation>.Raise(new UIPaymentCardOperation { isPlus = true, Price = _productData.Price });
    }

    public void ReduceAmount()
    {
        if (_amount <= 0) return;
        _amount--;
        UpdateAmountText();
        EventBus<UIPaymentCardOperation>.Raise(new UIPaymentCardOperation { isPlus = false, Price = _productData.Price });
    }

    private void UpdateAmountText() => _amountText.text = _amount.ToString();

    private void ResetAmount()
    {
        _amount = 0;
        UpdateAmountText();
        EventBus<UIPaymentCardOperation>.Raise(new UIPaymentCardOperation { isPlus = false, Price = 1234 });
    }
    #endregion
}