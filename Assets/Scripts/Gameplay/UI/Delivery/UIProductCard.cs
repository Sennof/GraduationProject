using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIProductCard : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private Image _iconImage;

    private ProductData _productData;

    public void Initialize(ProductData data)
    {
        _productData = data;

        _titleText.text = data.TitleName;
        _iconImage.sprite = data.Icon;
    }

    public void OnClick()
    {
        //Invoke an event(create it) for showing side menu information in shop
        EventBus<DeliveryShopOnClickEvent>.Raise(new DeliveryShopOnClickEvent {ProductData = _productData});
    }
}
