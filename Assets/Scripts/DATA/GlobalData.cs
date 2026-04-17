using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GlobalData
{
    [Tooltip("Current open/closed state of the shop.")]
    public bool IsShopOpened = false;

    [Tooltip("Total number of visitors ever.")]
    public int TotalBuyers = 0;

    [Tooltip("Total products sold.")]
    public int TotalProducts = 0;

    [Tooltip("Current money balance.")]
    public int Money = 0;

    [Tooltip("Total money earned all time.")]
    public int TotalEarned = 0;

    [Tooltip("Total money spent all time.")]
    public int TotalSpent = 0;

    [Tooltip("Maximum amount earned in a single transaction.")]
    public int MaxEarned = 0;

    [Tooltip("Total number of deliveries made.")]
    public int TotalDeliveries = 0;

    [Tooltip("Daily earnings log.")]
    public List<string> SummaryDailyEarn = new();

    [Tooltip("Daily expenses log.")]
    public List<string> SummaryDailyExpenses = new();

    [Tooltip("Current shop rating (0-5).")]
    public float Rating = 1;

    [Tooltip("Individual product markups (key = product TitleName).")]
    public Dictionary<string, float> ProductMarkups = new();

    public void ResetDayMoneyStats()
    {
        SummaryDailyEarn.Clear();
        SummaryDailyExpenses.Clear();
    }
}