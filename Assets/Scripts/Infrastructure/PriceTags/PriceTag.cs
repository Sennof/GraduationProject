using TMPro;
using UnityEngine;

public class PriceTag : MonoBehaviour
{
    #region Fields

    [Header("Display")]
    [Tooltip("Label showing the product name.")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [Tooltip("Label showing the selling price.")]
    [SerializeField] private TextMeshProUGUI _priceText;

    [Header("State")]
    [Tooltip("Product this tag is configured for.")]
    [SerializeField] private ProductData _targetProduct;
    [Tooltip("Markup override for this shelf slot (0 = base price, 1.0 = +100%).")]
    [SerializeField][Range(0f, 2f)] private float _markup;

    private Shelf _attachedShelf;

    #endregion


    #region Public Methods

    public void Configure(ProductData product, float markup)
    {
        _targetProduct = product;
        _markup = Mathf.Clamp(markup, 0f, 2f);
        RefreshDisplay();
    }

    public void AttachToShelf(Shelf shelf, Transform attachPoint)
    {
        if (_attachedShelf != null)
            Detach();

        _attachedShelf = shelf;
        transform.SetParent(attachPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        shelf.RegisterPriceTag(this);
        gameObject.SetActive(true);
    }

    public void Detach()
    {
        if (_attachedShelf != null)
        {
            _attachedShelf.UnregisterPriceTag(this);
            _attachedShelf = null;
        }

        transform.SetParent(null);
        gameObject.SetActive(false);
    }

    public float GetMarkup() => _markup;

    public ProductData GetTargetProduct() => _targetProduct;

    public int GetEffectivePrice()
    {
        if (_targetProduct == null) return 0;
        return Mathf.RoundToInt(_targetProduct.Price * (1f + _markup));
    }

    public Shelf GetAttachedShelf() => _attachedShelf;

    #endregion


    #region Private Methods

    private void RefreshDisplay()
    {
        if (_titleText != null && _targetProduct != null)
            _titleText.text = _targetProduct.TitleName;

        if (_priceText != null && _targetProduct != null)
            _priceText.text = $"{GetEffectivePrice()} $";
    }

    #endregion
}
