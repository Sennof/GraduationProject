using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private bool _enabled = true;
    [SerializeField] private InteractableObjectTypeEnum _type;
    [SerializeField] private KeyCode _triggerKey = KeyCode.None;
    [SerializeField] private float _actDistance = 2.5f;

    [SerializeField] private UnityEvent _events;

    public void InvokeActions()
    {
        if (_events != null) _events.Invoke();
        else Debug.LogError($"Failed to interact | {gameObject.name}" +
            $"\nEvents null\n");
    }

    public KeyCode GetTriggerKey() => _triggerKey;

    public float GetActingDistance() => _actDistance;
    
    public bool GetActiveState() => _enabled;

    public InteractableObjectTypeEnum GetObjectType() => _type;
}
