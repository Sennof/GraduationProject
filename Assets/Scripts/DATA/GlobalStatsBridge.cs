using System.Collections.Generic;
using UnityEngine;

public class GlobalStatsBridge : MonoBehaviour, IInitializeable
{
    #region Fields

    [Header("Resource Paths")]
    [Tooltip("Paths to ProductData assets inside Resources folder (e.g., 'Products', 'Consumables').")]
    [SerializeField] private string[] _productResourcePaths = { "Products" };

    private GlobalData _data;
    public static GlobalStatsBridge Instance { get; private set; }

    #endregion


    #region Public Methods

    public void Initialize()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        _data = new GlobalData();
        LoadDefaultMarkups();
    }

    public void SetData(GlobalData data)
    {
        if (data == null) return;
        _data = data;
        LoadDefaultMarkups();
    }

    #endregion


    #region Markup Management

    private void LoadDefaultMarkups()
    {
        HashSet<ProductData> allProducts = new HashSet<ProductData>();

        foreach (string path in _productResourcePaths)
        {
            ProductData[] loaded = Resources.LoadAll<ProductData>(path);
            if (loaded != null && loaded.Length > 0)
            {
                allProducts.UnionWith(loaded);
            }
        }

        if (allProducts.Count == 0)
        {
            Debug.LogWarning("No ProductData found in specified resource paths. Default markups will be 0.2.");
            return;
        }

        foreach (ProductData product in allProducts)
        {
            if (!_data.ProductMarkups.ContainsKey(product.TitleName))
            {
                _data.ProductMarkups[product.TitleName] = product.DefaultMarkup;
            }
        }
    }

    public float GetProductMarkup(string productId)
    {
        if (_data.ProductMarkups.TryGetValue(productId, out float markup))
            return markup;
        return 0.2f;
    }

    public void SetProductMarkup(string productId, float markup)
    {
        _data.ProductMarkups[productId] = Mathf.Clamp(markup, 0f, 2f);
    }

    #endregion


    #region Data Modification

    public void ResetDayData() => _data.ResetDayMoneyStats();

    public void AddMoney(int value)
    {
        _data.Money += value;
        _data.TotalEarned += value;
        if (value > _data.MaxEarned) _data.MaxEarned = value;
    }

    public void ReduceMoney(int value)
    {
        _data.Money -= value;
        _data.TotalSpent += value;
    }

    public void SetMoney(int value) => _data.Money = value;

    public void AddSummaryDailyEarn(string text) => _data.SummaryDailyEarn.Add(text);

    public void AddSummaryDailyExpenses(string text) => _data.SummaryDailyExpenses.Add(text);

    public void AddRating(float value) => _data.Rating += value;

    public void ReduceRating(float value)
    {
        _data.Rating -= value;
        if (_data.Rating < 0) _data.Rating = 0;
    }

    public void SetRating(float value) => _data.Rating = value;

    public void SetShopOpenClosed(bool state) => _data.IsShopOpened = state;

    public void AddTotalVisitors() => _data.TotalBuyers++;

    public void AddTotalProducts(int amount) => _data.TotalProducts += amount;

    public void AddTotalDeliveries() => _data.TotalDeliveries++;

    #endregion


    #region Data Retrieval

    public List<string> GetSummaryDailyEarn() => _data.SummaryDailyEarn;
    public List<string> GetSummaryDailyExpenses() => _data.SummaryDailyExpenses;
    public bool GetShopOpenClosed() => _data.IsShopOpened;
    public float GetRating() => _data.Rating;
    public int GetTotalBuyers() => _data.TotalBuyers;
    public int GetTotalProducts() => _data.TotalProducts;
    public int GetMaxEarned() => _data.MaxEarned;
    public int GetTotalDeliveries() => _data.TotalDeliveries;
    public int GetTotalEarned() => _data.TotalEarned;
    public int GetTotalSpent() => _data.TotalSpent;

    #endregion
}