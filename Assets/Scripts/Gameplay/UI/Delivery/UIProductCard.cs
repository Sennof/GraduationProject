using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIProductCard : MonoBehaviour
{
    #region Fields

    [Header("UI Elements")]
    [Tooltip("Text for product title.")]
    [SerializeField] private TMP_Text _titleText;
    [Tooltip("Image for product icon.")]
    [SerializeField] private Image _iconImage;

    private ProductData _productData;

    #endregion


    #region Public Methods

    public void Initialize(ProductData data)
    {
        _productData = data;

        _titleText.text = data.TitleName;
        _iconImage.sprite = data.Icon;
    }

    public void OnClick()
    {
        EventBus<DeliveryShopOnClickEvent>.Raise(new DeliveryShopOnClickEvent { ProductData = _productData });
    }

    #endregion
}