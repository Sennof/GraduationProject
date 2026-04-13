using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DeliveryManager : MonoBehaviour, Unity.VisualScripting.IInitializable
{
    [Inject]
    [SerializeField] private IMoneyBalance _moneyBalance;
    [SerializeField] private Transform _folder;
    [SerializeField] private float _spawnCooldown = 1f;

    private List<GameObject> _generatedObjects = new();
    private EventBinding<DeliveryRequestingEvent> _eventBinding;
    private Coroutine _spawningObjectsCor = null;

    private int _spawnColumns = 3;
    private int _spawnRows = 4;

    public void Initialize()
    {
        _eventBinding = new EventBinding<DeliveryRequestingEvent>(HandleDeliveryRequest);
        EventBus<DeliveryRequestingEvent>.Register(_eventBinding);
    }

    private void OnDisable()
    {
        EventBus<DeliveryRequestingEvent>.Deregister(_eventBinding);
    }

    private void HandleDeliveryRequest(DeliveryRequestingEvent eventData)
    {
        ResetData();

        if (eventData.Amount == 0 || eventData.ProductData == null)
        {
            Debug.LogError("Not enough data | DeliveryManager");
            return;
        }

        EventBus<DeliveryResponseEvent>.Raise(new DeliveryResponseEvent { isSuccess = true});
        _moneyBalance.RemoveMoney(eventData.ProductData.Price * eventData.Amount, $"Доставка({eventData.Amount}шт.) {eventData.ProductData.TitleName} {eventData.ProductData.Price * eventData.Amount}");
        GlobalStatsBridge.Instance.AddTotalDeliveries();

        if (eventData.Amount > 1)
        {
            if(_spawningObjectsCor != null)
            {
                StopCoroutine( _spawningObjectsCor );
                _spawningObjectsCor = null;
            }

            _spawningObjectsCor = StartCoroutine(SpawningObjects(eventData));
        }
        else
        {
            SpawnObject(eventData.ProductData, 1, new Vector3(0, 0, 0));
            InitializeObjectData(0);
            InitializePackedObjectLayout(0, eventData.ProductData.Icon);
        }
    }

    private void SpawnObject(ProductData _data, int id, Vector3 pos)
    {
        GameObject obj = Instantiate(_data.Prefab);
        obj.transform.SetParent(_folder);
        obj.transform.localPosition = pos;

        obj.name = obj.name + " " + _data.ObjectName + " " + id;
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

    private IEnumerator SpawningObjects(DeliveryRequestingEvent eventData)
    {
        float x = eventData.ProductData.Prefab.transform.localScale.x;
        float z = eventData.ProductData.Prefab.transform.localScale.z;

        int preGenerated = _folder.transform.childCount;

        for (int i = 0; i < eventData.Amount; i++)
        {
            SpawnObject(eventData.ProductData, i + 1, CalcPosition(preGenerated + i, x, z));
            InitializeObjectData(i);
            InitializePackedObjectLayout(i, eventData.ProductData.Icon);

            yield return new WaitForSeconds(_spawnCooldown);
        }

        ResetData();
    }

    private Vector3 CalcPosition(int totalIndex, float x, float z)
    {
        int xPos = totalIndex % _spawnColumns;
        int zPos = (totalIndex / _spawnColumns) % _spawnRows;
        int yPos = totalIndex / (_spawnColumns * _spawnRows);
        Vector3 targetPos = new Vector3(xPos * x, yPos, zPos * z);
        return targetPos;
    }
}
