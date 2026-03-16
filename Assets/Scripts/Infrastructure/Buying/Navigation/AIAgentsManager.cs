#region AI Agents Manager Logic
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgentsManager : MonoBehaviour, IInitializeable
{
    #region Fields
    [Header("Links")]
    [SerializeField] private ProductGenerator _productGenerator;

    [Header("General Settings")]
    [SerializeField] private bool _enabled = true;
    [SerializeField] private int _maxAgents = 6;

    [Header("Spawn Settings")]
    [SerializeField, Range(1, 100)] private int _visitCooldownMin = 2;
    [SerializeField, Range(2, 101)] private int _visitCooldownMax = 10;
    [SerializeField] private List<GameObject> _agentPrefabs = new();
    [SerializeField] private Vector3 _spawnpoint = Vector3.zero;
    [SerializeField] private Transform _folder;

    [Header("Queue Settings")]
    [SerializeField] private Vector3 _buyingPlacePoint = Vector3.zero;
    [SerializeField] private Vector3 _queueDirection = new Vector3(0, 0, -1);
    [SerializeField] private float _queueSpacing = 1.2f;
    [SerializeField] private List<AICustomer> _customerQueue = new();

    [Header("Debug Info")]
    [SerializeField] private List<GameObject> _activeAgents = new();
    [SerializeField] private List<Vector3> _navPoints = new();

    private Coroutine _trafficCor = null;
    private EventBinding<OnShelfInitializationEvent> _shelfBinding = null;
    private EventBinding<PaymentResponseEvent> _paymentFinishedBinding = null;
    #endregion

    #region Unity Methods
    private void Update()
    {
        if (_visitCooldownMin >= _visitCooldownMax)
            _visitCooldownMax = _visitCooldownMin + 1;
    }

    private void OnDisable()
    {
        EventBus<OnShelfInitializationEvent>.Deregister(_shelfBinding);
        EventBus<PaymentResponseEvent>.Deregister(_paymentFinishedBinding);
    }
    #endregion

    #region Public Methods
    public void Initialize()
    {
        _activeAgents.Clear();
        _customerQueue.Clear();

        _shelfBinding = new EventBinding<OnShelfInitializationEvent>(HandleShelfEvent);
        EventBus<OnShelfInitializationEvent>.Register(_shelfBinding);

        _paymentFinishedBinding = new EventBinding<PaymentResponseEvent>(HandlePaymentFinished);
        EventBus<PaymentResponseEvent>.Register(_paymentFinishedBinding);

        if (_trafficCor != null)
            StopCoroutine(_trafficCor);

        _trafficCor = StartCoroutine(TrafficGoingRoutine());
    }

    public void KillAgent(GameObject agent)
    {
        if (_activeAgents.Contains(agent))
        {
            _activeAgents.Remove(agent);
            Destroy(agent);
        }
    }
    #endregion

    #region Traffic Systems
    private IEnumerator TrafficGoingRoutine()
    {
        while (true)
        {
            if (_enabled && _activeAgents.Count < _maxAgents && _navPoints.Count > 0)
            {
                SpawnAgent();
            }
            yield return new WaitForSeconds(Random.Range(_visitCooldownMin, _visitCooldownMax));
        }
    }

    private void SpawnAgent()
    {
        GameObject prefab = _agentPrefabs[Random.Range(0, _agentPrefabs.Count)];
        GameObject agent = Instantiate(prefab, _spawnpoint, Quaternion.identity, _folder);
        _activeAgents.Add(agent);

        if (agent.TryGetComponent(out AICustomer customer))
        {
            customer.Initialize(GenerateWay(), _productGenerator.GenerateProducts(), this);
        }
    }

    private Vector3[] GenerateWay()
    {
        int wayLength = Random.Range(1, Mathf.Min(4, _navPoints.Count + 1));
        List<Vector3> way = new List<Vector3>();

        for (int i = 0; i < wayLength; i++)
        {
            way.Add(_navPoints[Random.Range(0, _navPoints.Count)]);
        }

        return way.ToArray();
    }
    #endregion

    #region Queue Systems
    public void JoinQueue(AICustomer customer)
    {
        if (!_customerQueue.Contains(customer))
        {
            _customerQueue.Add(customer);

            int myIndex = _customerQueue.Count - 1;
            Vector3 targetPos = CalculateQueuePosition(myIndex);

            customer.MoveToQueuePoint(targetPos);

            if (_customerQueue.Count == 1)
                StartCoroutine(CheckAndSendPaymentRequest());
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
            StartCoroutine(CheckAndSendPaymentRequest());
    }

    private Vector3 CalculateQueuePosition(int index)
    {
        return _buyingPlacePoint + (_queueDirection.normalized * index * _queueSpacing);
    }

    private IEnumerator CheckAndSendPaymentRequest()
    {
        if (_customerQueue.Count == 0)
            yield break;

        AICustomer first = _customerQueue[0];

        yield return new WaitUntil(() =>
        {
            if (first == null) return true;

            Vector3 pos1 = first.transform.position;
            Vector3 pos2 = _buyingPlacePoint;

            float flatDistance = Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));

            return flatDistance < (first.GetMinReachDistance() + 0.2f);
        });

        if (first == null) yield break;

        GameObject[] products = first.GetProducts();

        if (products == null || products.Length < 1)
        {
            _customerQueue.RemoveAt(0);
            first.ReleaseFromQueue(_spawnpoint);
            AdvanceQueue();
            yield return new WaitForSeconds(7f);
            KillAgent(first.gameObject);
            yield break;
        }

        EventBus<PaymentRequestEvent>.Raise(new PaymentRequestEvent { Products = products });
    }
    #endregion

    #region Event Handlers
    private void HandlePaymentFinished(PaymentResponseEvent eventData)
    {
        if (_customerQueue.Count > 0)
        {
            AICustomer finisher = _customerQueue[0];
            _customerQueue.RemoveAt(0);

            finisher.ReleaseFromQueue(_spawnpoint);
            AdvanceQueue();
        }
    }

    private void HandleShelfEvent(OnShelfInitializationEvent data)
    {
        if (data.Adding)
            _navPoints.Add(data.GlobalPosition);
        else
            _navPoints.Remove(data.GlobalPosition);
    }
    #endregion
}
#endregion
