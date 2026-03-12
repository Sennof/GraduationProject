using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class InteractingObject : MonoBehaviour
{
    #region State Settings
    [Header("State Settings")]
    [Tooltip("Global toggle to control the system's operation time")]
    [SerializeField] private bool _enabled = true;

    [Tooltip("Flag to determine if the object is currently held in hands")]
    [SerializeField] private bool _inHands = false;
    #endregion

    [Space(10)]

    #region Detection Settings
    [Header("Detection & Raycasting")]
    [Tooltip("Determine if this logic requires a raycast check")]
    [SerializeField] private bool _needRaycast = true;

    [Tooltip("The origin point from which the ray is cast (e.g., Camera or Eyes)")]
    [SerializeField] private Transform _raycastStartPoint;
    #endregion

    [Space(10)]

    #region Interaction & Events
    [Header("Interaction & Results")]
    [Tooltip("List of actions triggered upon activation")]
    [SerializeField] private UnityEvent _actions;

    [Tooltip("Target folder where generated or moved objects will be placed")]
    [SerializeField] private Transform _targetObjectFolder;
    private GameObject _hittedObject = null;
    #endregion

    [Space(10)]

    #region Input Configuration
    [Header("Input Configuration")]
    [Tooltip("The key used to trigger the interaction")]
    [SerializeField] private KeyCode _triggerKey = KeyCode.F;
    [Tooltip("")]
    [SerializeField] private bool _needDistance = false;
    [Tooltip("")]
    [SerializeField] private float _actDistance = 0;

    #endregion

    public void Initialize(Transform raycastStartPoint)
    {
        _raycastStartPoint = raycastStartPoint;
    }

    public void SetTargetObjectFolder(Transform TargetObjectFolder)
    {
        _targetObjectFolder = TargetObjectFolder;
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

            Transform hitObj = ThrowRay();
            if (hitObj == null) 
            {
                _hittedObject = null;    
                return;
            }
            _hittedObject = hitObj.gameObject;

            if (_targetObjectFolder.childCount == 0) return;

            foreach (Transform child in _targetObjectFolder)
            {
                if (hitObj == child)
                {
                    _actions.Invoke();
                    return;
                }
            }
        }
    }

    public Transform GetTargetObjectFolder() => _targetObjectFolder;

    public GameObject GetTargetObject() => _hittedObject;

    public float GetDistanceToTarget() => Vector3.Distance(transform.position, _hittedObject.transform.position);

    public float GetActDistance()
    {
        if (_needDistance)
            return _actDistance;
        else
            return 1000f;
    }

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
