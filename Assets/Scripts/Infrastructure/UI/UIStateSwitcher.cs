using UnityEngine;
using UnityEngine.Events;

public class UIStateSwitcher : MonoBehaviour
{
    [SerializeField] private UnityEvent _defaultActivatingEvents;
    [SerializeField] private UnityEvent _defaultDisctivatingEvents;

    public void InvokeActivatingEvents() => _defaultActivatingEvents?.Invoke();

    public void InvokeDisactivatingEvents() => _defaultDisctivatingEvents?.Invoke();
}
