using UnityEngine;

[RequireComponent (typeof(InteractingObject))]
public class PenKnife : MonoBehaviour, IInitializeable
{
    [SerializeField] private InteractingObject _interactingObject;
    private EventBinding<PenKnifeResponsingEvent> _binding;

    public void Initialize()
    {
        _binding = new EventBinding<PenKnifeResponsingEvent>(GetData);
        EventBus<PenKnifeResponsingEvent>.Register(_binding);

        EventBus<PenKnifeDataRequestingEvent>.Raise(new PenKnifeDataRequestingEvent { Target = gameObject});
    }


    public void InvokeUnpacking()
    {
        EventBus<UnpackingEvent>.Raise(new UnpackingEvent {Distance = _interactingObject.GetDistanceToTarget()});
    }

    private void GetData(PenKnifeResponsingEvent eventData)
    {
        if (eventData.Target != gameObject) return;

        _interactingObject.SetTargetObjectFolder(eventData.RaycastFolder);
    }
}
