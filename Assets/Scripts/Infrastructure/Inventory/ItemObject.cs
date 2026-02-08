using Unity.VisualScripting;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInitializable
{
    [SerializeField] private Sprite _icon;

    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Collider _collider;

    [SerializeField] private Transform _defaultParent;

    private Vector3 _scale;

    public void Initialize()
    {
        _scale = transform.localScale;
        _defaultParent = transform.parent;
    }

    public void InvokePickUpEvent()
    {
        EventBus<ItemPickUpEvent>.Raise(new ItemPickUpEvent
        {
            ItemObjectData = this,
            ItemGameObject = gameObject,
        });
    }

    public void PickUp()
    {
        transform.localPosition = Vector3.zero;
        _rigidbody.isKinematic = true;
        transform.rotation = new Quaternion(0, 0, 0, 0);
        _collider.excludeLayers = _layerMask; 
        
        transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
    }

    public void Throw()
    {
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(transform.forward * _rigidbody.mass * 8, ForceMode.Impulse);
        _collider.excludeLayers = 0;

        transform.localScale = _scale;
    }

    public void Drop()
    {
        _rigidbody.isKinematic = false;
        transform.localScale = _scale;
        transform.position += transform.forward * 0.5f;
        _collider.excludeLayers = 0;
    }

    public Sprite GetIcon() => _icon;

    public Transform GetDefaultParent() => _defaultParent;

}
