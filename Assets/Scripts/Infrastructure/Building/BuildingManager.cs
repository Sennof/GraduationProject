using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour, IInitializeable
{
    #region Serialized Fields
    [Header("Settings")]
    [SerializeField] private bool _canBuilding = false;
    [SerializeField] private float _snapGridSize = 0.25f;
    [SerializeField] private float _raycastDistance = 6f;
    [SerializeField] private LayerMask _raycastLayerMask;

    [Header("References")]
    [SerializeField] private UIBuildingMode _uiBuildingMode;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private Transform _raycastStartPoint;
    [SerializeField] private Transform _targetFolder;

    [Header("Controls")]
    [SerializeField] private KeyCode _buildingModeKeyCode = KeyCode.B;
    [SerializeField] private KeyCode _rotationKeyCode = KeyCode.R;
    [SerializeField] private KeyCode _buildKeyCode = KeyCode.Mouse0;
    #endregion

    #region Private Variables
    private bool _isBuilding = false;
    private GameObject _targetObjectPrefab;
    private GameObject _targetObject;
    private BoxCollider _targetObjectCollider;
    private Transform _targetObjectTransform;
    private BuildingObject _buildingObject;
    private BuildedObject _buildedObject;

    private List<BuildedObject> _buildedObjectBuilderColliders = new();

    private RaycastHit _rayHit;
    private EventBinding<BuildingModeTriggerEvent> _buildingModeTriggerBinding;
    private EventBinding<RemoveBuildingEvent> _removeBuildingBinding;

    private Vector3 _lastCalculatedPosition;
    private bool _isFirstPlacementAttempt = true;
    #endregion

    #region Core
    public void Initialize()
    {
        ResetData();

        _buildingModeTriggerBinding = new EventBinding<BuildingModeTriggerEvent>(HandleBuildingModeTrigger);
        EventBus<BuildingModeTriggerEvent>.Register(_buildingModeTriggerBinding);

        _removeBuildingBinding = new EventBinding<RemoveBuildingEvent>(RemoveBuilding);
        EventBus<RemoveBuildingEvent>.Register(_removeBuildingBinding);

        GetBuildingColliders();
        _uiBuildingMode.Initialize(_buildingModeKeyCode, _rotationKeyCode, _buildKeyCode);
    }

    private void OnDisable()
    {
        if (_isBuilding) TurnOffBuildMode();
        EventBus<BuildingModeTriggerEvent>.Deregister(_buildingModeTriggerBinding);
        EventBus<RemoveBuildingEvent>.Deregister(_removeBuildingBinding);
    }

    private void Update()
    {
        if (GlobalStatsBridge.Instance.GetShopOpenClosed()) return;
        if (!_canBuilding) return;

        if (Input.GetKeyDown(_buildingModeKeyCode))
            BuildingProcessTrigger();

        if (_isBuilding)
            HandleBuildingInput();
    }

    //part of initialization
    private void GetBuildingColliders()
    {
        _buildedObjectBuilderColliders.Clear();

        BuildedObject[] buildedObjs = GameObject.FindObjectsByType<BuildedObject>(0);
        foreach(BuildedObject obj in buildedObjs)
        {
            _buildedObjectBuilderColliders.Add(obj);
        }
    }

    private void HandleBuildingInput()
    {
        if (Input.GetKeyDown(_rotationKeyCode))
            RotateObject();
        if (Input.GetKeyDown(_buildKeyCode))
            Build();

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
    #endregion

    #region Building Logic
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
        {
            _targetObjectCollider.enabled = false;
        }
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

        if (_buildingObject.GetAmount() == 0)
            _uiBuildingMode.TurnOffUI();
        else
            _uiBuildingMode.SetUI(0, -1);

        _buildedObject.DisableBuilding();

        ResetData();
        _inventory.TurnOnVisual();
        _inventory.UnlockChangingSlot();
        _inventory.UnlockInteractions();

        SetStateBuildingBarrierObjects(false);
    }

    private void Build()
    {
        if (_targetObject == null) return;

        if (_buildedObject.CheckPlace() == false) return;

        _buildingObject.DecreaseAmount();

        if (_targetObjectCollider != null) _targetObjectCollider.enabled = true;
        _targetObject.layer = 0;
        _targetObject = null;

        _buildedObject.Initialize();
        _buildedObjectBuilderColliders.Add(_buildedObject);

        if (_buildingObject.GetAmount() == 0)
            _inventory.DestroySlot();

        _buildedObject.SetBuildedState();
        TurnOffBuildMode();
    }

    private void RemoveBuilding(RemoveBuildingEvent eventData)
    {
        for (int i = _buildedObjectBuilderColliders.Count - 1; i >= 0; i--)
        {
            if (_buildedObjectBuilderColliders[i].GetMe() == eventData.Target)
            {
                BuildedObject objToRemove = _buildedObjectBuilderColliders[i];
                _buildedObjectBuilderColliders.RemoveAt(i);
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

                if (_isFirstPlacementAttempt || Vector3.Distance(_lastCalculatedPosition, snappedPosition) > 0.001f) // ֿמנמד ג 1 לל
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
        if(_buildedObjectBuilderColliders != null && _buildedObjectBuilderColliders.Count > 0)
        {
            foreach(BuildedObject obj in _buildedObjectBuilderColliders)
            {
                if (state)
                    obj.SetActive();
                else
                    obj.SetInactive();
            }
        }
    }

    private void UpdateObjectPosition(Vector3 basePos)
    {
        if (_targetObjectTransform == null || _targetObjectCollider == null) return;

        float halfHeight = (_targetObjectCollider.size.y * _targetObjectTransform.localScale.y) * 0.5f; ; 
        float yPos = halfHeight - (_targetObjectCollider.center.y * _targetObjectTransform.localScale.y);

        _targetObjectTransform.position = new Vector3(basePos.x, yPos, basePos.z);
    }

    private void RotateObject()
    {
        if (_targetObjectTransform != null)
            _targetObjectTransform.Rotate(0, 90, 0);
    }
    #endregion

    #region Event handlers
    private void HandleBuildingModeTrigger(BuildingModeTriggerEvent eventData)
    {
        //earlier there was a logic with pushing some parent data through event though now it is forbidden
        if (eventData.TargetFolder != null)
            _canBuilding = true;
        else
            _canBuilding = false;

        if (!_canBuilding && _isBuilding) TurnOffBuildMode();
    }
    #endregion
}
