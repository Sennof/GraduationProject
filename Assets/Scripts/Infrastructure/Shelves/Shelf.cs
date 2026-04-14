using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour, IInitializeable
{
    #region Fields

    [Header("Dependencies")]
    [Tooltip("Inventory reference.")]
    [SerializeField] private Inventory _inventory;

    [Header("Settings")]
    [Tooltip("Slots belonging to this shelf.")]
    [SerializeField] private List<ShelfSlot> _slots;
    [Tooltip("Size category of items this shelf holds.")]
    [SerializeField] private ObjectSizeEnum _objectSize;
    [Tooltip("Navigation point for AI agents.")]
    [SerializeField] private Transform _navPoint;

    private EventBinding<ShelfDataResponsingEvent> _dataBinding;
    private bool _initialized = false;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        _dataBinding = new EventBinding<ShelfDataResponsingEvent>(GetData);
        EventBus<ShelfDataResponsingEvent>.Register(_dataBinding);

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
        int counter = Random.Range(1, _slots.Count);

        for (int i = 0; i < counter; i++)
        {
            int slotId = Random.Range(0, _slots.Count);
            GameObject found = _slots[slotId].TryGetItem();

            if (found != null)
            {
                return found;
            }
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

    #endregion


    #region Unity Methods

    private void OnDisable()
    {
        EventBus<OnShelfInitializationEvent>.Raise(new OnShelfInitializationEvent
        {
            GlobalPosition = _navPoint.position,
            Adding = false,
            Shelf = this
        });
        EventBus<ShelfDataResponsingEvent>.Deregister(_dataBinding);
    }

    #endregion
}