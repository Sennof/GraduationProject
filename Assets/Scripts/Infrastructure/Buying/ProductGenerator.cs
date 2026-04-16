using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ProductGenerator : MonoBehaviour, IInitializeable
{
    #region Fields

    [Header("Settings")]
    [Tooltip("List of shelves (not directly used for generation anymore, kept for potential analytics).")]
    [SerializeField] private List<Shelf> _shelves = new();
    [Tooltip("Folder where generated products are placed.")]
    [SerializeField] private Transform _spawnFolder;
    [Tooltip("Cooldown between spawning products at checkout.")]
    [SerializeField, Range(0, 10)] private float _puttingCooldown = 0.5f;

    [Inject] private IRatingManager _ratingManager;

    private List<GameObject> _generatedObjects = new();
    private Coroutine _spawningProductsCoroutine = null;
    private int _currentRealTotalPrice = 0;

    private EventBinding<OnShelfInitializationEvent> _shelfEventBinding;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        _shelfEventBinding = new EventBinding<OnShelfInitializationEvent>(HandleShelfEvent);
        EventBus<OnShelfInitializationEvent>.Register(_shelfEventBinding);
    }

    public void SpawnBuyingProducts(GameObject[] products)
    {
        if (_spawningProductsCoroutine != null)
        {
            StopCoroutine(_spawningProductsCoroutine);
        }

        // Calculate real total price synchronously before spawning
        _currentRealTotalPrice = CalculateRealTotalPrice(products);

        _spawningProductsCoroutine = StartCoroutine(SpawningProducts(products));
    }

    public void DestroyAllGenerated()
    {
        foreach (GameObject obj in _generatedObjects)
        {
            Destroy(obj);
        }

        _generatedObjects.Clear();
        _currentRealTotalPrice = 0;
    }

    public int GetRealTotalPrice() => _currentRealTotalPrice;

    #endregion


    #region Event Handlers

    private void HandleShelfEvent(OnShelfInitializationEvent eventData)
    {
        if (eventData.Adding)
        {
            if (!_shelves.Contains(eventData.Shelf))
            {
                _shelves.Add(eventData.Shelf);
            }
        }
        else
        {
            _shelves.Remove(eventData.Shelf);
        }
    }

    #endregion


    #region Private Methods

    private int CalculateRealTotalPrice(GameObject[] products)
    {
        int total = 0;
        float pricingMod = GlobalStatsBridge.Instance.GetPricingMod();

        foreach (GameObject product in products)
        {
            if (product != null && product.TryGetComponent(out ItemObject item))
            {
                ProductData data = item.GetProductData();
                if (data != null)
                {
                    total += (int)(data.Price * pricingMod);
                }
            }
        }

        return total;
    }

    #endregion


    #region Coroutines

    private IEnumerator SpawningProducts(GameObject[] products)
    {
        foreach (GameObject product in products)
        {
            if (product == null) continue;

            if (product.TryGetComponent(out ItemObject item))
            {
                _generatedObjects.Add(product);

                product.GetComponent<Rigidbody>().isKinematic = false;
                if (product.TryGetComponent(out Interactable interactable))
                {
                    interactable.SetActiveState(false);
                }

                product.transform.SetParent(_spawnFolder);
                product.transform.position = _spawnFolder.position;
                product.SetActive(true);

                yield return new WaitForSeconds(_puttingCooldown);
            }
        }
    }

    #endregion


    #region Unity Methods

    private void OnDisable() => EventBus<OnShelfInitializationEvent>.Deregister(_shelfEventBinding);

    #endregion
}