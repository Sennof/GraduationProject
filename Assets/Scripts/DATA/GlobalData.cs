
using System;
using System.Collections.Generic;

[Serializable]
public class GlobalData
{
    public bool isShopOpened = false;

    public int TotalBuyers = 0;
    public int TotalProducts = 0;

    public int Money = 0;
    public int TotalEarned = 0;
    public int TotalSpent = 0;
    public int MaxEarned = 0;

    public float PricingMod = 1.2f;

    public int TotalDeliveries = 0;

    public List<string> SummaryDailyEarn = new();
    public List<string> SummaryDailyExpenses = new();

    public float Rating = 1;

    public void ResetDayMoneyStats()
    {
        SummaryDailyEarn.Clear();
        SummaryDailyExpenses.Clear();
    }
}
