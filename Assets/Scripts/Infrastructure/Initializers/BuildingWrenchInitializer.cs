using UnityEngine;

public class BuildingWrenchInitializer : MonoBehaviour, IInitializeable
{
    [SerializeField] private Transform _targetFolder;
    private EventBinding<BuildingWrenchRequestingEvent> _binding;

    public void Initialize()
    {
        _binding = new EventBinding<BuildingWrenchRequestingEvent>(PushData);
        EventBus<BuildingWrenchRequestingEvent>.Register(_binding);
    }

    private void PushData(BuildingWrenchRequestingEvent eventData)
    {
        EventBus<BuildingWrenchResponsingEvent>.Raise(new BuildingWrenchResponsingEvent { Target = eventData.Target, RaycastFolder = _targetFolder });
    }

    private void OnDisable()
    {
        EventBus<BuildingWrenchRequestingEvent>.Deregister(_binding);
    }
}
