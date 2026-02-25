using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class DeliveryManager : MonoBehaviour, Unity.VisualScripting.IInitializable
{
    [Inject]
    [SerializeField] private IMoneyBalance _moneyBalance;

    [SerializeField] private Transform _folder;
    [SerializeField] private Transform _spawnPoint;

    private List<GameObject> _generatedObjects = new();
    private EventBinding<DeliveryRequestingEvent> _eventBinding;
    private Coroutine _spawningObjectsCor = null;

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
        if (eventData.Amount == 0 || eventData.ProductData == null)
        {
            Debug.LogError("Not enough data | DeliveryManager");
            return;
        }
        _moneyBalance.RemoveMoney(eventData.ProductData.Price * eventData.Amount, $"Доставка({eventData.Amount}шт.) {eventData.ProductData.TitleName} {eventData.ProductData.Price * eventData.Amount}");

        if(eventData.Amount > 1)
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
            SpawnObject(eventData.ProductData, 1);
            InitializeObject(0);
        }
    }

    private void SpawnObject(ProductData _data, int id)
    {
        GameObject obj = Instantiate(_data.Prefab, _spawnPoint.position, Quaternion.identity, _folder);
        obj.name = _data.ObjectName + id;
        obj.SetActive(true);

        _generatedObjects.Add(obj);
    }

    private void InitializeObject(int id) => _generatedObjects[id].GetComponent<ItemObject>().Initialize();

    private IEnumerator SpawningObjects(DeliveryRequestingEvent eventData)
    {
        _generatedObjects.Clear();

        for (int i = 0; i < eventData.Amount; i++)
        {
            SpawnObject(eventData.ProductData, i + 1);
            InitializeObject(i);

            yield return new WaitForSeconds(1f);
        }
    }
}
