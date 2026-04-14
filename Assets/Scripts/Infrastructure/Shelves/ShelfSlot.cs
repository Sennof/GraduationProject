using System.Collections.Generic;
using UnityEngine;

public class ShelfSlot : MonoBehaviour
{
    #region Fields

    [Header("Settings")]
    [Tooltip("Enable or disable this slot.")]
    [SerializeField] private bool _enabled = true;
    [Tooltip("Size category of items this slot accepts.")]
    [SerializeField] private ObjectSizeEnum _size;
    [Tooltip("Maximum number of items this slot can hold.")]
    [SerializeField] private int _capacity = 1;

    [Header("Runtime State")]
    [SerializeField] private List<GameObject> _keptObjects = new();

    private Inventory _inventory;

    #endregion


    #region Public Methods

    public void Initialize(Inventory inventory, ObjectSizeEnum size)
    {
        _inventory = inventory;
        _size = size;
    }

    public void SetInSlot()
    {
        if (!_enabled || _keptObjects.Count >= _capacity)
        {
            return;
        }

        GameObject targetObject = _inventory.GetCurrentItem();
        if (targetObject == null)
        {
            return;
        }

        if (targetObject.TryGetComponent(out ItemObject item))
        {
            if (item.GetSize() != _size)
            {
                return;
            }

            _keptObjects.Add(targetObject);
            _inventory.DropObj();

            targetObject.transform.SetParent(transform);
            if (targetObject.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = true;
            }

            targetObject.transform.rotation = Quaternion.identity;

            if (targetObject.TryGetComponent(out BoxCollider col))
            {
                float yOffset = (col.size.y * targetObject.transform.localScale.y) / 2f;
                targetObject.transform.localPosition = new Vector3(0, yOffset, 0);
            }
        }
    }

    public void GetAwayFromSlot()
    {
        if (!_enabled || _keptObjects.Count < 1 || !_inventory.CanPickUpMore())
        {
            return;
        }

        GameObject keptObject = _keptObjects[_keptObjects.Count - 1];
        if (keptObject.TryGetComponent(out ItemObject item))
        {
            keptObject.transform.SetParent(item.GetDefaultParent());
            if (keptObject.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
            }

            _inventory.PickUp(item);
            _keptObjects.Remove(keptObject);
        }
    }

    public GameObject TryGetItem()
    {
        if (!_enabled || _keptObjects.Count < 1)
        {
            return null;
        }

        int index = Random.Range(0, _keptObjects.Count);
        GameObject item = _keptObjects[index];
        _keptObjects.RemoveAt(index);
        item.SetActive(false);

        return item;
    }

    #endregion
}