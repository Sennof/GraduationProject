using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIProductPricingManager : MonoBehaviour
{
    #region Fields

    [Header("References")]
    [Tooltip("Container where product cards will be instantiated.")]
    [SerializeField] private Transform _contentContainer;
    [Tooltip("Prefab for a single product pricing card.")]
    [SerializeField] private GameObject _cardPrefab;
    [Tooltip("The title where the category is displayed")]
    [SerializeField] private TMP_Text _categoryTitleText;

    [Header("Categories")]
    [Tooltip("Display names for each category (must match order of resource paths).")]
    [SerializeField] private string[] _categoryNames = { "All" };
    [Tooltip("Resource paths corresponding to each category (same order as names).")]
    [SerializeField] private string[] _resourcePaths = { "Products" };

    [Header("State")]
    [Tooltip("Index of currently active category. Use -1 for 'All'.")]
    [SerializeField] private int _currentCategoryIndex = -1;

    private Dictionary<string, List<ProductData>> _categoryProducts = new();
    private List<UIProductPricingCard> _activeCards = new();

    private EventBinding<OnShopStateChanging> _shopStateBinding;

    #endregion


    #region Unity Methods

    private void Start()
    {
        ValidateSetup();
        LoadAllCategories();
        ShowCategory(_currentCategoryIndex);

        _shopStateBinding = new EventBinding<OnShopStateChanging>(OnShopStateChanged);
        EventBus<OnShopStateChanging>.Register(_shopStateBinding);
    }

    private void OnDestroy()
    {
        EventBus<OnShopStateChanging>.Deregister(_shopStateBinding);
    }

    #endregion


    #region Public Methods

    public void ShowCategory(int categoryIndex)
    {
        if (categoryIndex < -1 || categoryIndex >= _resourcePaths.Length)
        {
            Debug.LogWarning($"Category index {categoryIndex} out of range.");
            return;
        }

        _currentCategoryIndex = categoryIndex;
        ClearContent();
        GenerateUIForCategory(categoryIndex);

        if (categoryIndex == -1)
            _categoryTitleText.text = "Все";
        else
            _categoryTitleText.text = _categoryNames[categoryIndex];

        UpdateSlidersInteractable();
    }

    public void ShowCategory(string categoryName)
    {
        int index = System.Array.IndexOf(_categoryNames, categoryName);
        if (index == -1)
        {
            Debug.LogWarning($"Category '{categoryName}' not found.");
            return;
        }
        ShowCategory(index);
    }

    #endregion


    #region Private Methods

    private void ValidateSetup()
    {
        if (_categoryNames.Length != _resourcePaths.Length)
        {
            Debug.LogError("Category names and resource paths arrays must have the same length.");
        }
    }

    private void LoadAllCategories()
    {
        _categoryProducts.Clear();

        for (int i = 0; i < _resourcePaths.Length; i++)
        {
            string path = _resourcePaths[i];
            ProductData[] loaded = Resources.LoadAll<ProductData>(path);
            if (loaded != null && loaded.Length > 0)
            {
                List<ProductData> products = new List<ProductData>(loaded);
                products.Sort((a, b) => string.Compare(a.TitleName, b.TitleName));
                _categoryProducts[_categoryNames[i]] = products;
            }
            else
            {
                Debug.LogWarning($"No ProductData found in Resources/{path}");
                _categoryProducts[_categoryNames[i]] = new List<ProductData>();
            }
        }

        HashSet<ProductData> allUnique = new HashSet<ProductData>();
        foreach (var list in _categoryProducts.Values)
            allUnique.UnionWith(list);
        List<ProductData> allProducts = new List<ProductData>(allUnique);
        allProducts.Sort((a, b) => string.Compare(a.TitleName, b.TitleName));
        _categoryProducts["All"] = allProducts;
    }

    private void GenerateUIForCategory(int categoryIndex)
    {
        string categoryKey = (categoryIndex == -1) ? "All" : _categoryNames[categoryIndex];
        if (!_categoryProducts.ContainsKey(categoryKey))
        {
            Debug.LogError($"Category '{categoryKey}' not found in loaded data.");
            return;
        }

        foreach (ProductData product in _categoryProducts[categoryKey])
        {
            GameObject cardObj = Instantiate(_cardPrefab, _contentContainer);
            UIProductPricingCard card = cardObj.GetComponent<UIProductPricingCard>();
            if (card != null)
            {
                card.Initialize(product);
                _activeCards.Add(card);
            }
        }
    }

    private void ClearContent()
    {
        foreach (UIProductPricingCard card in _activeCards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }
        _activeCards.Clear();

        foreach (Transform child in _contentContainer)
            Destroy(child.gameObject);
    }

    private void UpdateSlidersInteractable()
    {
        bool shopOpen = GlobalStatsBridge.Instance.GetShopOpenClosed();
        // Sliders should be interactable only when shop is closed
        bool interactable = !shopOpen;

        foreach (var card in _activeCards)
        {
            if (card != null)
                card.SetSliderInteractable(interactable);
        }
    }

    private void OnShopStateChanged(OnShopStateChanging eventData)
    {
        UpdateSlidersInteractable();
    }

    #endregion
}