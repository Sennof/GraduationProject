public interface IMoneyBalance
{
    public void Initialize(int moneyAmount);

    public void AddMoney(int amount);

    public void RemoveMoney(int amount);

    public void SetMoney(int value); // DEBUG FEATURE

    public int GetMoney();
}
