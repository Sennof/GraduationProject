using System.Collections.Generic;
using UnityEngine;

public class ShelfSlot : MonoBehaviour
{
    [SerializeField] private bool _enabled = true;
    [SerializeField] private ObjectSizeEnum _size;

    [SerializeField] private int _capacity = 1;
    [SerializeField] private List<GameObject> _keptObjects;

    private Inventory _inventory;

    public void Initialize(Inventory inventory, ObjectSizeEnum size)
    {
        _inventory = inventory;
        _size = size;
    }

    public void SetInSlot() //via inspector
    {
        if (_enabled == false) return;
        if (_keptObjects.Count >= _capacity) return;

        GameObject targetObject = _inventory.GetCurrentItem();
        if (targetObject == null) return;

        ItemObject targetObjectManager = targetObject.GetComponent<ItemObject>();
        if (targetObjectManager == null) return;
        if(targetObjectManager.GetSize() != _size) return;

        _keptObjects.Add(targetObject);
        BoxCollider objCollider = targetObject.GetComponent<BoxCollider>();

        _inventory.DropObj();
        targetObject.transform.SetParent(transform);
        targetObject.GetComponent<Rigidbody>().isKinematic = true;
        targetObject.transform.rotation = Quaternion.identity; //rewrite bro (you should make rotation of 90 degrees via code)
        targetObject.transform.localPosition = new Vector3(0, (objCollider.size.y * targetObject.transform.localScale.y) / 2f, 0);
    }

    public void GetAwayFromSlot() //via inspector
    {
        if (_enabled == false) return;
        if (_keptObjects.Count < 1) return;
        if (_inventory.CanPickUpMore() == false) return;

        GameObject keptObject = _keptObjects[_keptObjects.Count - 1];

        keptObject.transform.SetParent(keptObject.GetComponent<ItemObject>().GetDefaultParent());
        keptObject.GetComponent<Rigidbody>().isKinematic = false;
        keptObject.transform.rotation = Quaternion.identity;
        _inventory.PickUp(keptObject.GetComponent<ItemObject>());

        _keptObjects.Remove(keptObject);
    }
}
