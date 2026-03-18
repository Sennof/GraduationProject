using UnityEngine;

[RequireComponent(typeof(InteractingObject))]
public class BuildingWrench : MonoBehaviour, IInitializeable
{

    [SerializeField] private InteractingObject _interactingObject;
    private EventBinding<BuildingWrenchResponsingEvent> _binding;

    public void Initialize()
    {
        //DataRequesting
        _binding = new EventBinding<BuildingWrenchResponsingEvent>(GetData);
        EventBus<BuildingWrenchResponsingEvent>.Register(_binding);

        EventBus<BuildingWrenchRequestingEvent>.Raise(new BuildingWrenchRequestingEvent { Target = gameObject});
    }

    private void OnDisable()
    {
        EventBus<BuildingWrenchResponsingEvent>.Deregister(_binding);
        _binding = null;
    }

    public void GetData(BuildingWrenchResponsingEvent eventData)
    {
        if (eventData.Target != gameObject) return;
        _interactingObject.SetTargetObjectFolder(eventData.RaycastFolder);
    }

    public void InvokeRemoving()
    {
        if (_interactingObject.GetActDistance() >= _interactingObject.GetDistanceToTarget())
        {
            GameObject hittedObject = _interactingObject.GetTargetObject();
            if (hittedObject != null)
                EventBus<RemoveBuildingEvent>.Raise(new RemoveBuildingEvent { Target = hittedObject });
        }
    }
}
