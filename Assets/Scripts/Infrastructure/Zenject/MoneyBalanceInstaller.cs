using UnityEngine;
using Zenject;

public class MoneyBalanceInstaller : MonoInstaller
{
    [SerializeField] private MoneyBalance _moneyBalance;

    public override void InstallBindings()
    {
        Container.Bind<IMoneyBalance>().FromInstance(_moneyBalance).AsSingle();
    }
}
