using UnityEngine;

[RequireComponent(typeof(InteractingObject))]
public class PenKnife : MonoBehaviour, IInitializeable
{
    #region Fields

    [Header("Dependencies")]
    [Tooltip("InteractingObject component for raycasting.")]
    [SerializeField] private InteractingObject _interactingObject;

    private EventBinding<PenKnifeResponsingEvent> _responseBinding;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        _responseBinding = new EventBinding<PenKnifeResponsingEvent>(GetData);
        EventBus<PenKnifeResponsingEvent>.Register(_responseBinding);

        EventBus<PenKnifeDataRequestingEvent>.Raise(new PenKnifeDataRequestingEvent { Target = gameObject });
    }

    public void InvokeUnpacking()
    {
        EventBus<UnpackingEvent>.Raise(new UnpackingEvent { Distance = _interactingObject.GetDistanceToTarget() });
    }

    #endregion


    #region Private Methods

    private void GetData(PenKnifeResponsingEvent eventData)
    {
        if (eventData.Target != gameObject)
        {
            return;
        }

        _interactingObject.SetTargetObjectFolder(eventData.RaycastFolder);
    }

    #endregion


    #region Unity Methods

    private void OnDisable()
    {
        EventBus<PenKnifeResponsingEvent>.Deregister(_responseBinding);
        _responseBinding = null;
    }

    #endregion
}