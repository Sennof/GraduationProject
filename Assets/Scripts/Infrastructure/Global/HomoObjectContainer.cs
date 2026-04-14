using UnityEngine;
using UnityEngine.Events;

public class HomoObjectContainer : MonoBehaviour
{
    #region Fields

    [Header("Events")]
    [Tooltip("Actions invoked when the container is enabled.")]
    [SerializeField] private UnityEvent _actionsOnEnable;

    private GameObject _homoObject;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        _homoObject = transform.GetChild(0).gameObject;

        if (_homoObject == null)
        {
            Debug.LogError($"Failed to initialize HomoObject Container | {gameObject.name}");
        }
    }

    public void InvokeActionsOnEnable() => _actionsOnEnable.Invoke();

    public void TurnOff() => _homoObject.SetActive(false);

    public void TurnOn() => _homoObject.SetActive(true);

    #endregion
}