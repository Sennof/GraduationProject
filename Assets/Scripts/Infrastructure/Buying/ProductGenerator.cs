
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductGenerator : MonoBehaviour, IInitializeable
{
    #region Fields
    [Header("Settings")]
    [SerializeField] private List<Shelf> _shelves = new();
    [SerializeField] private Transform _spawnFolder;
    [SerializeField, Range(0, 10)] private float _puttingCooldown = 0.5f;

    private List<GameObject> _generatedObjects = new();
    private Coroutine _spawningProductsCor = null;

    private int _currentRealTotalPrice = 0;

    private EventBinding<OnShelfInitializationEvent> _shelfBinding;
    #endregion

    #region Public Methods
    public void Initialize()
    {
        _shelfBinding = new EventBinding<OnShelfInitializationEvent>(HandleShelfEvent);
        EventBus<OnShelfInitializationEvent>.Register(_shelfBinding);
    }

    public void SpawnBuyingProducts(GameObject[] products)
    {
        if (_spawningProductsCor != null) StopCoroutine(_spawningProductsCor);
            _spawningProductsCor = StartCoroutine(SpawningProducts(products));
    }

    public void DestroyAllGenerated()
    {
        foreach(GameObject obj in _generatedObjects)
        {
            Destroy(obj);
        }

        _generatedObjects.Clear();
    }

    public int GetRealTotalPrice() => _currentRealTotalPrice;

    public GameObject[] GenerateProducts()
    {
        List<GameObject> items = new();
        foreach (Shelf shelf in _shelves)
        {
            GameObject item = shelf.PrepareProduct();
            if (item != null) items.Add(item);
        }
        return items.ToArray();
    }


    #endregion

    #region Private Methods
    private void HandleShelfEvent(OnShelfInitializationEvent eventData)
    {
        if (eventData.Adding)
        {
            if (!_shelves.Contains(eventData.Shelf)) _shelves.Add(eventData.Shelf);
        }
        else _shelves.Remove(eventData.Shelf);
    }

    private IEnumerator SpawningProducts(GameObject[] products)
    {
        foreach (GameObject product in products)
        {
            if (product.TryGetComponent(out ItemObject item))
            {
                _generatedObjects.Add(product);

                ProductData data = item.GetProductData();
                product.GetComponent<Rigidbody>().isKinematic = false;
                product.GetComponent<Interactable>().SetActiveState(false);

                product.transform.SetParent(_spawnFolder);
                product.transform.position = _spawnFolder.position;

                product.SetActive(true);

                _currentRealTotalPrice += (int)(data.Price * GlobalStatsBridge.Instance.GetPricingMod());

                yield return new WaitForSeconds(_puttingCooldown);
            }
        }
    }

    private void OnDisable() => EventBus<OnShelfInitializationEvent>.Deregister(_shelfBinding);
    #endregion
}