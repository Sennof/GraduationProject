using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class UnpackingPlot : MonoBehaviour, IInitializable
{
    #region Fields

    [Header("References")]
    [Tooltip("Inventory manager.")]
    [SerializeField] private Inventory _inventoryManager;
    [Tooltip("Folder for unpacked objects.")]
    [SerializeField] private Transform _folder;

    private bool _isEmpty = true;
    private Transform _keptObject;
    private EventBinding<UnpackingEvent> _unpackingEventBinding;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        _unpackingEventBinding = new EventBinding<UnpackingEvent>(HandleUnpackingEvent);
        EventBus<UnpackingEvent>.Register(_unpackingEventBinding);
    }

    #endregion


    #region Event Handlers

    private void HandleUnpackingEvent(UnpackingEvent eventData)
    {
        if (_isEmpty == true)
        {
            return;
        }
        if (eventData.Distance > _keptObject.GetComponent<Interactable>().GetActingDistance())
        {
            return;
        }

        _keptObject.GetComponent<PackedObject>().UnpackObject();
        ResetData();
    }

    #endregion


    #region Private Methods

    private void ResetData()
    {
        _keptObject = null;
        _isEmpty = true;
    }

    #endregion


    #region Unity Methods

    private void OnTriggerEnter(Collider other)
    {
        if (_isEmpty == false)
        {
            return;
        }
        if (other.CompareTag("packedBox") == false)
        {
            return;
        }
        if (other.GetComponent<ItemObject>().GetObjectType() != InteractableObjectTypeEnum.PackedBox)
        {
            return;
        }

        _isEmpty = false;
        _keptObject = other.transform;

        _inventoryManager.DropObj();

        other.GetComponent<Rigidbody>().isKinematic = true;
        other.transform.SetParent(_folder);
        other.transform.rotation = Quaternion.identity;
        other.transform.localPosition = new Vector3(0, other.transform.lossyScale.y, 0);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == _keptObject)
        {
            ResetData();
        }
    }

    private void OnDisable()
    {
        EventBus<UnpackingEvent>.Deregister(_unpackingEventBinding);
    }

    #endregion
}