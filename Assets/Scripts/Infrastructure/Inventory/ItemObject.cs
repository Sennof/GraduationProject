using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInitializable
{
    [SerializeField] private Sprite _icon;

    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Collider _collider;

    [SerializeField] private Transform _defaultParent;

    [Tooltip("UNNESSESARY (needs to be only if the object has)")]
    [SerializeField] private InteractingObject _interactingObject;

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
        
        transform.localScale = new Vector3(transform.localScale.x * 0.75f, transform.localScale.y * 0.75f, transform.localScale.z * 0.75f);

        if(_interactingObject != null)
        {
            _interactingObject.SetInHands();
        }
    }

    public void Throw()
    {
        _rigidbody.isKinematic = false;
        _collider.excludeLayers = 0;
        _rigidbody.AddForce(-transform.right * _rigidbody.mass * 8, ForceMode.Impulse); //using "-transform.right" beacuse of the rotation of the parent object "hands"
        transform.localScale = _scale;

        if (_interactingObject != null)
        {
            _interactingObject.SetOutHands();
        }
    }

    public void Drop()
    {
        _rigidbody.isKinematic = false;
        transform.localScale = _scale;
        transform.position += -transform.right * 0.5f; //using "-transform.right" beacuse of the rotation of the parent object "hands"
        _collider.excludeLayers = 0;

        if (_interactingObject != null)
        {
            _interactingObject.SetOutHands();
        }
    }

    public Sprite GetIcon() => _icon;

    public Transform GetDefaultParent() => _defaultParent;

}
