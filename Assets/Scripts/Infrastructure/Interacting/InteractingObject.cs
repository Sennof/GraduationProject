using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class InteractingObject : MonoBehaviour
{
    //parameter to control time of work;
    [SerializeField] private bool _enabled = true;
    [SerializeField] private bool _inHands = false;

    [SerializeField] private bool _needRaycast = true;
    [SerializeField] private Transform _raycastStartPoint;

    [SerializeField] private UnityEvent _actions;
    [SerializeField] private Transform _targetObjectFolder;

    [SerializeField] private KeyCode _triggerKey = KeyCode.F;

    public void Initialize(Transform raycastStartPoint, Transform targetObj)
    {
        _raycastStartPoint = raycastStartPoint;
        _targetObjectFolder = targetObj;
    }

    private void Update()
    {
        if (_enabled == false || _inHands == false) return;

        if (Input.GetKeyDown(_triggerKey))
        {
            if (_needRaycast == false)
            {
                _actions.Invoke();
                return;
            }

            Transform targetObj = null;
            try
            {
                targetObj = _targetObjectFolder.GetChild(0);
            }
            catch
            {
                Debug.Log($"There is no object in the plot | InteractingObject: {gameObject.name}");
                return;
            }

            if (ThrowRay() == targetObj) // getting child of the container object
            {
                _actions.Invoke();
                return;
            }
        }
    }

    public Transform GetTargetObject() => _targetObjectFolder;

    public float GetDistanceToTarget() => Vector3.Distance(transform.position, _targetObjectFolder.position);

    public void SetInHands() => _inHands = true;

    public void SetOutHands() => _inHands = false;

    private Transform ThrowRay()
    {
        RaycastHit rayHit;

        if(Physics.Raycast(_raycastStartPoint.position, _raycastStartPoint.TransformDirection(Vector3.forward), out rayHit, 5))
        {
            return rayHit.transform;
        }

        return null;
    }

}
