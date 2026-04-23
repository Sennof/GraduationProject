using System.Collections.Generic;
using UnityEngine;

// InteractingObject._triggerKey must be set to KeyCode.None in the prefab Inspector
// to prevent it from consuming key presses that PriceTagMaker handles itself.
[RequireComponent(typeof(InteractingObject))]
public class PriceTagMaker : MonoBehaviour, IInitializeable
{
    #region Fields

    [Header("Dependencies")]
    [Tooltip("InteractingObject used only to track in-hands state.")]
    [SerializeField] private InteractingObject _interactingObject;
    [Tooltip("UI panel for creating and configuring price tags.")]
    [SerializeField] private UIPriceTagCreator _uiCreator;
    [Tooltip("Prefab instantiated when placing a configured tag onto a shelf.")]
    [SerializeField] private GameObject _priceTagPrefab;

    [Header("Settings")]
    [Tooltip("Key to open/close the tag creation UI.")]
    [SerializeField] private KeyCode _openUIKey = KeyCode.F;
    [Tooltip("Key to place the currently selected tag on an aimed shelf.")]
    [SerializeField] private KeyCode _placeTagKey = KeyCode.E;
    [Tooltip("Key to cycle forward through configured tags.")]
    [SerializeField] private KeyCode _cycleTagKey = KeyCode.Q;
    [Tooltip("Raycast distance for detecting shelves during placement.")]
    [SerializeField] private float _placeDistance = 3f;
    [Tooltip("Initial number of blank tags available before any rolls are used.")]
    [SerializeField] private int _initialCapacity = 5;

    [Header("State")]
    [Tooltip("Remaining blank tag capacity.")]
    [SerializeField][ReadOnly] private int _remainingCapacity;
    [Tooltip("Index of the currently selected configured tag.")]
    [SerializeField][ReadOnly] private int _selectedTagIndex;

    private Transform _raycastStartPoint;
    private Inventory _inventory;
    private List<(ProductData product, float markup)> _configuredTags = new();

    private EventBinding<PriceTagMakerDataResponsingEvent> _responseBinding;
    private EventBinding<CreatePriceTagsRequestEvent> _createTagsBinding;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        _remainingCapacity = _initialCapacity;

        _responseBinding = new EventBinding<PriceTagMakerDataResponsingEvent>(HandleDataResponse);
        EventBus<PriceTagMakerDataResponsingEvent>.Register(_responseBinding);

        _createTagsBinding = new EventBinding<CreatePriceTagsRequestEvent>(HandleCreateTagsRequest);
        EventBus<CreatePriceTagsRequestEvent>.Register(_createTagsBinding);

        EventBus<PriceTagMakerDataRequestingEvent>.Raise(new PriceTagMakerDataRequestingEvent { Target = gameObject });

        if (_uiCreator != null)
            _uiCreator.Initialize(this);
    }

    public void RefillCapacity(int amount)
    {
        _remainingCapacity += Mathf.Max(0, amount);
    }

    public void AddConfiguredTagFromHanger(PriceTag tag)
    {
        if (tag != null && tag.GetTargetProduct() != null)
            _configuredTags.Add((tag.GetTargetProduct(), tag.GetMarkup()));
        if (tag != null)
            Destroy(tag.gameObject);
    }

    public int GetRemainingCapacity() => _remainingCapacity;

    public int GetConfiguredTagsCount() => _configuredTags.Count;

    public string GetSelectedTagInfo()
    {
        if (_configuredTags.Count == 0) return "No tags";
        var tag = _configuredTags[GetSafeIndex()];
        return tag.product != null
            ? $"{tag.product.TitleName}  +{Mathf.RoundToInt(tag.markup * 100)}%"
            : "Unknown";
    }

    #endregion


    #region Private Methods

    private int GetSafeIndex() =>
        Mathf.Clamp(_selectedTagIndex, 0, Mathf.Max(0, _configuredTags.Count - 1));

    private void TryPlaceCurrentTag()
    {
        if (_configuredTags.Count == 0 || _raycastStartPoint == null || _priceTagPrefab == null)
            return;

        if (!Physics.Raycast(_raycastStartPoint.position, _raycastStartPoint.forward,
                out RaycastHit hit, _placeDistance))
            return;

        Shelf shelf = hit.collider.GetComponentInParent<Shelf>()
                      ?? hit.collider.GetComponent<Shelf>();
        if (shelf == null) return;

        Transform attachPoint = shelf.GetNextFreeAttachPoint();
        if (attachPoint == null)
        {
            Debug.Log("[PriceTagMaker] No free attach point on this shelf.");
            return;
        }

        int idx = GetSafeIndex();
        var config = _configuredTags[idx];
        _configuredTags.RemoveAt(idx);
        _selectedTagIndex = Mathf.Clamp(_selectedTagIndex, 0, Mathf.Max(0, _configuredTags.Count - 1));

        GameObject tagObj = Instantiate(_priceTagPrefab);
        if (tagObj.TryGetComponent(out PriceTag priceTag))
        {
            priceTag.Configure(config.product, config.markup);
            priceTag.AttachToShelf(shelf, attachPoint);
        }
    }

    private void TryCycleTag()
    {
        if (_configuredTags.Count == 0) return;
        _selectedTagIndex = (_selectedTagIndex + 1) % _configuredTags.Count;
    }

    private void TryUseRollFromOtherSlot()
    {
        if (_inventory == null) return;

        ItemObject otherItem = _inventory.GetOtherSlotItemManager();
        if (otherItem == null) return;

        PriceTagRoll roll = otherItem.GetComponent<PriceTagRoll>();
        if (roll == null) return;

        RefillCapacity(roll.GetTagsCount());
        _inventory.DestroySlot();
    }

    #endregion


    #region Event Handlers

    private void HandleDataResponse(PriceTagMakerDataResponsingEvent eventData)
    {
        if (eventData.Target != gameObject) return;
        _raycastStartPoint = eventData.RaycastStartPoint;
        if (_inventory == null)
            _inventory = eventData.Inventory;
    }

    private void HandleCreateTagsRequest(CreatePriceTagsRequestEvent eventData)
    {
        if (eventData.TargetMaker != this) return;

        int toCreate = Mathf.Min(eventData.Quantity, _remainingCapacity);
        for (int i = 0; i < toCreate; i++)
            _configuredTags.Add((eventData.ProductData, eventData.Markup));

        _remainingCapacity -= toCreate;
    }

    #endregion


    #region Unity Methods

    private void Update()
    {
        if (!_interactingObject.GetIsInHands()) return;

        if (Input.GetKeyDown(_openUIKey))
        {
            if (_uiCreator != null)
                _uiCreator.Toggle();
        }

        if (Input.GetKeyDown(_placeTagKey))
            TryPlaceCurrentTag();

        if (Input.GetKeyDown(_cycleTagKey))
            TryCycleTag();
    }

    private void OnDisable()
    {
        EventBus<PriceTagMakerDataResponsingEvent>.Deregister(_responseBinding);
        EventBus<CreatePriceTagsRequestEvent>.Deregister(_createTagsBinding);

        if (_uiCreator != null)
            _uiCreator.Hide();
    }

    #endregion
}
