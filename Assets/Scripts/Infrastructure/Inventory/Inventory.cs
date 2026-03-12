using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour, IInitializable
{
    #region Valuables
    [Header("UI")]
    [Tooltip("View ñontroller")]
    [SerializeField] private InventoryUI _uiController;
    [Tooltip("Building Mode Hints Manager")]
    [SerializeField] private UIBuildingMode _uiBuildingMode;

    [Header("Folders")]
    [Tooltip("The character's hands, into which the object will be \"taken\"")]
    [SerializeField] private Transform _handsFolder;

    [Header("Interaction")]
    [Tooltip("The key for throwing away an object")]
    [SerializeField] private KeyCode _throwTriggerey = KeyCode.Mouse0;
    [Tooltip("The key for droping an object")]
    [SerializeField] private KeyCode _dropTriggerey = KeyCode.Mouse1;

    [Header("Control KeyCodes")]
    [Tooltip("The key to set active first slot")]
    [SerializeField] private KeyCode _firstSlotKeyCode = KeyCode.Alpha1;
    [Tooltip("The key to set active second slot")]
    [SerializeField] private KeyCode _secondSlotKeyCode = KeyCode.Alpha2;

    [Header("States")]
    [Tooltip("Can a player change the active slot")]
    [SerializeField] private bool _canChangeSlot = true;
    [Tooltip("Can a player pick up/throw/drop objects")]
    [SerializeField] private bool _canInteract = true;

    private EventBinding<ItemPickUpEvent> _itemPickUpEventBinding;

    private GameObject[] _keptItemGameObjects = new GameObject[2];
    private ItemObject[] _keptItemObjects = new ItemObject[2];

    private int _currentItemSlotIndex = 0;
    private bool _enabled = true;

    private int _targetSlot = 0; //value to change current slot via keypad
    #endregion                   //if it changes current slot changes

    #region Core
    // Rewrite to ExitPoint (as entryPoint)
    private void OnDisable()
    {
        EventBus<ItemPickUpEvent>.Deregister(_itemPickUpEventBinding);
    }

    private void Update()
    {
        if (!_enabled)
            return;

        if(_canInteract)
        {
            if (Input.GetKeyDown(_throwTriggerey))
                ThrowObj();

            if (Input.GetKeyDown(_dropTriggerey))
                DropObj();
        }

        if (_canChangeSlot)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                ChangeSlot();
            }

            if (Input.GetKeyDown(_firstSlotKeyCode)) _targetSlot = 0;
            else if (Input.GetKeyDown(_secondSlotKeyCode)) _targetSlot = 1;

            if (_currentItemSlotIndex != _targetSlot) ChangeSlot();
        }
    }

    public void Initialize()
    {
        if(_handsFolder == null)
        {
            Debug.LogError("Some data is missing | Inventory");
            return;
        }

        _itemPickUpEventBinding = new EventBinding<ItemPickUpEvent>(HandlePickUp);
        EventBus<ItemPickUpEvent>.Register(_itemPickUpEventBinding);
    }
    #endregion

    #region Main
    private void ChangeSlot()
    {
        if (_keptItemGameObjects[_currentItemSlotIndex] != null)
            _keptItemGameObjects[_currentItemSlotIndex].SetActive(false);

        _currentItemSlotIndex = 1 - _currentItemSlotIndex;
        _targetSlot = _currentItemSlotIndex;

        if (_keptItemGameObjects[_currentItemSlotIndex] != null)
            _keptItemGameObjects[_currentItemSlotIndex].SetActive(true);

        _uiController.SelectSlot(_currentItemSlotIndex);
        CheckForBuildingMode(_currentItemSlotIndex);
    }

    public void ThrowObj()
    {
        if (_keptItemObjects[_currentItemSlotIndex] != null)
        {
            _keptItemGameObjects[_currentItemSlotIndex].transform.SetParent(_keptItemObjects[_currentItemSlotIndex].GetDefaultParent());
            _keptItemObjects[_currentItemSlotIndex].Throw();

            _keptItemGameObjects[_currentItemSlotIndex] = null;
            _keptItemObjects[_currentItemSlotIndex] = null;

            _uiController.ClearIcon(_currentItemSlotIndex);
            CheckForBuildingMode(_currentItemSlotIndex);
        }
    }

    public bool CanPickUpMore()
    {
        if (_keptItemGameObjects[0] == null || _keptItemGameObjects[1] == null) return true;
        else return false;
    }

    public void DropObj()
    {
        if (_keptItemObjects[_currentItemSlotIndex] != null)
        {
            _keptItemGameObjects[_currentItemSlotIndex].transform.SetParent(_keptItemObjects[_currentItemSlotIndex].GetDefaultParent()
                );
            _keptItemObjects[_currentItemSlotIndex].Drop();

            _keptItemGameObjects[_currentItemSlotIndex] = null;
            _keptItemObjects[_currentItemSlotIndex] = null;

            _uiController.ClearIcon(_currentItemSlotIndex);
            CheckForBuildingMode(_currentItemSlotIndex);
        }
    }

    public void PickUp(ItemObject itemObject)
    {
        if (!enabled)
            return;

        int slotIndex = _currentItemSlotIndex;
        bool isLocalSlotDifferent = false;

        if (_keptItemGameObjects[_currentItemSlotIndex] != null)
        {
            slotIndex = 1 - _currentItemSlotIndex;
            isLocalSlotDifferent = true;

            if (_keptItemGameObjects[slotIndex] != null)
            {
                Debug.Log($"Not enough space in inventory | {name}");
                return;
            }
        }

        _keptItemGameObjects[slotIndex] = itemObject.gameObject;
        _keptItemObjects[slotIndex] = itemObject;

        _keptItemGameObjects[slotIndex].transform.SetParent(_handsFolder);
        _keptItemObjects[slotIndex].PickUp();

        if (isLocalSlotDifferent)
            _keptItemGameObjects[slotIndex].SetActive(false);

        _uiController.SetIcon(_keptItemObjects[slotIndex].GetIcon(), slotIndex);

        CheckForBuildingMode(slotIndex);
    }

    public void DestroySlot()
    {
        Destroy(_keptItemGameObjects[_currentItemSlotIndex]);
        _uiController.ClearIcon(_currentItemSlotIndex);
        _keptItemGameObjects[_currentItemSlotIndex] = null;
        _keptItemObjects[_currentItemSlotIndex] = null;

        CheckForBuildingMode(_currentItemSlotIndex);
    }

    private void CheckForBuildingMode(int id)
    {
        if (_keptItemGameObjects[id] != null)
        {
            if (_keptItemGameObjects[id].GetComponent<BuildingObject>() != null)
                _uiBuildingMode.SetUI(0, -1);
            else
                _uiBuildingMode.TurnOffUI();
        }
        else
        {
            _uiBuildingMode.TurnOffUI();
        }
    }
    #endregion

    #region Event handlers
    public void HandlePickUp(ItemPickUpEvent eventData)
    {
        PickUp(eventData.ItemObjectData);
    }
    #endregion

    #region Condition controls
    public void LockChangingSlot() => _canChangeSlot = false;

    public void UnlockChangingSlot() => _canChangeSlot = true;

    public void TurnOffVisual()
    {
        if (_keptItemGameObjects[_currentItemSlotIndex] == null) return;
        _keptItemGameObjects[_currentItemSlotIndex].SetActive(false);
    }

    public void TurnOnVisual()
    {
        if (_keptItemGameObjects[_currentItemSlotIndex] == null) return;
        _keptItemGameObjects[_currentItemSlotIndex].SetActive(true);
    }

    public void LockInteractions() => _canInteract = false;

    public void UnlockInteractions() => _canInteract = true;
    #endregion

    #region Getting from outside
    public bool CheckAvaliableness() => _enabled;

    public GameObject GetCurrentItem() => _keptItemGameObjects[_currentItemSlotIndex];

    public ItemObject GetCurrentItemManager() => _keptItemObjects[_currentItemSlotIndex];
    #endregion
}
