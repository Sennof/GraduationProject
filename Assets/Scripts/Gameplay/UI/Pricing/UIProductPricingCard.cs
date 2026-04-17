using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIProductPricingCard : MonoBehaviour
{
    #region Fields

    [Header("UI Elements")]
    [Tooltip("Text displaying product name.")]
    [SerializeField] private TMP_Text _nameText;
    [Tooltip("Slider for adjusting markup.")]
    [SerializeField] private Slider _markupSlider;
    [Tooltip("Text showing current markup percentage.")]
    [SerializeField] private TMP_Text _percentageText;
    [Tooltip("Text showing base price.")]
    [SerializeField] private TMP_Text _basePriceText;
    [Tooltip("Text showing final price with markup.")]
    [SerializeField] private TMP_Text _finalPriceText;

    private ProductData _productData;

    #endregion


    #region Public Methods

    public void Initialize(ProductData product)
    {
        _productData = product;
        _nameText.text = product.TitleName;
        _basePriceText.text = $"Base: ${product.Price}";

        float currentMarkup = GlobalStatsBridge.Instance.GetProductMarkup(product.TitleName);
        _markupSlider.value = currentMarkup;
        UpdateDisplay(currentMarkup);

        _markupSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    /// <summary>
    /// Enables or disables the markup slider.
    /// </summary>
    public void SetSliderInteractable(bool interactable)
    {
        if (_markupSlider != null)
            _markupSlider.interactable = interactable;
    }

    #endregion


    #region Private Methods

    private void OnSliderChanged(float value)
    {
        GlobalStatsBridge.Instance.SetProductMarkup(_productData.TitleName, value);
        UpdateDisplay(value);
    }

    private void UpdateDisplay(float markup)
    {
        int percent = Mathf.RoundToInt(markup * 100f);
        _percentageText.text = $"+{percent}%";

        int finalPrice = Mathf.RoundToInt(_productData.Price * (1f + markup));
        _finalPriceText.text = $"${finalPrice}";
    }

    #endregion


    #region Unity Methods

    private void OnDestroy()
    {
        if (_markupSlider != null)
            _markupSlider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    #endregion
}