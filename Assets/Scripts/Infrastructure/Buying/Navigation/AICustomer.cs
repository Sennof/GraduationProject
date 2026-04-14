#region AI Customer Logic
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(NavMeshObstacle))]
public class AICustomer : MonoBehaviour
{
    #region Fields

    [Header("Movement Settings")]
    [Tooltip("Time the agent stops at each waypoint to simulate browsing.")]
    [SerializeField] private float _stopDuration = 3.0f;
    [Tooltip("Distance at which the agent considers it has reached a destination.")]
    [SerializeField] private float _minReachDistance = 0.5f;

    [Header("State")]
    private NavMeshAgent _navMeshAgent;
    private NavMeshObstacle _navMeshObstacle;
    private Vector3[] _destinations;
    private int _currentDestinationIndex = 0;
    private Coroutine _logicCoroutine;
    private CustomerState _state = CustomerState.Shopping;
    private AIAgentsManager _manager;
    [Tooltip("Products the customer intends to buy.")]
    public GameObject[] _products;

    private enum CustomerState
    {
        Shopping,
        WaitingInQueue,
        Exiting
    }

    #endregion


    #region Unity Methods

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshObstacle = GetComponent<NavMeshObstacle>();
        _navMeshAgent.stoppingDistance = _minReachDistance;
        _navMeshObstacle.enabled = false;
        _navMeshObstacle.carving = true;
    }

    #endregion


    #region Public Methods

    public void Initialize(Vector3[] destinations, GameObject[] products, AIAgentsManager manager)
    {
        _products = products;
        _destinations = destinations;
        _manager = manager;
        _currentDestinationIndex = 0;
        _state = CustomerState.Shopping;

        if (_logicCoroutine != null)
        {
            StopCoroutine(_logicCoroutine);
        }

        _logicCoroutine = StartCoroutine(FollowDestinationsRoutine());
        GlobalStatsBridge.Instance.AddTotalVisitors();
    }

    public void MoveToQueuePoint(Vector3 newPoint)
    {
        _state = CustomerState.WaitingInQueue;

        if (_logicCoroutine != null)
        {
            StopCoroutine(_logicCoroutine);
        }

        _logicCoroutine = StartCoroutine(MoveToPointAndStationary(newPoint));
    }

    public void ReleaseFromQueue(Vector3 exitPoint)
    {
        _state = CustomerState.Exiting;
        _destinations = new Vector3[] { exitPoint };
        _currentDestinationIndex = 0;

        if (_logicCoroutine != null)
        {
            StopCoroutine(_logicCoroutine);
        }

        _logicCoroutine = StartCoroutine(FollowDestinationsRoutine());
    }

    public float GetMinReachDistance() => _minReachDistance;

    public GameObject[] GetProducts() => _products;

    #endregion


    #region Movement Logic

    private void SetNavigationMode(bool isMoving)
    {
        if (isMoving)
        {
            _navMeshObstacle.enabled = false;
            _navMeshAgent.enabled = true;
        }
        else
        {
            if (_navMeshAgent.enabled)
            {
                _navMeshAgent.isStopped = true;
            }

            _navMeshAgent.enabled = false;
            _navMeshObstacle.enabled = true;
        }
    }

    private IEnumerator MoveToPoint(Vector3 point)
    {
        SetNavigationMode(true);
        _navMeshAgent.ResetPath();
        yield return null;

        if (_navMeshAgent.isOnNavMesh)
        {
            _navMeshAgent.SetDestination(point);
            _navMeshAgent.isStopped = false;

            yield return new WaitUntil(() => _navMeshAgent.pathPending || _navMeshAgent.remainingDistance > 0.1f);
            yield return new WaitUntil(() => !_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance + 0.1f);
            yield return new WaitUntil(() => _navMeshAgent.velocity.sqrMagnitude < 0.01f);
        }
    }

    private IEnumerator MoveToPointAndStationary(Vector3 point)
    {
        yield return StartCoroutine(MoveToPoint(point));
        SetNavigationMode(false);
    }

    private IEnumerator FollowDestinationsRoutine()
    {
        while (_currentDestinationIndex < _destinations.Length)
        {
            Vector3 target = _destinations[_currentDestinationIndex];
            yield return StartCoroutine(MoveToPoint(target));

            if (_state == CustomerState.Shopping)
            {
                SetNavigationMode(false);
                yield return new WaitForSeconds(_stopDuration);
                SetNavigationMode(true);
            }
            _currentDestinationIndex++;
        }

        if (_state == CustomerState.Shopping)
        {
            _manager.JoinQueue(this);
        }
    }

    #endregion
}
#endregion