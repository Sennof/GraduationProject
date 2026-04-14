using UnityEngine;
using UnityEngine.Events;

public class UIStateSwitcher : MonoBehaviour
{
    #region Fields

    [Header("Events")]
    [Tooltip("Actions invoked when activating.")]
    [SerializeField] private UnityEvent _defaultActivatingEvents;
    [Tooltip("Actions invoked when deactivating.")]
    [SerializeField] private UnityEvent _defaultDisctivatingEvents;

    #endregion


    #region Public Methods

    public void InvokeActivatingEvents() => _defaultActivatingEvents?.Invoke();

    public void InvokeDisactivatingEvents() => _defaultDisctivatingEvents?.Invoke();

    #endregion
}