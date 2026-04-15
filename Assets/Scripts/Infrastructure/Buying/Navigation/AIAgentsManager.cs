#region AI Agents Manager Logic
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgentsManager : MonoBehaviour, IInitializeable
{
    #region Fields

    [Header("Links")]
    [Tooltip("Reference to the product generator (used only for checkout spawning).")]
    [SerializeField] private ProductGenerator _productGenerator;

    [Header("General Settings")]
    [Tooltip("Enables or disables the spawning of AI agents.")]
    [SerializeField] private bool _enabled = true;
    [Tooltip("Maximum number of simultaneously active agents.")]
    [SerializeField] private int _maxAgents = 2;

    [Header("Spawn Settings")]
    [Tooltip("Minimum cooldown between agent spawns in seconds.")]
    [SerializeField, Range(1, 100)] private int _visitCooldownMin = 2;
    [Tooltip("Maximum cooldown between agent spawns in seconds.")]
    [SerializeField, Range(2, 101)] private int _visitCooldownMax = 10;
    [Tooltip("List of agent prefabs to spawn randomly.")]
    [SerializeField] private List<GameObject> _agentPrefabs = new();
    [Tooltip("Spawn point for new agents.")]
    [SerializeField] private Vector3 _spawnPoint = Vector3.zero;
    [Tooltip("Parent transform to organize spawned agents in hierarchy.")]
    [SerializeField] private Transform _folder;

    [Header("Queue Settings")]
    [Tooltip("Position where customers stand to pay.")]
    [SerializeField] private Vector3 _buyingPlacePoint = Vector3.zero;
    [Tooltip("Direction in which the queue extends from the buying point.")]
    [SerializeField] private Vector3 _queueDirection = new Vector3(0, 0, -1);
    [Tooltip("Distance between customers in the queue.")]
    [SerializeField] private float _queueSpacing = 1.2f;
    [Tooltip("Current list of customers waiting in line.")]
    [SerializeField] private List<AICustomer> _customerQueue = new();

    [Header("Debug Info")]
    [Tooltip("List of currently active agent instances.")]
    [SerializeField] private List<GameObject> _activeAgents = new();
    [Tooltip("Available shelves for agent navigation.")]
    [SerializeField] private List<Shelf> _availableShelves = new();

    private Coroutine _trafficCoroutine;
    private EventBinding<OnShelfInitializationEvent> _shelfEventBinding;
    private EventBinding<PaymentResponseEvent> _paymentFinishedEventBinding;
    private EventBinding<OnRatingLevelChange> _ratingLevelChangeEventBinding;

    #endregion


    #region Unity Methods

    private void Update()
    {
        if (_visitCooldownMin >= _visitCooldownMax)
        {
            _visitCooldownMax = _visitCooldownMin + 1;
        }
    }

    private void OnDisable()
    {
        EventBus<OnShelfInitializationEvent>.Deregister(_shelfEventBinding);
        EventBus<PaymentResponseEvent>.Deregister(_paymentFinishedEventBinding);
        EventBus<OnRatingLevelChange>.Deregister(_ratingLevelChangeEventBinding);
    }

    #endregion


    #region Public Methods

    public void Initialize()
    {
        _activeAgents.Clear();
        _customerQueue.Clear();
        _availableShelves.Clear();

        _shelfEventBinding = new EventBinding<OnShelfInitializationEvent>(HandleShelfEvent);
        EventBus<OnShelfInitializationEvent>.Register(_shelfEventBinding);

        _paymentFinishedEventBinding = new EventBinding<PaymentResponseEvent>(HandlePaymentFinished);
        EventBus<PaymentResponseEvent>.Register(_paymentFinishedEventBinding);

        _ratingLevelChangeEventBinding = new EventBinding<OnRatingLevelChange>(HandleRatingChange);
        EventBus<OnRatingLevelChange>.Register(_ratingLevelChangeEventBinding);

        if (_trafficCoroutine != null)
        {
            StopCoroutine(_trafficCoroutine);
        }

        _trafficCoroutine = StartCoroutine(TrafficGoingRoutine());
    }

    public void KillAgent(GameObject agent)
    {
        if (_activeAgents.Contains(agent))
        {
            _activeAgents.Remove(agent);
            if (agent.TryGetComponent(out AICustomer customer))
            {
                customer.FinalizeSession(false);
            }
            Destroy(agent);
        }
    }

    public AICustomer GetFirstInQueue() => _customerQueue.Count > 0 ? _customerQueue[0] : null;

    #endregion


    #region Traffic Logic

    private void SpawnAgent()
    {
        GameObject prefab = _agentPrefabs[Random.Range(0, _agentPrefabs.Count)];
        GameObject agent = Instantiate(prefab, _spawnPoint, Quaternion.identity, _folder);
        _activeAgents.Add(agent);

        if (agent.TryGetComponent(out AICustomer customer))
        {
            Shelf[] targetShelves = GenerateTargetShelves();
            customer.Initialize(targetShelves, this);
        }
    }

    private Shelf[] GenerateTargetShelves()
    {
        List<Shelf> visitableShelves = new List<Shelf>();
        foreach (Shelf shelf in _availableShelves)
        {
            if (shelf.IsVisitable())
                visitableShelves.Add(shelf);
        }

        if (visitableShelves.Count == 0)
        {
            return new Shelf[0];
        }

        int wayLength = Random.Range(1, Mathf.Min(4, visitableShelves.Count + 1));
        List<Shelf> selectedShelves = new List<Shelf>();

        List<Shelf> tempList = new List<Shelf>(visitableShelves);
        for (int i = 0; i < wayLength && tempList.Count > 0; i++)
        {
            int index = Random.Range(0, tempList.Count);
            selectedShelves.Add(tempList[index]);
            tempList.RemoveAt(index);
        }

        return selectedShelves.ToArray();
    }

    private IEnumerator TrafficGoingRoutine()
    {
        while (true)
        {
            if (GlobalStatsBridge.Instance.GetShopOpenClosed())
            {
                if (_enabled && _activeAgents.Count < _maxAgents && _availableShelves.Count > 0)
                {
                    SpawnAgent();
                }
            }

            yield return new WaitForSeconds(Random.Range(_visitCooldownMin, _visitCooldownMax));
        }
    }

    private IEnumerator CooldownedKillingRoutine(AICustomer agent, float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        KillAgent(agent.gameObject);
    }

    #endregion


    #region Queue Logic

    public void JoinQueue(AICustomer customer)
    {
        if (!_customerQueue.Contains(customer))
        {
            _customerQueue.Add(customer);

            int myIndex = _customerQueue.Count - 1;
            Vector3 targetPos = CalculateQueuePosition(myIndex);

            customer.MoveToQueuePoint(targetPos);

            if (_customerQueue.Count == 1)
            {
                StartCoroutine(CheckAndSendPaymentRequest());
            }
        }
    }

    private void AdvanceQueue()
    {
        for (int i = 0; i < _customerQueue.Count; i++)
        {
            Vector3 pos = CalculateQueuePosition(i);
            _customerQueue[i].MoveToQueuePoint(pos);
        }

        if (_customerQueue.Count > 0)
        {
            StartCoroutine(CheckAndSendPaymentRequest());
        }
    }

    private Vector3 CalculateQueuePosition(int index)
    {
        return _buyingPlacePoint + (_queueDirection.normalized * index * _queueSpacing);
    }

    private IEnumerator CheckAndSendPaymentRequest()
    {
        if (_customerQueue.Count == 0)
        {
            yield break;
        }

        AICustomer first = _customerQueue[0];

        yield return new WaitUntil(() =>
        {
            if (first == null)
            {
                return true;
            }

            Vector3 pos1 = first.transform.position;
            Vector3 pos2 = _buyingPlacePoint;

            float flatDistance = Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));

            return flatDistance < (first.GetMinReachDistance() + 0.2f);
        });

        if (first == null)
        {
            yield break;
        }

        GameObject[] products = first.GetProducts();

        if (products == null || products.Length < 1)
        {
            _customerQueue.RemoveAt(0);
            first.ReleaseFromQueue(_spawnPoint);
            AdvanceQueue();
            yield return new WaitForSeconds(7f);
            KillAgent(first.gameObject);
            yield break;
        }

        EventBus<PaymentRequestEvent>.Raise(new PaymentRequestEvent
        {
            Products = products,
            Customer = first
        });
    }

    #endregion


    #region Event Handlers

    private void HandlePaymentFinished(PaymentResponseEvent eventData)
    {
        if (_customerQueue.Count > 0)
        {
            AICustomer finisher = _customerQueue[0];
            _customerQueue.RemoveAt(0);

            finisher.ReleaseFromQueue(_spawnPoint);
            AdvanceQueue();
            StartCoroutine(CooldownedKillingRoutine(finisher, 7));
        }
    }

    private void HandleShelfEvent(OnShelfInitializationEvent data)
    {
        if (data.Adding)
        {
            if (!_availableShelves.Contains(data.Shelf))
            {
                _availableShelves.Add(data.Shelf);
            }
        }
        else
        {
            _availableShelves.Remove(data.Shelf);
        }
    }

    private void HandleRatingChange(OnRatingLevelChange eventData)
    {
        switch (eventData.Level)
        {
            case LevelsEnum.Level0:
                _visitCooldownMin = 12;
                _visitCooldownMax = 36;
                _maxAgents = 1;
                break;
            case LevelsEnum.Level1:
                _visitCooldownMin = 12;
                _visitCooldownMax = 28;
                _maxAgents = 2;
                break;
            case LevelsEnum.Level2:
                _visitCooldownMin = 11;
                _visitCooldownMax = 24;
                _maxAgents = 3;
                break;
            case LevelsEnum.Level3:
                _visitCooldownMin = 9;
                _visitCooldownMax = 20;
                _maxAgents = 3;
                break;
            case LevelsEnum.Level4:
                _visitCooldownMin = 7;
                _visitCooldownMax = 16;
                _maxAgents = 4;
                break;
            case LevelsEnum.Level5:
                _visitCooldownMin = 5;
                _visitCooldownMax = 12;
                _maxAgents = 5;
                break;
        }
    }

    #endregion
}
#endregion