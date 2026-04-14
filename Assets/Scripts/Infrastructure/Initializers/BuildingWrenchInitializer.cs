using UnityEngine;

public class BuildingWrenchInitializer : MonoBehaviour, IInitializeable
{
    #region Fields

    [Header("Settings")]
    [Tooltip("Folder where objects targeted by wrench will be placed.")]
    [SerializeField] private Transform _targetFolder;

    private EventBinding<BuildingWrenchRequestingEvent> _requestBinding;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        _requestBinding = new EventBinding<BuildingWrenchRequestingEvent>(PushData);
        EventBus<BuildingWrenchRequestingEvent>.Register(_requestBinding);
    }

    #endregion


    #region Private Methods

    private void PushData(BuildingWrenchRequestingEvent eventData)
    {
        EventBus<BuildingWrenchResponsingEvent>.Raise(new BuildingWrenchResponsingEvent { Target = eventData.Target, RaycastFolder = _targetFolder });
    }

    #endregion


    #region Unity Methods

    private void OnDisable()
    {
        EventBus<BuildingWrenchRequestingEvent>.Deregister(_requestBinding);
    }

    #endregion
}