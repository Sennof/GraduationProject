using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour, IInitializable
{
    #region Valuables
    [Header("UI")]
    [Tooltip("View ñontroller")]
    [SerializeField] private InventoryUI _uiController;

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

    private EventBinding<ItemPickUpEvent> _itemPickUpEventBinding;

    private GameObject[] _keptItemGameObjects = new GameObject[2];
    private ItemObject[] _keptItemObjects = new ItemObject[2];

    private int _currentItemSlotIndex = 0;
    private bool _enabled = true;

    private int _targetSlot = 0; //value to change current slot via keypad
    #endregion                   //if it changes current slot changes

    // Rewrite to ExitPoint (as entryPoint)
    private void OnDisable()
    {
        EventBus<ItemPickUpEvent>.Deregister(_itemPickUpEventBinding);
    }

    private void Update()
    {
        if (!_enabled)
            return;

        if (Input.GetKeyDown(_throwTriggerey) && _keptItemObjects[_currentItemSlotIndex] != null)
            ThrowObj();

        if (Input.GetKeyDown(_dropTriggerey) && _keptItemObjects[_currentItemSlotIndex] != null)
            DropObj();

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            ChangeSlot();
        }

        if (Input.GetKeyDown(_firstSlotKeyCode)) _targetSlot = 0;
        else if (Input.GetKeyDown(_secondSlotKeyCode)) _targetSlot = 1;

        if (_currentItemSlotIndex != _targetSlot) ChangeSlot();
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

    private void ChangeSlot()
    {
        if (_keptItemGameObjects[_currentItemSlotIndex] != null)
            _keptItemGameObjects[_currentItemSlotIndex].SetActive(false);

        _currentItemSlotIndex = 1 - _currentItemSlotIndex;
        _targetSlot = _currentItemSlotIndex;

        if (_keptItemGameObjects[_currentItemSlotIndex] != null)
            _keptItemGameObjects[_currentItemSlotIndex].SetActive(true);

        _uiController.SelectSlot(_currentItemSlotIndex);
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
        }
    }

    public void DropObj()
    {
        _keptItemGameObjects[_currentItemSlotIndex].transform.SetParent(_keptItemObjects[_currentItemSlotIndex].GetDefaultParent()
            );
        _keptItemObjects[_currentItemSlotIndex].Drop();

        _keptItemGameObjects[_currentItemSlotIndex] = null;
        _keptItemObjects[_currentItemSlotIndex] = null;

        _uiController.ClearIcon(_currentItemSlotIndex);
    }

    public void HandlePickUp(ItemPickUpEvent eventData)
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

        _keptItemGameObjects[slotIndex] = eventData.ItemObjectData.gameObject;
        _keptItemObjects[slotIndex] = eventData.ItemObjectData;

        _keptItemGameObjects[slotIndex].transform.SetParent(_handsFolder);
        _keptItemObjects[slotIndex].PickUp();

        if (isLocalSlotDifferent)
            _keptItemGameObjects[slotIndex].SetActive(false);

        _uiController.SetIcon(_keptItemObjects[slotIndex].GetIcon(), slotIndex);
    }
}
