using UnityEngine;

public class MoneyBalance : MonoBehaviour, IMoneyBalance
{
    [SerializeField] private UIMoneyBalance _ui;

    [SerializeField] private bool _cheatsEnabled = false;
    [SerializeField] private KeyCode _cheatingKeyCode;
    [SerializeField] private int _cheatingAmount = 10;

    private int _moneyAmount = 0;

    private void Update()
    {
        if (!_cheatsEnabled)
            return;

        if (Input.GetKeyDown(_cheatingKeyCode))
        {
            AddMoney(_cheatingAmount, "DebugCheats");
        }
    }

    public void Initialize(int moneyAmount)
    {
        //SAVINGSYS
        _moneyAmount = moneyAmount;
        _ui.SetMoneyUI(moneyAmount);
    }

    public void AddMoney(int amount, string description)
    {
        _moneyAmount += amount;
        GlobalStatsBridge.Instance.AddTotalEarned(amount);
        GlobalStatsBridge.Instance.AddSummaryDailyEarn(description + " " + amount);

        _ui.SetMoneyUI(_moneyAmount);
    }

    public void RemoveMoney(int amount, string description)
    {
        _moneyAmount -= amount;
        GlobalStatsBridge.Instance.AddTotalSpent(amount);
        GlobalStatsBridge.Instance.AddSummaryDailyExpenses(description + " " + amount);
        _ui.SetMoneyUI(_moneyAmount);
    }

    public void SetMoney(int value)
    {
        _moneyAmount = value;
        GlobalStatsBridge.Instance.SetMoney(value);
        _ui.SetMoneyUI(_moneyAmount);
    }

    public bool GetPriceAvailability(int price)
    {
        return price <= _moneyAmount;
    }

    public int GetMoney() => _moneyAmount;

    public bool GetCheatsEnabledState() => _cheatsEnabled;

    public void ChangeCheatsState() => _cheatsEnabled = !_cheatsEnabled;
}
