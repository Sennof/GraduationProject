using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private bool _enabled = true;
    [SerializeField] private bool _usingSideEvents = false;
    [SerializeField] private KeyCode _mainTriggerKey = KeyCode.E;
    [SerializeField] private KeyCode _sideTriggerKey = KeyCode.F;
    [SerializeField] private float _actDistance = 2.5f;

    [SerializeField] private UnityEvent _mainEvents;
    [SerializeField] private UnityEvent _sideEvents;

    public void InvokeMainActions()
    {
        if (_mainEvents != null) _mainEvents.Invoke();
        else Debug.LogError($"Failed to interact | {gameObject.name}" +
            $"\nEvents - null\n");

        Debug.Log("MainActions Invoked");
    }

    public void InvokeSideActions()
    {
        if (!_usingSideEvents) return;
        if(_sideEvents != null) _sideEvents.Invoke();
        else Debug.LogError($"Failed to interact | {gameObject.name}" +
            $"\nSide events - null\n");

        Debug.Log("SideActions invoked");
    }

    public bool GetStateUsingSideEvents() => _usingSideEvents;

    public KeyCode GetMainTriggerKey() => _mainTriggerKey;

    public KeyCode GetSideTriggerKey() => _sideTriggerKey;

    public float GetActingDistance() => _actDistance;
    
    public bool GetActiveState() => _enabled;
}
