using UnityEngine;

[RequireComponent (typeof(InteractingObject))]
public class PenKnife : MonoBehaviour
{
    [SerializeField] private InteractingObject _interactingObject;
    public void InvokeUnpacking()
    {
        EventBus<UnpackingEvent>.Raise(new UnpackingEvent {Distance = _interactingObject.GetDistanceToTarget()});
    }
}
