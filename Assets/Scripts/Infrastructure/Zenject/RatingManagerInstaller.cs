using UnityEngine;
using Zenject;

public class RatingManagerInstaller : MonoInstaller
{
    #region Fields

    [Header("Instance")]
    [Tooltip("RatingManager instance to bind.")]
    [SerializeField] private RatingManager _instance;

    #endregion


    #region Zenject

    public override void InstallBindings()
    {
        Container.Bind<IRatingManager>().FromInstance(_instance).AsSingle();
    }

    #endregion
}