using UnityEngine;

public class PriceTagMakerInitializer : MonoBehaviour, IInitializeable
{
    #region Fields

    [Header("Dependencies")]
    [Tooltip("Camera/raycast origin provided to PriceTagMaker instances.")]
    [SerializeField] private Transform _raycastStartPoint;
    [Tooltip("Player inventory provided to PriceTagMaker instances.")]
    [SerializeField] private Inventory _inventory;

    private EventBinding<PriceTagMakerDataRequestingEvent> _requestBinding;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        _requestBinding = new EventBinding<PriceTagMakerDataRequestingEvent>(PushData);
        EventBus<PriceTagMakerDataRequestingEvent>.Register(_requestBinding);
    }

    #endregion


    #region Private Methods

    private void PushData(PriceTagMakerDataRequestingEvent eventData)
    {
        EventBus<PriceTagMakerDataResponsingEvent>.Raise(new PriceTagMakerDataResponsingEvent
        {
            Target = eventData.Target,
            RaycastStartPoint = _raycastStartPoint,
            Inventory = _inventory
        });
    }

    #endregion


    #region Unity Methods

    private void OnDisable()
    {
        EventBus<PriceTagMakerDataRequestingEvent>.Deregister(_requestBinding);
    }

    #endregion
}
