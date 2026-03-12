
using UnityEngine;

public class PenKnivesInitializer : MonoBehaviour, IInitializeable
{
    [SerializeField] private Transform _targetFolder;
    private EventBinding<PenKnifeDataRequestingEvent> _binding;

    public void Initialize()
    {
        _binding = new EventBinding<PenKnifeDataRequestingEvent>(PushData);
        EventBus<PenKnifeDataRequestingEvent>.Register(_binding);
    }

    private void OnDisable()
    {
        EventBus<PenKnifeDataRequestingEvent>.Deregister(_binding);
    }

    private void PushData(PenKnifeDataRequestingEvent eventData)
    {
        EventBus<PenKnifeResponsingEvent>.Raise(new PenKnifeResponsingEvent { Target = eventData.Target, RaycastFolder = _targetFolder});
    }
}
