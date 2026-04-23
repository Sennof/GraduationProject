using System;
using Unity.VisualScripting;
using UnityEngine;

public class EntryPoint : BaseEntryPoint
{
    #region Fields

    [Header("Dependencies")]
    [Tooltip("Default raycast start point for interactions.")]
    [SerializeField] private Transform _defaultRaycastStartPoint;
    [Tooltip("Money balance component.")]
    [SerializeField] private MoneyBalance _moneyBalance;

    public static EntryPoint Instance { get; private set; }

    #endregion


    #region Unity Methods

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        InitializeAll<GlobalStatsBridge>();
        _moneyBalance.Initialize(1000);
        InitializeAll<RatingManager>();

        InitializeAll<ShelfInitializer>();
        InitializeAll<PenKnivesInitializer>();
        InitializeAll<BuildingWrenchInitializer>();
        InitializeAll<PriceTagMakerInitializer>();

        InitializeAll<PlayerServicesManager>();
        InitializeAll<UIChecking>();

        InitializeAll<HomoObjectSwitcher>();
        InitializeAll<Inventory>();
        InitializeAll<DeliveryManager>();
        InitializeAll<UnpackingPlot>();
        InitializeAll<BuildingManager>();
        InitializeAll<ProductGenerator>();
        InitializeAll<AIAgentsManager>();

        InitializeAll<BuyingManager>();

        InitializeAll<ItemObject>();
        InitializeInteractingObjects();
        InitializeAll<Shelf>();
        InitializeAll<PenKnife>();
        InitializeAll<BuildingWrench>();
        InitializeAll<PriceTagMaker>();
        InitializeAll<PriceTagHanger>();
        InitializeAll<BuildedObject>();

        InitializeAll<UIVersionSetter>();

        InitializeAll<InventoryUI>();
        InitializeAll<ShopUIFactory>();
        InitializeAll<UIShopSideMenu>();
        InitializeAll<SearchEngine>();

        InitializeAll<DeliveryAnimationHandler>();

        InitializeAll<UIWorkerCard>();

        // NEW: Initialize UI systems that need to be ready early
        InitializeAll<UIShelfConfig>();
        InitializeAll<CursorManager>();
    }

    private void Start()
    {
        InitializeAll<DayCycleManager>();
        InitializeAll<ShopStateTablet>();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    #endregion


    #region Special Initialization

    public void InitializeInteractingObjects()
    {
        string totalLog = string.Empty;

        InteractingObject[] objects = GameObject.FindObjectsByType<InteractingObject>(FindObjectsInactive.Include, 0);
        if (objects == null || objects.Length == 0)
        {
            Debug.LogWarning($"No objects of type {typeof(InteractingObject).Name} found");
            return;
        }

        int successCount = 0;
        int failCount = 0;
        foreach (var obj in objects)
        {
            try
            {
                obj.Initialize(_defaultRaycastStartPoint);
                successCount++;
                totalLog += $"({successCount + failCount}) Successfully initialized: {obj.name} | typeof {typeof(InteractingObject).Name}\n";
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize {obj.gameObject.name}: {ex}");
                failCount++;
                totalLog += $"({successCount + failCount}) Failed to initialize: {obj.name} | typeof {typeof(InteractingObject).Name}\n";
            }
        }

        totalLog += $"Initialized {successCount} {typeof(InteractingObject).Name}(s), failed {failCount}";
        Debug.Log("[TOTAL INIT LOG]\n" + totalLog);
    }

    #endregion
}