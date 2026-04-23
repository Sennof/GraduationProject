#region AI Customer Logic
using System.Collections;
using System.Collections.Generic;
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

    [Header("Feedback Bubble")]
    [Tooltip("Reference to the visual feedback bubble component.")]
    [SerializeField] private CustomerFeedbackBubble _feedbackBubble;

    [Header("State")]
    private NavMeshAgent _navMeshAgent;
    private NavMeshObstacle _navMeshObstacle;
    private Shelf[] _targetShelves;
    private int _currentShelfIndex = 0;
    private Coroutine _logicCoroutine;
    private CustomerState _state = CustomerState.Shopping;
    private AIAgentsManager _manager;
    private List<GameObject> _collectedProducts = new List<GameObject>();

    private float _sessionRatingDelta = 0f;
    private List<string> _sessionFeedbacks = new List<string>();
    private bool _sessionFinalized = false;

    private CustomerClass _customerClass;

    // Flags for current shelf feedback
    private bool _shelfTooExpensive = false;
    private bool _shelfEmpty = false;
    private bool _shelfNotEnough = false;
    private bool _shelfGreatPriceShown = false;

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

        if (_feedbackBubble != null)
            _feedbackBubble.Initialize(transform);
    }

    #endregion


    #region Public Methods

    public void Initialize(Shelf[] targetShelves, AIAgentsManager manager, CustomerClass customerClass)
    {
        _targetShelves = targetShelves;
        _manager = manager;
        _customerClass = customerClass;
        _collectedProducts.Clear();
        _currentShelfIndex = 0;
        _state = CustomerState.Shopping;
        _sessionRatingDelta = 0f;
        _sessionFeedbacks.Clear();
        _sessionFinalized = false;

        if (_logicCoroutine != null)
        {
            StopCoroutine(_logicCoroutine);
        }

        _logicCoroutine = StartCoroutine(FollowShelvesRoutine());
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

        if (_logicCoroutine != null)
        {
            StopCoroutine(_logicCoroutine);
        }

        _logicCoroutine = StartCoroutine(MoveToPointAndExit(exitPoint));
    }

    public float GetMinReachDistance() => _minReachDistance;

    public GameObject[] GetProducts() => _collectedProducts.ToArray();

    public void ShowFeedback(string message)
    {
        if (_feedbackBubble != null)
            _feedbackBubble.ShowMessage(message);
    }

    public void FinalizeSession(bool wasSuccessfulPurchase, int totalPriceDifference = 0)
    {
        if (_sessionFinalized) return;
        _sessionFinalized = true;

        if (wasSuccessfulPurchase)
        {
            if (totalPriceDifference == 0)
            {
                _sessionRatingDelta += 0.1f;
                _sessionFeedbacks.Add("All good, I liked it.");
                ShowFeedback("All good!");
            }
            else if (totalPriceDifference < 0)
            {
                _sessionRatingDelta += 0.025f;
                _sessionFeedbacks.Add("Cashier is a nice guy, miscalculated the receipt.");
                ShowFeedback("Nice discount!");
            }
            else
            {
                _sessionRatingDelta -= 0.12f;
                _sessionFeedbacks.Add("I was robbed!");
                ShowFeedback("I was robbed!");
            }
        }
        else
        {
            if (_collectedProducts.Count == 0)
            {
                _sessionRatingDelta -= 0.05f;
                _sessionFeedbacks.Add("Nothing interesting, left empty-handed.");
                ShowFeedback("Nothing here...");
            }
        }

        if (Mathf.Abs(_sessionRatingDelta) > 0.001f || _sessionFeedbacks.Count > 0)
        {
            string combinedFeedback = _sessionFeedbacks.Count > 0 ? string.Join(" ", _sessionFeedbacks) : "";
            RatingManager.Instance.ApplySessionFeedback(_sessionRatingDelta, combinedFeedback);
        }
    }

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
                _navMeshAgent.isStopped = true;

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

    private IEnumerator MoveToPointAndExit(Vector3 exitPoint)
    {
        yield return StartCoroutine(MoveToPoint(exitPoint));
    }

    private IEnumerator FollowShelvesRoutine()
    {
        while (_currentShelfIndex < _targetShelves.Length)
        {
            Shelf targetShelf = _targetShelves[_currentShelfIndex];
            Vector3 navPoint = targetShelf.GetNavPointPosition();

            yield return StartCoroutine(MoveToPoint(navPoint));

            if (_state == CustomerState.Shopping)
            {
                SetNavigationMode(false);

                // Reset shelf flags
                _shelfTooExpensive = false;
                _shelfEmpty = false;
                _shelfNotEnough = false;
                _shelfGreatPriceShown = false;

                int desiredAmount = GetDesiredItemCount();
                int takenCount = 0;
                int attemptedCount = 0; // how many items we actually tried to take

                for (int i = 0; i < desiredAmount; i++)
                {
                    GameObject product = targetShelf.PrepareProduct();
                    if (product == null)
                    {
                        _shelfEmpty = true;
                        break;
                    }

                    attemptedCount++;
                    if (product.TryGetComponent(out ItemObject item))
                    {
                        ProductData data = item.GetProductData();
                        if (data != null)
                        {
                            float currentMarkup = targetShelf.GetProductMarkup(data);
                            float maxAllowedMarkup = GetMaxAllowedMarkup();

                            if (currentMarkup > maxAllowedMarkup)
                            {
                                targetShelf.ReturnProduct(product);
                                _shelfTooExpensive = true;
                                _sessionRatingDelta -= 0.05f;
                                _sessionFeedbacks.Add("Too expensive!");
                                continue;
                            }

                            _collectedProducts.Add(product);
                            takenCount++;

                            if (!_shelfGreatPriceShown && Random.value < 0.2f)
                            {
                                ShowFeedback("Great price!");
                                _sessionRatingDelta += 0.1f;
                                _sessionFeedbacks.Add("Great price!");
                                _shelfGreatPriceShown = true;
                            }
                        }
                    }
                }

                // Determine feedback priority
                if (_shelfTooExpensive)
                {
                    ShowFeedback("Too expensive!");
                }
                else if (_shelfEmpty)
                {
                    _sessionRatingDelta -= 0.025f;
                    _sessionFeedbacks.Add("Empty shelf...");
                    ShowFeedback("Empty shelf!");
                }
                else if (takenCount < desiredAmount && attemptedCount > 0)
                {
                    _shelfNotEnough = true;
                    _sessionRatingDelta -= 0.01f;
                    _sessionFeedbacks.Add("Not enough items...");
                    ShowFeedback("Not enough...");
                }
                else if (takenCount == desiredAmount && Random.value < 0.25f)
                {
                    ShowFeedback("Just what I needed!");
                }

                yield return new WaitForSeconds(_stopDuration);
                SetNavigationMode(true);
            }
            _currentShelfIndex++;
        }

        if (_state == CustomerState.Shopping)
        {
            _manager.JoinQueue(this);
        }
    }

    private int GetDesiredItemCount()
    {
        float rating = RatingManager.Instance.GetRating();

        if (rating < 1f) return 1;
        else if (rating < 2f) return Random.Range(1, 3);
        else if (rating < 3f) return Random.Range(2, 4);
        else if (rating < 4f) return Random.Range(3, 5);
        else return Random.Range(4, 7);
    }

    private float GetMaxAllowedMarkup()
    {
        switch (_customerClass)
        {
            case CustomerClass.Poor:
                return 0.2f;
            case CustomerClass.Middle:
                return 0.5f;
            case CustomerClass.Rich:
                return float.MaxValue;
            default:
                return 0.2f;
        }
    }

    #endregion
}
#endregion