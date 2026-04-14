public interface IMoneyBalance
{
    public void Initialize(int moneyAmount);

    public void AddMoney(int amount, string description);

    public void RemoveMoney(int amount, string description);

    public void SetMoney(int value);

    public int GetMoney();

    public bool GetPriceAvailability(int price);
}