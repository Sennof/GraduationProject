using UnityEngine;

public class ShelfInitializer : MonoBehaviour, IInitializeable
{
    #region Fields

    [Header("Dependencies")]
    [Tooltip("Inventory reference.")]
    [SerializeField] private Inventory _inventory;

    private EventBinding<ShelfDataRequestingEvent> _requestBinding;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        _requestBinding = new EventBinding<ShelfDataRequestingEvent>(PushData);
        EventBus<ShelfDataRequestingEvent>.Register(_requestBinding);
    }

    #endregion


    #region Private Methods

    private void PushData(ShelfDataRequestingEvent eventData)
    {
        EventBus<ShelfDataResponsingEvent>.Raise(new ShelfDataResponsingEvent { Target = eventData.Target, Inventory = _inventory });
    }

    #endregion


    #region Unity Methods

    private void OnDisable()
    {
        EventBus<ShelfDataRequestingEvent>.Deregister(_requestBinding);
    }

    #endregion
}