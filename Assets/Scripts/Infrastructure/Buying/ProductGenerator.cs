using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ProductGenerator : MonoBehaviour, IInitializeable
{
    #region Fields

    [Header("Settings")]
    [Tooltip("List of shelves available for product generation.")]
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
        _spawningProductsCoroutine = StartCoroutine(SpawningProducts(products));
    }

    public void DestroyAllGenerated()
    {
        foreach (GameObject obj in _generatedObjects)
        {
            Destroy(obj);
        }

        _generatedObjects.Clear();
    }

    public int GetRealTotalPrice() => _currentRealTotalPrice;

    public GameObject[] GenerateProducts()
    {
        int needProductsMin = Random.Range(1, _shelves.Count);
        List<GameObject> items = new();

        foreach (Shelf shelf in _shelves)
        {
            GameObject item = shelf.PrepareProduct();
            if (item != null)
            {
                items.Add(item);
            }
        }

        if (items.Count < needProductsMin)
        {
            _ratingManager.ReduceRating(0.05f);
            _ratingManager.AddFeedback("Small assortment...");
        }
        else if (items.Count == needProductsMin)
        {
            _ratingManager.AddRating(0.025f);
            _ratingManager.AddFeedback("Found what I needed!");
        }
        else
        {
            _ratingManager.AddRating(0.05f);
            _ratingManager.AddFeedback("You have so much stuff!");
        }

        GlobalStatsBridge.Instance.AddTotalProducts(items.Count);

        return items.ToArray();
    }

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


    #region Coroutines

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

    #endregion


    #region Unity Methods

    private void OnDisable() => EventBus<OnShelfInitializationEvent>.Deregister(_shelfEventBinding);

    #endregion
}