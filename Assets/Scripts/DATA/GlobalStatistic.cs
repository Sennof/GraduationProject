
using System.Collections.Generic;

public static class GlobalStatistic
{
    #region Money
    public static int Money = 0;
    public static int TotalEarned = 0;
    public static int TotalSpent = 0;

    public static List<string> SummaryDailyEarn = new();
    public static List<string> SummaryDailyExpenses = new();
    #endregion
}
