using Unity.VisualScripting;
using UnityEngine;

public class DeliveryAnimationHandler : MonoBehaviour, IInitializeable, IAnimationHandler
{
    [SerializeField] private int _animId = 0;
    [SerializeField] private UIAnimsManager _animsManager;
    private EventBinding<DeliveryResponseEvent> _eventBinding;

    public void Initialize()
    {
        _eventBinding = new EventBinding<DeliveryResponseEvent>(Handle);
        EventBus<DeliveryResponseEvent>.Register(_eventBinding);
    }

    public void Handle()
    {
        _animsManager.PlayAnimation(_animId);
    }

    public void DeInit()
    {
        EventBus<DeliveryResponseEvent>.Deregister(_eventBinding);
    }

    private void OnDisable() => DeInit();
}
