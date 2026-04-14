using UnityEngine;

public class MoneyBalance : MonoBehaviour, IMoneyBalance
{
    #region Fields

    [Header("Configuration")]
    [Tooltip("Settings configuration asset.")]
    [SerializeField] private SettingsConfigurationSO _settings;

    [Header("UI")]
    [Tooltip("UI component displaying money balance.")]
    [SerializeField] private UIMoneyBalance _ui;

    [Header("Cheats")]
    [Tooltip("Key to add cheat money.")]
    [SerializeField] private KeyCode _cheatingKeyCode = KeyCode.M;
    [Tooltip("Amount added per cheat press.")]
    [SerializeField] private int _cheatingAmount = 10;

    private int _moneyAmount = 0;

    #endregion


    #region Public Methods

    public void Initialize(int moneyAmount)
    {
        _moneyAmount = moneyAmount;
        _ui.SetMoneyUI(moneyAmount);
    }

    public void AddMoney(int amount, string description)
    {
        _moneyAmount += amount;
        GlobalStatsBridge.Instance.AddSummaryDailyEarn(description + " " + amount);
        GlobalStatsBridge.Instance.AddMoney(amount);
        _ui.SetMoneyUI(_moneyAmount);
    }

    public void RemoveMoney(int amount, string description)
    {
        _moneyAmount -= amount;
        GlobalStatsBridge.Instance.AddSummaryDailyExpenses(description + " " + amount);
        GlobalStatsBridge.Instance.ReduceMoney(amount);
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

    #endregion


    #region Unity Methods

    private void Update()
    {
        if (!_settings.IsCheatsEnabled)
        {
            return;
        }

        if (Input.GetKeyDown(_cheatingKeyCode))
        {
            AddMoney(_cheatingAmount, "DebugCheats");
        }
    }

    #endregion
}