using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class UnpackingPlot : MonoBehaviour, IInitializable
{
    [SerializeField] private Inventory _inventoryManager;
    [SerializeField] private Transform _folder;

    private bool _isEmpty = true;
    private Transform _keptObject;
    private EventBinding<UnpackingEvent> _eventBinding;

    public void Initialize()
    {
        _eventBinding = new EventBinding<UnpackingEvent>(HandleUnpackingEvent);
        EventBus<UnpackingEvent>.Register(_eventBinding); 
    }

    private void OnDisable()
    {
        EventBus<UnpackingEvent>.Deregister(_eventBinding);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isEmpty == false) 
            return;
        if (other.GetComponent<Interactable>().GetObjectType() != InteractableObjectTypeEnum.PackedBox)
            return;

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
        if(other.transform == _keptObject)
        {
            ResetData();   
        }
    }

    private void HandleUnpackingEvent(UnpackingEvent eventData)
    {
        if (_isEmpty == true) 
            return;
        if (eventData.Distance > _keptObject.GetComponent<Interactable>().GetActingDistance())
            return;

        _keptObject.GetComponent<PackedObject>().UnpackObject();
        ResetData();
    }

    private void ResetData()
    {
        _keptObject = null;
        _isEmpty = true;
    }
}
