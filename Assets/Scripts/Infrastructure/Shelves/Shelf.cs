using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour, IInitializeable
{
    #region Fields
    [Header("Dependencies")]
    [SerializeField] private Inventory _inventory;

    [Header("Settings")]
    [SerializeField] private List<ShelfSlot> _slots;
    [SerializeField] private ObjectSizeEnum _objectSize;
    [SerializeField] private Transform _navPoint;

    private EventBinding<ShelfDataResponsingEvent> _binding;
    private bool _initialized = false;
    #endregion

    #region Public Methods
    public void Initialize()
    {
        if (_initialized) return;

        _binding = new EventBinding<ShelfDataResponsingEvent>(GetData);
        EventBus<ShelfDataResponsingEvent>.Register(_binding);

        EventBus<ShelfDataRequestingEvent>.Raise(new ShelfDataRequestingEvent { Target = gameObject });

        foreach (ShelfSlot slot in _slots)
        {
            slot.Initialize(_inventory, _objectSize);
        }

        EventBus<OnShelfInitializationEvent>.Raise(new OnShelfInitializationEvent
        {
            GlobalPosition = _navPoint.position,
            Adding = true,
            Shelf = this
        });

        _initialized = true;
    }

    public GameObject PrepareProduct()
    {
        foreach (ShelfSlot slot in _slots)
        {
            GameObject found = slot.TryGetItem();
            if (found != null) return found;
        }
        return null;
    }
    #endregion

    #region Private Methods
    private void GetData(ShelfDataResponsingEvent eventData)
    {
        if (eventData.Target == gameObject)
        {
            _inventory = eventData.Inventory;
        }
    }

    private void OnDisable()
    {
        EventBus<OnShelfInitializationEvent>.Raise(new OnShelfInitializationEvent
        {
            GlobalPosition = _navPoint.position,
            Adding = false,
            Shelf = this
        });
        EventBus<ShelfDataResponsingEvent>.Deregister(_binding);
    }
    #endregion
}