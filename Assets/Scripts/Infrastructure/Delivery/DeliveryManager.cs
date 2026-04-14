using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DeliveryManager : MonoBehaviour, Unity.VisualScripting.IInitializable
{
    #region Fields

    [Header("Dependencies")]
    [Inject]
    [Tooltip("Money balance service.")]
    [SerializeField] private IMoneyBalance _moneyBalance;

    [Header("Settings")]
    [Tooltip("Parent folder for spawned delivery objects.")]
    [SerializeField] private Transform _folder;
    [Tooltip("Cooldown between spawning multiple items.")]
    [SerializeField] private float _spawnCooldown = 1f;

    private List<GameObject> _generatedObjects = new();
    private EventBinding<DeliveryRequestingEvent> _eventBinding;
    private Coroutine _spawningObjectsCoroutine = null;

    private const int SpawnColumns = 3;
    private const int SpawnRows = 4;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        _eventBinding = new EventBinding<DeliveryRequestingEvent>(HandleDeliveryRequest);
        EventBus<DeliveryRequestingEvent>.Register(_eventBinding);
    }

    #endregion


    #region Private Methods

    private void HandleDeliveryRequest(DeliveryRequestingEvent eventData)
    {
        ResetData();

        if (eventData.Amount == 0 || eventData.ProductData == null)
        {
            Debug.LogError("Not enough data | DeliveryManager");
            return;
        }

        EventBus<DeliveryResponseEvent>.Raise(new DeliveryResponseEvent { IsSuccess = true });
        _moneyBalance.RemoveMoney(eventData.ProductData.Price * eventData.Amount, $"Delivery({eventData.Amount} pcs) {eventData.ProductData.TitleName}");
        GlobalStatsBridge.Instance.AddTotalDeliveries();

        if (eventData.Amount > 1)
        {
            if (_spawningObjectsCoroutine != null)
            {
                StopCoroutine(_spawningObjectsCoroutine);
                _spawningObjectsCoroutine = null;
            }

            _spawningObjectsCoroutine = StartCoroutine(SpawningObjects(eventData));
        }
        else
        {
            SpawnObject(eventData.ProductData, 1, new Vector3(0, 0, 0));
            InitializeObjectData(0);
            InitializePackedObjectLayout(0, eventData.ProductData.Icon);
        }
    }

    private void SpawnObject(ProductData data, int id, Vector3 position)
    {
        GameObject obj = Instantiate(data.Prefab);
        obj.transform.SetParent(_folder);
        obj.transform.localPosition = position;

        obj.name = obj.name + " " + data.ObjectName + " " + id;
        obj.SetActive(true);

        _generatedObjects.Add(obj);
    }

    private void InitializeObjectData(int id) => _generatedObjects[id].GetComponent<ItemObject>().Initialize();

    private void InitializePackedObjectLayout(int id, Sprite sprite)
    {
        PackedBoxLayout obj = _generatedObjects[id].GetComponent<PackedBoxLayout>();
        obj.Initialize(sprite);
    }

    private void ResetData()
    {
        _generatedObjects.Clear();
        _generatedObjects = null;
        _generatedObjects = new();
    }

    private Vector3 CalculatePosition(int totalIndex, float xSize, float zSize)
    {
        int xPos = totalIndex % SpawnColumns;
        int zPos = (totalIndex / SpawnColumns) % SpawnRows;
        int yPos = totalIndex / (SpawnColumns * SpawnRows);
        Vector3 targetPos = new Vector3(xPos * xSize, yPos, zPos * zSize);
        return targetPos;
    }

    #endregion


    #region Coroutines

    private IEnumerator SpawningObjects(DeliveryRequestingEvent eventData)
    {
        float x = eventData.ProductData.Prefab.transform.localScale.x;
        float z = eventData.ProductData.Prefab.transform.localScale.z;

        int preGenerated = _folder.transform.childCount;

        for (int i = 0; i < eventData.Amount; i++)
        {
            SpawnObject(eventData.ProductData, i + 1, CalculatePosition(preGenerated + i, x, z));
            InitializeObjectData(i);
            InitializePackedObjectLayout(i, eventData.ProductData.Icon);

            yield return new WaitForSeconds(_spawnCooldown);
        }

        ResetData();
    }

    #endregion


    #region Unity Methods

    private void OnDisable()
    {
        EventBus<DeliveryRequestingEvent>.Deregister(_eventBinding);
    }

    #endregion
}