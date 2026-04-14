using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    #region Fields

    [Header("State")]
    [Tooltip("Enable or disable interaction.")]
    [SerializeField] private bool _enabled = true;

    [Header("Input")]
    [Tooltip("Enable side events with a secondary key.")]
    [SerializeField] private bool _usingSideEvents = false;
    [Tooltip("Primary interaction key.")]
    [SerializeField] private KeyCode _mainTriggerKey = KeyCode.E;
    [Tooltip("Secondary interaction key.")]
    [SerializeField] private KeyCode _sideTriggerKey = KeyCode.F;

    [Header("Range")]
    [Tooltip("Maximum distance to interact.")]
    [SerializeField] private float _actDistance = 2.5f;

    [Header("Events")]
    [Tooltip("Actions invoked on main trigger.")]
    [SerializeField] private UnityEvent _mainEvents;
    [Tooltip("Actions invoked on side trigger.")]
    [SerializeField] private UnityEvent _sideEvents;

    #endregion


    #region Public Methods

    public void InvokeMainActions()
    {
        if (_mainEvents != null)
        {
            _mainEvents.Invoke();
        }
        else
        {
            Debug.LogError($"Failed to interact | {gameObject.name}\nEvents - null\n");
        }

        Debug.Log("MainActions Invoked");
    }

    public void InvokeSideActions()
    {
        if (!_usingSideEvents)
        {
            return;
        }

        if (_sideEvents != null)
        {
            _sideEvents.Invoke();
        }
        else
        {
            Debug.LogError($"Failed to interact | {gameObject.name}\nSide events - null\n");
        }

        Debug.Log("SideActions invoked");
    }

    public bool GetStateUsingSideEvents() => _usingSideEvents;

    public KeyCode GetMainTriggerKey() => _mainTriggerKey;

    public KeyCode GetSideTriggerKey() => _sideTriggerKey;

    public float GetActingDistance() => _actDistance;

    public bool GetActiveState() => _enabled;

    public void SetActiveState(bool state) => _enabled = state;

    #endregion
}