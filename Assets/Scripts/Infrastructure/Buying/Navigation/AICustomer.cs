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

    // Session rating tracking
    private float _sessionRatingDelta = 0f;
    private List<string> _sessionFeedbacks = new List<string>();
    private bool _sessionFinalized = false;

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

    public void Initialize(Shelf[] targetShelves, AIAgentsManager manager)
    {
        _targetShelves = targetShelves;
        _manager = manager;
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

    /// <summary>
    /// Shows a feedback message above the customer's head.
    /// </summary>
    public void ShowFeedback(string message)
    {
        if (_feedbackBubble != null)
            _feedbackBubble.ShowMessage(message);
    }

    /// <summary>
    /// Called when the customer session ends (successful purchase or leaving without buying).
    /// </summary>
    public void FinalizeSession(bool wasSuccessfulPurchase, int totalPriceDifference = 0)
    {
        if (_sessionFinalized) return;
        _sessionFinalized = true;

        // Add purchase-related feedback if it was a successful transaction
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
            // Leaving without buying anything
            if (_collectedProducts.Count == 0)
            {
                _sessionRatingDelta -= 0.05f;
                _sessionFeedbacks.Add("Nothing interesting, left empty-handed.");
                ShowFeedback("Nothing here...");
            }
        }

        // Apply accumulated rating delta and feedbacks to the global manager
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

    private IEnumerator MoveToPointAndExit(Vector3 exitPoint)
    {
        yield return StartCoroutine(MoveToPoint(exitPoint));
        // Session will be finalized when agent is killed by manager
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

                // Determine how many items customer wants from this shelf based on rating
                int desiredAmount = GetDesiredItemCount();
                int takenCount = 0;

                for (int i = 0; i < desiredAmount; i++)
                {
                    GameObject product = targetShelf.PrepareProduct();
                    if (product != null)
                    {
                        _collectedProducts.Add(product);
                        takenCount++;
                    }
                    else
                    {
                        break; // No more items on shelf
                    }
                }

                if (takenCount == 0)
                {
                    // Shelf was completely empty
                    _sessionRatingDelta -= 0.025f;
                    _sessionFeedbacks.Add("Empty shelf...");
                    ShowFeedback("Empty shelf!");
                }
                else if (takenCount < desiredAmount)
                {
                    // Not enough items
                    _sessionRatingDelta -= 0.01f;
                    _sessionFeedbacks.Add("Not enough items...");
                    ShowFeedback("Not enough...");
                }
                else if (Random.value < 0.25f)
                {
                    // 25% chance to show positive feedback when shelf has items
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
        else if (rating < 2f) return Random.Range(1, 3);  // 1-2
        else if (rating < 3f) return Random.Range(2, 4);  // 2-3
        else if (rating < 4f) return Random.Range(3, 5);  // 3-4
        else return Random.Range(4, 7);                   // 4-6
    }

    #endregion
}
#endregion