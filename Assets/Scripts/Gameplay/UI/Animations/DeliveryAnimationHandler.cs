using Unity.VisualScripting;
using UnityEngine;

public class DeliveryAnimationHandler : MonoBehaviour, IInitializeable, IAnimationHandler
{
    #region Fields

    [Header("Settings")]
    [Tooltip("ID of the animation to play.")]
    [SerializeField] private int _animId = 0;

    [Header("Dependencies")]
    [Tooltip("UI animation manager reference.")]
    [SerializeField] private UIAnimsManager _animsManager;

    private EventBinding<DeliveryResponseEvent> _eventBinding;

    #endregion


    #region Public Methods

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

    #endregion


    #region Unity Methods

    private void OnDisable() => DeInit();

    #endregion
}