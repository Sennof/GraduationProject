using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class HomoObjectSwitcher : MonoBehaviour, IInitializable
{
    #region Fields

    [Header("Main Core")]
    [Tooltip("List of container objects to switch between.")]
    [SerializeField] private List<HomoObjectContainer> _homoObjects;
    [Tooltip("Current active container index.")]
    [SerializeField] private int _currentIndex = 0;

    [Header("Initialization")]
    [Tooltip("Turn off all containers on awake.")]
    [SerializeField] private bool _turnOffAwake = true;
    [Tooltip("Turn on specific container on awake.")]
    [SerializeField] private bool _turnOnAwake = true;
    [Tooltip("ID of the container to activate on awake.")]
    [SerializeField] private int _onAwakeObjId = 0;

    [Header("Input")]
    [Tooltip("Key to toggle the active state.")]
    [SerializeField] private KeyCode _triggerKey = KeyCode.None;

    [Header("Side Actions")]
    [Tooltip("Actions invoked when switching on.")]
    [SerializeField] private UnityEvent _actionsOnActivating;
    [Tooltip("Actions invoked when switching off.")]
    [SerializeField] private UnityEvent _actionsOnDisactivating;

    [Header("If UI")]
    [Tooltip("Is this switcher used as UI.")]
    [SerializeField] private bool _isUI = false;
    [Tooltip("Reference to UI checking component.")]
    [SerializeField] private UIChecking _uiChecking;

    private bool _isActive = false;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        if (_homoObjects == null)
        {
            Debug.LogWarning($"Objects folder is empty. May cause issues. | HomoObjectSwitcher | {gameObject.name}");
            return;
        }

        foreach (HomoObjectContainer homoObj in _homoObjects)
        {
            homoObj.Initialize();
        }

        if (_turnOffAwake)
        {
            OffAll();
        }
        if (_turnOnAwake)
        {
            SetOn(_onAwakeObjId);
        }
    }

    public void SetOnInWorld(int id)
    {
        if (_isUI && _uiChecking.GetState())
        {
            return;
        }

        SetOn(_onAwakeObjId);
        _actionsOnActivating?.Invoke();
    }

    public void InvokeActivatingActions() => _actionsOnActivating?.Invoke();

    public void InvokeDisactivatingActions() => _actionsOnDisactivating?.Invoke();

    public void OffAll()
    {
        foreach (HomoObjectContainer homoObj in _homoObjects)
        {
            homoObj.TurnOff();
        }

        _currentIndex = 0;
        _isActive = false;
    }

    public void OffByIndex(int id)
    {
        _homoObjects[id].TurnOff();
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
        _homoObjects[id].InvokeActionsOnEnable();
        _currentIndex = id;
        _isActive = true;
    }

    #endregion


    #region Unity Methods

    private void Update()
    {
        if (Input.GetKeyDown(_triggerKey))
        {
            if (!_isActive)
            {
                if (_isUI && _uiChecking.GetState())
                {
                    return;
                }

                SetOn(_onAwakeObjId);
                _actionsOnActivating?.Invoke();
            }
            else
            {
                OffAll();
                _actionsOnDisactivating?.Invoke();
            }
        }
    }

    #endregion
}