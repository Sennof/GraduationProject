using Unity.VisualScripting;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInitializable
{
    #region Fields

    [Header("Main Metadata")]
    [Tooltip("Associated product data.")]
    [SerializeField] private ProductData _productData;
    [Tooltip("Type of interactable object.")]
    [SerializeField] private InteractableObjectTypeEnum _type;
    [Tooltip("Size category of the object.")]
    [SerializeField] private ObjectSizeEnum _size;
    [Tooltip("Icon displayed in UI.")]
    [SerializeField] private Sprite _icon;

    [Space(10)]

    [Header("Physics Components")]
    [Tooltip("Layer mask to exclude when held.")]
    [SerializeField] private LayerMask _layerMask;
    [Tooltip("Rigidbody component.")]
    [SerializeField] private Rigidbody _rigidbody;
    [Tooltip("Collider component.")]
    [SerializeField] private Collider _collider;

    [Space(10)]

    [Header("Placement")]
    [Tooltip("Default parent transform when dropped.")]
    [SerializeField] private Transform _defaultParent;

    [Space(10)]

    [Header("Optional Modules")]
    [Tooltip("InteractingObject component if present.")]
    [SerializeField] private InteractingObject _interactingObject;
    [Tooltip("BuildingObject component if present.")]
    [SerializeField] private BuildingObject _buildingObject;

    private Vector3 _originalScale;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        _originalScale = transform.localScale;
        _defaultParent = transform.parent;

        if (_buildingObject != null)
        {
            _buildingObject.Initialize(_defaultParent);
        }
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

        if (_interactingObject != null)
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
        _rigidbody.AddForce(-transform.right * _rigidbody.mass * 8, ForceMode.Impulse);
        transform.localScale = _originalScale;

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
        transform.localScale = _originalScale;
        transform.position += -transform.right * 0.5f;
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

    public ProductData GetProductData() => _productData;

    public Sprite GetIcon() => _icon;

    public Transform GetDefaultParent() => _defaultParent;

    public ObjectSizeEnum GetSize() => _size;

    public InteractableObjectTypeEnum GetObjectType() => _type;

    #endregion
}