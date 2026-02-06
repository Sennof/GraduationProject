using UnityEngine;

public class MoneyBalance : MonoBehaviour, IMoneyBalance
{
    private int _moneyAmount = 0;

    public void Initialize(int moneyAmount)
    {
        //SAVINGSYS
        _moneyAmount = moneyAmount;
    }

    public void AddMoney(int amount)
    {
        _moneyAmount += amount;
        GlobalStatistic.TotalEarned += amount;
    }

    public void RemoveMoney(int amount)
    {
        _moneyAmount -= amount;
        GlobalStatistic.TotalEarned += amount;
    }

    public void SetMoney(int value)
    {
        _moneyAmount = value;
    }

    public int GetMoney() => _moneyAmount;

}
