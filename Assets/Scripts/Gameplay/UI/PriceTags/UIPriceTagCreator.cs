using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPriceTagCreator : MonoBehaviour
{
    #region Fields

    [Header("UI References")]
    [Tooltip("Dropdown populated with available sellable products.")]
    [SerializeField] private TMP_Dropdown _productDropdown;
    [Tooltip("Slider controlling the markup percentage (0–200%).")]
    [SerializeField] private Slider _markupSlider;
    [Tooltip("Input field for the number of tags to create.")]
    [SerializeField] private TMP_InputField _quantityInput;
    [Tooltip("Button that confirms tag creation.")]
    [SerializeField] private Button _createButton;
    [Tooltip("Shows the calculated selling price for the selected product and markup.")]
    [SerializeField] private TextMeshProUGUI _pricePreviewText;
    [Tooltip("Shows remaining blank capacity and configured-but-unplaced tag count.")]
    [SerializeField] private TextMeshProUGUI _capacityText;
    [Tooltip("Shows current markup percentage value next to the slider.")]
    [SerializeField] private TextMeshProUGUI _markupValueText;

    [Header("Resources")]
    [Tooltip("Resource folder paths scanned for ProductData assets.")]
    [SerializeField] private string[] _productResourcePaths = { "Products" };

    [Header("Events")]
    [Tooltip("Invoked when the panel becomes visible (use to pause player, show cursor, etc.).")]
    [SerializeField] private UnityEvent _onShow;
    [Tooltip("Invoked when the panel is hidden.")]
    [SerializeField] private UnityEvent _onHide;

    private PriceTagMaker _targetMaker;
    private List<ProductData> _availableProducts = new();

    #endregion


    #region Public Methods

    public void Initialize(PriceTagMaker maker)
    {
        _targetMaker = maker;

        LoadProducts();
        PopulateDropdown();

        if (_createButton != null)
            _createButton.onClick.AddListener(OnCreateClicked);

        if (_markupSlider != null)
            _markupSlider.onValueChanged.AddListener(OnMarkupChanged);

        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        RefreshCapacityDisplay();
        RefreshPricePreview();
        _onShow?.Invoke();
    }

    public void Hide()
    {
        if (!gameObject.activeSelf) return;
        gameObject.SetActive(false);
        _onHide?.Invoke();
    }

    public void Toggle()
    {
        if (gameObject.activeSelf)
            Hide();
        else
            Show();
    }

    #endregion


    #region Private Methods

    private void LoadProducts()
    {
        _availableProducts.Clear();
        HashSet<ProductData> all = new HashSet<ProductData>();

        foreach (string path in _productResourcePaths)
        {
            ProductData[] loaded = Resources.LoadAll<ProductData>(path);
            if (loaded != null) all.UnionWith(loaded);
        }

        foreach (ProductData product in all)
        {
            if (product.IsSellable)
                _availableProducts.Add(product);
        }
    }

    private void PopulateDropdown()
    {
        if (_productDropdown == null) return;

        _productDropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (ProductData p in _availableProducts)
            options.Add(p.TitleName);

        _productDropdown.AddOptions(options);
        _productDropdown.onValueChanged.AddListener(_ => RefreshPricePreview());
    }

    private void OnCreateClicked()
    {
        if (_targetMaker == null || _availableProducts.Count == 0) return;

        if (!int.TryParse(_quantityInput != null ? _quantityInput.text : "1", out int quantity))
            quantity = 1;
        quantity = Mathf.Max(1, quantity);

        ProductData selected = _availableProducts[_productDropdown != null ? _productDropdown.value : 0];
        float markup = _markupSlider != null ? _markupSlider.value : 0f;

        EventBus<CreatePriceTagsRequestEvent>.Raise(new CreatePriceTagsRequestEvent
        {
            ProductData = selected,
            Markup = markup,
            Quantity = quantity,
            TargetMaker = _targetMaker
        });

        RefreshCapacityDisplay();
    }

    private void OnMarkupChanged(float value)
    {
        if (_markupValueText != null)
            _markupValueText.text = $"{Mathf.RoundToInt(value * 100)}%";
        RefreshPricePreview();
    }

    private void RefreshPricePreview()
    {
        if (_pricePreviewText == null || _availableProducts.Count == 0) return;

        int idx = _productDropdown != null ? _productDropdown.value : 0;
        if (idx >= _availableProducts.Count) return;

        float markup = _markupSlider != null ? _markupSlider.value : 0f;
        ProductData product = _availableProducts[idx];
        int price = Mathf.RoundToInt(product.Price * (1f + markup));
        _pricePreviewText.text = $"Price: {price} $";
    }

    private void RefreshCapacityDisplay()
    {
        if (_capacityText == null || _targetMaker == null) return;
        _capacityText.text =
            $"Stock: {_targetMaker.GetRemainingCapacity()} | Ready: {_targetMaker.GetConfiguredTagsCount()}";
    }

    #endregion
}
