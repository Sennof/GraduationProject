using Unity.VisualScripting;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInitializable
{
    #region Data Settings
    [Header("Main Metadata")]
    [SerializeField] private InteractableObjectTypeEnum _type;
    [SerializeField] private ObjectSizeEnum _size;
    [SerializeField] private Sprite _icon;
    #endregion

    [Space(10)]

    #region Physics Settings
    [Header("Physics Components")]
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Collider _collider;
    #endregion

    [Space(10)]

    #region Hierarchy Settings
    [Header("Placement")]
    [SerializeField] private Transform _defaultParent;
    #endregion

    [Space(10)]

    #region Optional Components
    [Header("Optional Modules")]

    [Tooltip("UNNECESSARY (needs to be only if the object has)")]
    [SerializeField] private InteractingObject _interactingObject;

    [Tooltip("UNNECESSARY (needs to be only if the object has)")]
    [SerializeField] private BuildingObject _buildingObject;
    #endregion

    #region Internal State
    private Vector3 _scale;
    #endregion

    public void Initialize()
    {
        _scale = transform.localScale;
        _defaultParent = transform.parent;

        if (_buildingObject != null)
            _buildingObject.Initialize(_defaultParent);
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
        if (_buildingObject != null)
        {
            _buildingObject.SetInHands();
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
        if (_buildingObject != null)
        {
            _buildingObject.SetOutHands();
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
        if (_buildingObject != null)
        {
            _buildingObject.SetOutHands();
        }
    }

    public Sprite GetIcon() => _icon;

    public Transform GetDefaultParent() => _defaultParent;

    public ObjectSizeEnum GetSize() => _size;

    public InteractableObjectTypeEnum GetObjectType() => _type;
}