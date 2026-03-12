using UnityEngine;

public class ShelfInitializer : MonoBehaviour, IInitializeable
{
    [SerializeField] private Inventory _inventory;

    private EventBinding<ShelfDataRequestingEvent> _binding;

    public void Initialize()
    {
        _binding = new EventBinding<ShelfDataRequestingEvent>(PushData);
        EventBus<ShelfDataRequestingEvent>.Register(_binding);
    }

    private void OnDisable()
    {
        EventBus<ShelfDataRequestingEvent>.Deregister(_binding);
    }

    private void PushData(ShelfDataRequestingEvent eventData)
    {
        EventBus<ShelfDataResponsingEvent>.Raise(new ShelfDataResponsingEvent { Target = eventData.Target, Inventory = _inventory });
    }
}
