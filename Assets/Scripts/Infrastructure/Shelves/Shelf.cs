using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour, IInitializeable
{
    [Header("Dependences")]
    [SerializeField] private Inventory _inventory;

    [Header("Main values")]
    [SerializeField] private List<ShelfSlot> _slots;
    [SerializeField] private ObjectSizeEnum _objectSize;

    private EventBinding<ShelfDataResponsingEvent> _binding;

    private bool _initialized = false;

    public void Initialize()
    {
        if (_initialized) return;

        _binding = new EventBinding<ShelfDataResponsingEvent>(GetData);
        EventBus<ShelfDataResponsingEvent>.Register(_binding);

        EventBus<ShelfDataRequestingEvent>.Raise(new ShelfDataRequestingEvent { Target = gameObject });
        InitializeSlots();

        _initialized = true;
    }

    private void OnDisable()
    {
        EventBus<ShelfDataResponsingEvent>.Deregister(_binding);
    }

    public void InitializeSlots()
    {
        foreach(ShelfSlot slot in _slots)
        {
            slot.Initialize(_inventory, _objectSize);
        }
    }

    private void GetData(ShelfDataResponsingEvent eventData)
    {
        if (eventData.Target != gameObject) return;

        _inventory = eventData.Inventory;
    }
}
