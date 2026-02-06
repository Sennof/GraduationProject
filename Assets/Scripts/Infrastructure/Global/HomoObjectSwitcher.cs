using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HomoObjectSwitcher : MonoBehaviour
{
    [Header("MAIN CORE")]
    [SerializeField] private List<HomoObjectContainer> _homoObjects;
    [SerializeField] private int _currentIndex = 0;

    [SerializeField] private bool _turnOffAwake = true;
    [SerializeField] private bool _turnOnAwake = true;
    [SerializeField] private int _onAwakeObjId = 0;

    [SerializeField] private bool _isActive = false;
    [SerializeField] private KeyCode _triggerKey = KeyCode.Tab;

    [Header("Side Actions (unnessesary)")]
    [SerializeField] private UnityEvent _actionsOnActivating;
    [SerializeField] private UnityEvent _actionsOnDisactivating;

    private void Update()
    {
        if (Input.GetKeyDown(_triggerKey))
        {
            if (!_isActive)
            {
                SetOn(_onAwakeObjId);
                if (_actionsOnActivating != null)
                    _actionsOnActivating.Invoke();
            }
            else
            {
                OffAll();
                if(_actionsOnDisactivating != null)
                    _actionsOnDisactivating.Invoke();
            }
        }
    }

    public void Initialize()
    {
        if(_homoObjects == null)
        {
            Debug.LogWarning($"Objects folder is empty. " +
                $"May cause some issues. " +
                $"| HomoObjectSwitcher " +
                $"| {gameObject.name}");
            
            return;
        }

        foreach(HomoObjectContainer homoObj in _homoObjects)
        {
            homoObj.Initialize();
        }

        if(_turnOffAwake) OffAll();
        if (_turnOnAwake) SetOn(_onAwakeObjId);
    }

    public void InvokeActivatingActions() => _actionsOnActivating?.Invoke();

    public void InvokeDisactivatingActions() => _actionsOnDisactivating?.Invoke();
        
    public void OffAll()
    {
        foreach(HomoObjectContainer homoObj in _homoObjects)
            homoObj.TurnOff();

        _currentIndex = 0;
        _isActive = false;
    }

    public void OffCurrent()
    {
        _homoObjects[_currentIndex].TurnOff();
        _currentIndex = 0;

        _isActive = true;
    }

    public void SetOn(int id)
    {
        _homoObjects[id].TurnOn();
        _currentIndex = id;

        _isActive = true;
    }
}
