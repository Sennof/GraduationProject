using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBuyingCard : MonoBehaviour
{
    #region Fields

    [Header("UI Elements")]
    [Tooltip("Text for product title.")]
    [SerializeField] private TMP_Text _titleText;
    [Tooltip("Text for amount.")]
    [SerializeField] private TMP_Text _amountText;
    [Tooltip("Image for product icon.")]
    [SerializeField] private Image _iconImage;

    private int _amount = 0;
    private ProductData _productData;
    private UIBuyingManager _manager;

    private EventBinding<PaymentResponseEvent> _paymentResponseBinding;

    #endregion


    #region Public Methods

    public void Initialize(ProductData data, UIBuyingManager manager)
    {
        _paymentResponseBinding = new EventBinding<PaymentResponseEvent>(ResetAmount);
        EventBus<PaymentResponseEvent>.Register(_paymentResponseBinding);

        _productData = data;
        _manager = manager;
        _titleText.text = data.TitleName;
        _iconImage.sprite = data.Icon;

        ResetAmount();
    }

    public void AddAmount()
    {
        _amount++;
        UpdateAmountText();
        EventBus<UIPaymentCardOperation>.Raise(new UIPaymentCardOperation { IsPlus = true, Price = _productData.Price });
    }

    public void ReduceAmount()
    {
        if (_amount <= 0)
        {
            return;
        }
        _amount--;
        UpdateAmountText();
        EventBus<UIPaymentCardOperation>.Raise(new UIPaymentCardOperation { IsPlus = false, Price = _productData.Price });
    }

    #endregion


    #region Private Methods

    private void UpdateAmountText() => _amountText.text = _amount.ToString();

    private void ResetAmount()
    {
        _amount = 0;
        UpdateAmountText();
        EventBus<UIPaymentCardOperation>.Raise(new UIPaymentCardOperation { IsPlus = false, Price = 1234 });
    }

    #endregion


    #region Unity Methods

    private void OnDisable()
    {
        ResetAmount();
        EventBus<PaymentResponseEvent>.Deregister(_paymentResponseBinding);
    }

    #endregion
}