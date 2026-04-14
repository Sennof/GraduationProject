using UnityEngine;

[RequireComponent(typeof(InteractingObject))]
public class BuildingWrench : MonoBehaviour, IInitializeable
{
    #region Fields

    [Header("Dependencies")]
    [Tooltip("InteractingObject component for raycasting.")]
    [SerializeField] private InteractingObject _interactingObject;

    private EventBinding<BuildingWrenchResponsingEvent> _responseBinding;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        _responseBinding = new EventBinding<BuildingWrenchResponsingEvent>(GetData);
        EventBus<BuildingWrenchResponsingEvent>.Register(_responseBinding);

        EventBus<BuildingWrenchRequestingEvent>.Raise(new BuildingWrenchRequestingEvent { Target = gameObject });
    }

    public void InvokeRemoving()
    {
        if (_interactingObject.GetActDistance() >= _interactingObject.GetDistanceToTarget())
        {
            GameObject hittedObject = _interactingObject.GetTargetObject();
            if (hittedObject != null)
            {
                EventBus<RemoveBuildingEvent>.Raise(new RemoveBuildingEvent { Target = hittedObject });
            }
        }
    }

    #endregion


    #region Private Methods

    private void GetData(BuildingWrenchResponsingEvent eventData)
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
        EventBus<BuildingWrenchResponsingEvent>.Deregister(_responseBinding);
        _responseBinding = null;
    }

    #endregion
}