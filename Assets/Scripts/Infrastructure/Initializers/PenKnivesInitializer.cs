using UnityEngine;

public class PenKnivesInitializer : MonoBehaviour, IInitializeable
{
    #region Fields

    [Header("Settings")]
    [Tooltip("Folder for raycast target.")]
    [SerializeField] private Transform _targetFolder;

    private EventBinding<PenKnifeDataRequestingEvent> _requestBinding;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        _requestBinding = new EventBinding<PenKnifeDataRequestingEvent>(PushData);
        EventBus<PenKnifeDataRequestingEvent>.Register(_requestBinding);
    }

    #endregion


    #region Private Methods

    private void PushData(PenKnifeDataRequestingEvent eventData)
    {
        EventBus<PenKnifeResponsingEvent>.Raise(new PenKnifeResponsingEvent { Target = eventData.Target, RaycastFolder = _targetFolder });
    }

    #endregion


    #region Unity Methods

    private void OnDisable()
    {
        EventBus<PenKnifeDataRequestingEvent>.Deregister(_requestBinding);
    }

    #endregion
}