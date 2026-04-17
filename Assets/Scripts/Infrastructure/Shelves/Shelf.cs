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
    [Tooltip("Can customers visit this shelf?")]
    [SerializeField] private bool _isVisitable = true;

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

        if (_isVisitable)
        {
            EventBus<OnShelfInitializationEvent>.Raise(new OnShelfInitializationEvent
            {
                GlobalPosition = _navPoint.position,
                Adding = true,
                Shelf = this
            });
        }

        _initialized = true;
    }

    public GameObject PrepareProduct()
    {
        foreach (ShelfSlot slot in _slots)
        {
            GameObject found = slot.TryGetItem();
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

    public void ReturnProduct(GameObject product)
    {
        foreach (ShelfSlot slot in _slots)
        {
            if (slot.CanAcceptItem(product))
            {
                slot.ReturnItem(product);
                return;
            }
        }
        Destroy(product);
    }

    public Vector3 GetNavPointPosition() => _navPoint.position;

    public bool IsVisitable() => _isVisitable;

    public int GetTotalItemCount()
    {
        int count = 0;
        foreach (ShelfSlot slot in _slots)
        {
            count += slot.GetKeptObjectsCount();
        }
        return count;
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
        if (_isVisitable)
        {
            EventBus<OnShelfInitializationEvent>.Raise(new OnShelfInitializationEvent
            {
                GlobalPosition = _navPoint.position,
                Adding = false,
                Shelf = this
            });
        }
        EventBus<ShelfDataResponsingEvent>.Deregister(_dataBinding);
    }

    #endregion
}