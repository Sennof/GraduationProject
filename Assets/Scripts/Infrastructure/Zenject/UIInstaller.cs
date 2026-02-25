using UnityEngine;
using Zenject;

public class UIInstaller : MonoInstaller
{
    [SerializeField] private MoneyBalance _moneyBalance; //not ui!!! fuck rewrite!!!

    public override void InstallBindings()
    {
        Container.Bind<IMoneyBalance>().FromInstance(_moneyBalance).AsSingle();
    }
}
