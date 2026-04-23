using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class InteractingObject : MonoBehaviour
{
    #region Fields

    [Header("State Settings")]
    [Tooltip("Global toggle to control system operation.")]
    [SerializeField] private bool _enabled = true;
    [Tooltip("Whether the object is currently held in hands.")]
    [SerializeField] private bool _inHands = false;

    [Space(10)]

    [Header("Detection & Raycasting")]
    [Tooltip("Require raycast check for activation.")]
    [SerializeField] private bool _needRaycast = true;
    [Tooltip("Origin point for the interaction raycast.")]
    [SerializeField] private Transform _raycastStartPoint;

    [Space(10)]

    [Header("Interaction & Results")]
    [Tooltip("Actions triggered upon activation.")]
    [SerializeField] private UnityEvent _actions;
    [Tooltip("Target folder for generated or moved objects.")]
    [SerializeField] private Transform _targetObjectFolder;

    [Space(10)]

    [Header("Input Configuration")]
    [Tooltip("Key to trigger the interaction.")]
    [SerializeField] private KeyCode _triggerKey = KeyCode.F;
    [Tooltip("Enable distance check for activation.")]
    [SerializeField] private bool _needDistance = false;
    [Tooltip("Maximum activation distance.")]
    [SerializeField] private float _actDistance = 0;

    private GameObject _hittedObject = null;

    #endregion


    #region Public Methods

    public void Initialize(Transform raycastStartPoint)
    {
        _raycastStartPoint = raycastStartPoint;
    }

    public void SetTargetObjectFolder(Transform targetObjectFolder)
    {
        _targetObjectFolder = targetObjectFolder;
    }

    public Transform GetTargetObjectFolder() => _targetObjectFolder;

    public GameObject GetTargetObject() => _hittedObject;

    public float GetDistanceToTarget() => Vector3.Distance(transform.position, _hittedObject.transform.position);

    public float GetActDistance()
    {
        if (_needDistance)
        {
            return _actDistance;
        }
        else
        {
            return 1000f;
        }
    }

    public void SetInHands() => _inHands = true;

    public void SetOutHands() => _inHands = false;

    public bool GetIsInHands() => _inHands;

    public Transform GetRaycastStartPoint() => _raycastStartPoint;

    #endregion


    #region Private Methods

    private Transform ThrowRay()
    {
        RaycastHit rayHit;

        if (Physics.Raycast(_raycastStartPoint.position, _raycastStartPoint.TransformDirection(Vector3.forward), out rayHit, 5))
        {
            return rayHit.transform;
        }

        return null;
    }

    #endregion


    #region Unity Methods

    private void Update()
    {
        if (_enabled == false || _inHands == false)
        {
            return;
        }

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

            if (_targetObjectFolder.childCount == 0)
            {
                return;
            }

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

    #endregion
}