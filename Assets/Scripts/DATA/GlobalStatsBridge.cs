using System.Collections.Generic;
using UnityEngine;

public class GlobalStatsBridge : MonoBehaviour, IInitializeable
{
    public static GlobalStatsBridge Instance { get; private set; }

    private GlobalData _data;

    public void Initialize()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        _data = new GlobalData();
    }

    public void SetData(GlobalData data)
    {
        if (data == null) return;
        _data = data;
    }

    #region CHANGING DATA
    public void ResetDayData() => _data.ResetDayMoneyStats();

    public void SetPricingMod(float value) => _data.PricingMod = value;

    public void AddMoney(int value) => _data.Money += value;

    public void ReduceMoney(int value) => _data.Money -= value;

    public void SetMoney(int value) => _data.Money = value;

    public void AddTotalEarned(int value) => _data.TotalEarned += value;

    public void AddTotalSpent(int value) => _data.TotalSpent += value;

    public void AddSummaryDailyEarn(string text) => _data.SummaryDailyEarn.Add(text);

    public void AddSummaryDailyExpenses(string text) => _data.SummaryDailyExpenses.Add(text);

    public void AddRating(float value) => _data.Rating += value;

    public void ReduceRating(float value)
    {
        _data.Rating -= value;
        if(_data.Rating < 0) _data.Rating = 0;
    }
    #endregion

    public void SetShopOpenClosed(bool state) => _data.isShopOpened = state;


    #region GETTING
    public float GetPricingMod() => _data.PricingMod;

    public List<string> GetSummaryDailyEarn() => _data.SummaryDailyEarn;

    public List<string> GetSummaryDailyExpenses() => _data.SummaryDailyExpenses;

    public bool GetShopOpenClosed() => _data.isShopOpened;
    #endregion
}
