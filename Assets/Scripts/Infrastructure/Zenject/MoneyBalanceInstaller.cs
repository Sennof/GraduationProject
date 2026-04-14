using UnityEngine;
using Zenject;

public class MoneyBalanceInstaller : MonoInstaller
{
    #region Fields

    [Header("Instance")]
    [Tooltip("MoneyBalance instance to bind.")]
    [SerializeField] private MoneyBalance _moneyBalance;

    #endregion


    #region Zenject

    public override void InstallBindings()
    {
        Container.Bind<IMoneyBalance>().FromInstance(_moneyBalance).AsSingle();
    }

    #endregion
}