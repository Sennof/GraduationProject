#region AI Customer Logic
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(NavMeshObstacle))]
public class AICustomer : MonoBehaviour
{
    #region Fields
    [Header("Movement Settings")]
    [SerializeField] private float _stopDuration = 3.0f;
    [SerializeField] private float _minDistance = 0.5f;

    [Header("State")]
    private NavMeshAgent _agent;
    private NavMeshObstacle _obstacle;
    private Vector3[] _destinations;
    private int _currentDestinationIndex = 0;
    private Coroutine _logicCor = null;
    private CustomerState _state = CustomerState.Shopping;
    private AIAgentsManager _manager;
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
        _agent = GetComponent<NavMeshAgent>();
        _obstacle = GetComponent<NavMeshObstacle>();
        _agent.stoppingDistance = _minDistance;
        _obstacle.enabled = false;
        _obstacle.carving = true;
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

        if (_logicCor != null)
            StopCoroutine(_logicCor);

        _logicCor = StartCoroutine(FollowDestinationsRoutine());
        GlobalStatsBridge.Instance.AddTotalVisitors();
    }

    public void MoveToQueuePoint(Vector3 newPoint)
    {
        _state = CustomerState.WaitingInQueue;

        if (_logicCor != null)
            StopCoroutine(_logicCor);

        _logicCor = StartCoroutine(MoveToPointAndStationary(newPoint));
    }

    public void ReleaseFromQueue(Vector3 exitPoint)
    {
        _state = CustomerState.Exiting;
        _destinations = new Vector3[] { exitPoint };
        _currentDestinationIndex = 0;

        if (_logicCor != null)
            StopCoroutine(_logicCor);

        _logicCor = StartCoroutine(FollowDestinationsRoutine());
    }

    public float GetMinReachDistance() => _minDistance;

    public GameObject[] GetProducts() => _products;
    #endregion

    #region Internal Logic
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

    private IEnumerator MoveToPointAndStationary(Vector3 point)
    {
        yield return StartCoroutine(MoveToPoint(point));
        SetNavigationMode(false);
    }

    private IEnumerator MoveToPoint(Vector3 point)
    {
        SetNavigationMode(true);
        _agent.ResetPath();
        yield return null;

        if (_agent.isOnNavMesh)
        {
            _agent.SetDestination(point);
            _agent.isStopped = false;

            yield return new WaitUntil(() => _agent.pathPending || _agent.remainingDistance > 0.1f);
            yield return new WaitUntil(() => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance + 0.1f);
            yield return new WaitUntil(() => _agent.velocity.sqrMagnitude < 0.01f);
        }
    }

    private void SetNavigationMode(bool isMoving)
    {
        if (isMoving)
        {
            _obstacle.enabled = false;
            _agent.enabled = true;
        }
        else
        {
            if (_agent.enabled)
                _agent.isStopped = true;

            _agent.enabled = false;
            _obstacle.enabled = true;
        }
    }
    #endregion
}
#endregion