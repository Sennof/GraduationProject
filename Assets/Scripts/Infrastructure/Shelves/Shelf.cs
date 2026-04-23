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

    [Header("Price Tags")]
    [Tooltip("Transform anchor points where price tags can be placed.")]
    [SerializeField] private Transform[] _priceTagAttachPoints;

    private Dictionary<ProductData, PriceTag> _priceTags = new();
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

    public void SetVisitable(bool visitable)
    {
        if (_isVisitable == visitable) return;

        _isVisitable = visitable;

        if (_isVisitable)
        {
            EventBus<OnShelfInitializationEvent>.Raise(new OnShelfInitializationEvent
            {
                GlobalPosition = _navPoint.position,
                Adding = true,
                Shelf = this
            });
        }
        else
        {
            EventBus<OnShelfInitializationEvent>.Raise(new OnShelfInitializationEvent
            {
                GlobalPosition = _navPoint.position,
                Adding = false,
                Shelf = this
            });
        }
    }

    public int GetTotalItemCount()
    {
        int count = 0;
        foreach (ShelfSlot slot in _slots)
        {
            count += slot.GetKeptObjectsCount();
        }
        return count;
    }

    public void RegisterPriceTag(PriceTag tag)
    {
        ProductData product = tag.GetTargetProduct();
        if (product == null) return;

        if (_priceTags.TryGetValue(product, out PriceTag existing) && existing != tag)
            existing.Detach();

        _priceTags[product] = tag;
    }

    public void UnregisterPriceTag(PriceTag tag)
    {
        ProductData product = tag.GetTargetProduct();
        if (product == null) return;

        if (_priceTags.TryGetValue(product, out PriceTag registered) && registered == tag)
            _priceTags.Remove(product);
    }

    public float GetProductMarkup(ProductData data)
    {
        if (data == null) return 0f;
        if (_priceTags.TryGetValue(data, out PriceTag tag))
            return tag.GetMarkup();
        return GlobalStatsBridge.Instance.GetProductMarkup(data.TitleName);
    }

    public Transform GetNextFreeAttachPoint()
    {
        if (_priceTagAttachPoints == null) return null;
        foreach (Transform point in _priceTagAttachPoints)
        {
            if (point != null && point.childCount == 0)
                return point;
        }
        return null;
    }

    public bool HasPriceTagForProduct(ProductData data) => data != null && _priceTags.ContainsKey(data);

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