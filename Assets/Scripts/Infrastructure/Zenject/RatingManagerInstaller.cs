using UnityEngine;
using Zenject;

public class RatingManagerInstaller : MonoInstaller
{
    [SerializeField] private RatingManager _instance;

    public override void InstallBindings()
    {
        Container.Bind<IRatingManager>().FromInstance(_instance).AsSingle();
    }
}
