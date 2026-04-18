using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour, IInitializeable
{
    #region Fields

    [Header("Settings")]
    [Tooltip("Global toggle enabling building functionality.")]
    [SerializeField] private bool _canBuilding = false;
    [Tooltip("Grid size for snapping placed objects.")]
    [SerializeField] private float _snapGridSize = 0.25f;
    [Tooltip("Distance of the placement raycast.")]
    [SerializeField] private float _raycastDistance = 6f;
    [Tooltip("Layer mask for the placement raycast.")]
    [SerializeField] private LayerMask _raycastLayerMask;

    [Header("References")]
    [Tooltip("UI panel for building mode hints.")]
    [SerializeField] private UIBuildingMode _uiBuildingMode;
    [Tooltip("Player inventory reference.")]
    [SerializeField] private Inventory _inventory;
    [Tooltip("Starting point of the placement raycast.")]
    [SerializeField] private Transform _raycastStartPoint;
    [Tooltip("Parent transform for instantiated buildings.")]
    [SerializeField] private Transform _targetFolder;

    [Header("Controls")]
    [Tooltip("Key to toggle building mode.")]
    [SerializeField] private KeyCode _buildingModeKeyCode = KeyCode.B;
    [Tooltip("Key to rotate the preview object.")]
    [SerializeField] private KeyCode _rotationKeyCode = KeyCode.R;
    [Tooltip("Key to confirm placement.")]
    [SerializeField] private KeyCode _buildKeyCode = KeyCode.Mouse0;
    [Tooltip("Key to cancel relocation.")]
    [SerializeField] private KeyCode _cancelRelocationKeyCode = KeyCode.X;

    private bool _isBuilding = false;
    private GameObject _targetObjectPrefab;
    private GameObject _targetObject;
    private BoxCollider _targetObjectCollider;
    private Transform _targetObjectTransform;
    private BuildingObject _buildingObject;
    private BuildedObject _buildedObject;

    private List<BuildedObject> _buildedObjectColliders = new();

    private RaycastHit _rayHit;
    private EventBinding<BuildingModeTriggerEvent> _buildingModeTriggerBinding;
    private EventBinding<RemoveBuildingEvent> _removeBuildingBinding;

    private Vector3 _lastCalculatedPosition;
    private bool _isFirstPlacementAttempt = true;

    // Relocation state
    private bool _isRelocating = false;
    private BuildedObject _relocatingBuildedObject;
    private Vector3 _relocationOriginalPosition;
    private Quaternion _relocationOriginalRotation;
    private Transform _relocationOriginalParent;
    private System.Action _onRelocationFinished;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        ResetData();

        _buildingModeTriggerBinding = new EventBinding<BuildingModeTriggerEvent>(HandleBuildingModeTrigger);
        EventBus<BuildingModeTriggerEvent>.Register(_buildingModeTriggerBinding);

        _removeBuildingBinding = new EventBinding<RemoveBuildingEvent>(RemoveBuilding);
        EventBus<RemoveBuildingEvent>.Register(_removeBuildingBinding);

        GetBuildingColliders();
        _uiBuildingMode.Initialize(_buildingModeKeyCode, _rotationKeyCode, _buildKeyCode, _cancelRelocationKeyCode);
    }

    /// <summary>
    /// Starts relocating an already built object.
    /// </summary>
    public void StartRelocatingBuildedObject(BuildedObject target, System.Action onFinished = null)
    {
        if (_isBuilding || _isRelocating) return;

        _relocatingBuildedObject = target;
        _relocationOriginalPosition = target.transform.position;
        _relocationOriginalRotation = target.transform.rotation;
        _relocationOriginalParent = target.transform.parent;
        _onRelocationFinished = onFinished;

        if (_isBuilding) TurnOffBuildMode();
        _canBuilding = false;

        _relocatingBuildedObject.EnableBuilding();
        _relocatingBuildedObject.GetComponent<Collider>().enabled = false;

        _inventory.LockInteractions();
        _inventory.LockChangingSlot();

        _isRelocating = true;
        _isFirstPlacementAttempt = true;

        _uiBuildingMode.SetRelocationText($"Relocating: {_buildKeyCode} confirm, {_cancelRelocationKeyCode} cancel, {_rotationKeyCode} rotate");
    }

    #endregion


    #region Unity Methods

    private void OnDisable()
    {
        if (_isBuilding) TurnOffBuildMode();
        if (_isRelocating) CancelRelocation();

        EventBus<BuildingModeTriggerEvent>.Deregister(_buildingModeTriggerBinding);
        EventBus<RemoveBuildingEvent>.Deregister(_removeBuildingBinding);
    }

    private void Update()
    {
        if (GlobalStatsBridge.Instance.GetShopOpenClosed()) return;
        if (!_canBuilding && !_isRelocating) return;

        if (Input.GetKeyDown(_buildingModeKeyCode) && !_isRelocating)
        {
            BuildingProcessTrigger();
        }

        if (_isBuilding)
        {
            HandleBuildingInput();
        }
        else if (_isRelocating)
        {
            HandleRelocationInput();
        }
    }

    #endregion


    #region Building Logic

    private void GetBuildingColliders()
    {
        _buildedObjectColliders.Clear();

        BuildedObject[] buildedObjects = GameObject.FindObjectsByType<BuildedObject>();
        foreach (BuildedObject obj in buildedObjects)
        {
            _buildedObjectColliders.Add(obj);
        }
    }

    private void HandleBuildingInput()
    {
        if (Input.GetKeyDown(_rotationKeyCode)) RotateObject();
        if (Input.GetKeyDown(_buildKeyCode)) Build();

        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0 || _isFirstPlacementAttempt || _targetObject == null)
        {
            if (TryGetPlacementPosition(out Vector3 targetPosition))
            {
                UpdateObjectPosition(targetPosition);
                _isFirstPlacementAttempt = false;
            }
        }
    }

    private void ResetData()
    {
        _isBuilding = false;
        _targetObjectPrefab = null;
        _targetObject = null;
        _buildingObject = null;
        _buildedObject = null;
        _targetObjectCollider = null;
        _targetObjectTransform = null;
        _isFirstPlacementAttempt = true;
    }

    private void BuildingProcessTrigger()
    {
        ItemObject itemObj = _inventory.GetCurrentItemManager();
        if (itemObj == null || itemObj.GetObjectType() != InteractableObjectTypeEnum.UnbuildedObj)
        {
            if (_isBuilding) TurnOffBuildMode();
            return;
        }

        _isBuilding = !_isBuilding;

        if (_isBuilding)
        {
            _buildingObject = itemObj.GetComponent<BuildingObject>();
            _targetObjectPrefab = _buildingObject.GetPrefab();
            if (_targetObjectPrefab == null)
            {
                Debug.LogError("Prefab for building object not found! | BuildingManager");
                _isBuilding = false;
                return;
            }
            TurnOnBuildMode();
        }
        else
        {
            TurnOffBuildMode();
        }
    }

    private void TurnOnBuildMode()
    {
        _inventory.LockInteractions();
        _inventory.LockChangingSlot();
        _inventory.TurnOffVisual();

        _targetObject = Instantiate(_targetObjectPrefab, _targetFolder);
        _targetObjectTransform = _targetObject.transform;
        _buildedObject = _targetObject.GetComponent<BuildedObject>();

        if (_targetObject.TryGetComponent(out _targetObjectCollider))
            _targetObjectCollider.enabled = false;
        _targetObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        _targetObject.SetActive(true);
        _isFirstPlacementAttempt = true;

        _uiBuildingMode.SetUI(1, _buildingObject.GetAmount());
        _buildedObject.EnableBuilding();

        SetStateBuildingBarrierObjects(true);
    }

    private void TurnOffBuildMode()
    {
        if (_targetObject != null) Destroy(_targetObject);

        if (_buildingObject != null && _buildingObject.GetAmount() == 0)
            _uiBuildingMode.TurnOffUI();
        else
            _uiBuildingMode.SetUI(0, -1);

        if (_buildedObject != null)
            _buildedObject.DisableBuilding();

        ResetData();
        _inventory.TurnOnVisual();
        _inventory.UnlockChangingSlot();
        _inventory.UnlockInteractions();

        SetStateBuildingBarrierObjects(false);
    }

    private void Build()
    {
        if (_targetObject == null || _buildedObject.CheckPlace() == false) return;

        _buildingObject.DecreaseAmount();

        if (_targetObjectCollider != null) _targetObjectCollider.enabled = true;
        _targetObject.layer = 0;
        _targetObject = null;

        _buildedObject.Initialize();
        _buildedObjectColliders.Add(_buildedObject);

        if (_buildingObject.GetAmount() == 0) _inventory.DestroySlot();

        _buildedObject.SetBuildedState();
        TurnOffBuildMode();
    }

    private void RemoveBuilding(RemoveBuildingEvent eventData)
    {
        for (int i = _buildedObjectColliders.Count - 1; i >= 0; i--)
        {
            if (_buildedObjectColliders[i].GetMe() == eventData.Target)
            {
                BuildedObject objToRemove = _buildedObjectColliders[i];
                _buildedObjectColliders.RemoveAt(i);
                Destroy(objToRemove.gameObject);
                break;
            }
        }
    }

    private bool TryGetPlacementPosition(out Vector3 position)
    {
        position = Vector3.zero;
        if (_raycastStartPoint == null) return false;

        if (Physics.Raycast(_raycastStartPoint.position, _raycastStartPoint.forward, out _rayHit, _raycastDistance, _raycastLayerMask))
        {
            if (_rayHit.collider.CompareTag("floor"))
            {
                float snappedX = Mathf.Round(_rayHit.point.x / _snapGridSize) * _snapGridSize;
                float snappedZ = Mathf.Round(_rayHit.point.z / _snapGridSize) * _snapGridSize;
                Vector3 snappedPosition = new Vector3(snappedX, _rayHit.point.y, snappedZ);

                if (_isFirstPlacementAttempt || Vector3.Distance(_lastCalculatedPosition, snappedPosition) > 0.001f)
                {
                    _lastCalculatedPosition = snappedPosition;
                    position = snappedPosition;
                    return true;
                }
            }
        }
        return false;
    }

    private void SetStateBuildingBarrierObjects(bool state)
    {
        if (_buildedObjectColliders != null && _buildedObjectColliders.Count > 0)
        {
            foreach (BuildedObject obj in _buildedObjectColliders)
            {
                if (state) obj.SetActive();
                else obj.SetInactive();
            }
        }
    }

    private void UpdateObjectPosition(Vector3 basePos)
    {
        if (_targetObjectTransform == null || _targetObjectCollider == null) return;

        float halfHeight = (_targetObjectCollider.size.y * _targetObjectTransform.localScale.y) * 0.5f;
        float yPos = halfHeight - (_targetObjectCollider.center.y * _targetObjectTransform.localScale.y);
        _targetObjectTransform.position = new Vector3(basePos.x, yPos, basePos.z);
    }

    private void RotateObject()
    {
        if (_targetObjectTransform != null) _targetObjectTransform.Rotate(0, 90, 0);
    }

    #endregion


    #region Relocation Logic

    private void HandleRelocationInput()
    {
        if (Input.GetKeyDown(_rotationKeyCode))
        {
            _relocatingBuildedObject.transform.Rotate(0, 90, 0);
        }

        if (Input.GetKeyDown(_buildKeyCode))
        {
            ConfirmRelocation();
            return;
        }
        else if (Input.GetKeyDown(_cancelRelocationKeyCode))
        {
            CancelRelocation();
            return;
        }

        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0 || _isFirstPlacementAttempt)
        {
            if (TryGetPlacementPosition(out Vector3 targetPosition))
            {
                UpdateRelocatingObjectPosition(targetPosition);
                _isFirstPlacementAttempt = false;
            }
        }
    }

    private void UpdateRelocatingObjectPosition(Vector3 basePos)
    {
        if (_relocatingBuildedObject == null) return;

        BoxCollider col = _relocatingBuildedObject.GetComponent<BoxCollider>();
        if (col == null) return;

        float halfHeight = (col.size.y * _relocatingBuildedObject.transform.localScale.y) * 0.5f;
        float yPos = halfHeight - (col.center.y * _relocatingBuildedObject.transform.localScale.y);
        _relocatingBuildedObject.transform.position = new Vector3(basePos.x, yPos, basePos.z);
    }

    private void ConfirmRelocation()
    {
        if (_relocatingBuildedObject == null) return;

        _relocatingBuildedObject.SetBuildedState();
        _relocatingBuildedObject.GetComponent<Collider>().enabled = true;
        _relocatingBuildedObject.transform.SetParent(_relocationOriginalParent);

        FinishRelocation();
    }

    private void CancelRelocation()
    {
        if (_relocatingBuildedObject == null) return;

        _relocatingBuildedObject.transform.SetPositionAndRotation(_relocationOriginalPosition, _relocationOriginalRotation);
        _relocatingBuildedObject.transform.SetParent(_relocationOriginalParent);
        _relocatingBuildedObject.SetBuildedState();
        _relocatingBuildedObject.GetComponent<Collider>().enabled = true;

        FinishRelocation();
    }

    private void FinishRelocation()
    {
        _relocatingBuildedObject.DisableBuilding();
        _relocatingBuildedObject = null;
        _isRelocating = false;

        _inventory.UnlockInteractions();
        _inventory.UnlockChangingSlot();

        _uiBuildingMode.TurnOffUI();
        _canBuilding = true;

        _onRelocationFinished?.Invoke();
        _onRelocationFinished = null;
    }

    #endregion


    #region Event Handlers

    private void HandleBuildingModeTrigger(BuildingModeTriggerEvent eventData)
    {
        if (eventData.TargetFolder != null) _canBuilding = true;
        else _canBuilding = false;

        if (!_canBuilding && _isBuilding) TurnOffBuildMode();
    }

    #endregion
}