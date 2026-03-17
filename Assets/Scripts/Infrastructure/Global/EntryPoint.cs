using System;
using Unity.VisualScripting;
using UnityEngine;

public class EntryPoint : BaseEntryPoint
{
    public static EntryPoint Instance { get; private set; }

    [Header("Dependencies")]
    [SerializeField] private Transform _defaultRaycastStartPoint;
    [SerializeField] private MoneyBalance _moneyBalance;

    // This script is used for initialization.
    // Here are all the awake and start methods. 
    private void Awake() // EARLY INITIALIZATION 
    {
        //SINGLETON
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        //CORE BELOW
        InitializeAll<GlobalStatsBridge>();
        _moneyBalance.Initialize(1000 /*saving blyat*/);

        InitializeAll<ShelfInitializer>();
        InitializeAll<PenKnivesInitializer>();
        InitializeAll<BuildingWrenchInitializer>();

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
        InitializeAll<BuildedObject>();

        //UI BELOW
        InitializeAll<UIPricing>();
        InitializeAll<InventoryUI>();
        InitializeAll<ShopUIFactory>();
        InitializeAll<UIShopSideMenu>();
        InitializeAll<SearchEngine>();

        //UI BELOW DELETE LATER (rewrite)
        InitializeAll<UIWorkerCard>();
    }

    private void Start() // LATE INITIALIZATION
    {
        InitializeAll<DayCycleManager>();
        InitializeAll<ShopStateTablet>();

    }

    private void OnDisable()
    {
        StopAllCoroutines();   
    }

    #region Special
    public void InitializeInteractingObjects()
    {
        string totalLog = string.Empty;

        InteractingObject[] objs = GameObject.FindObjectsByType<InteractingObject>(FindObjectsInactive.Include, 0);
        if (objs == null || objs.Length == 0)
        {
            Debug.LogWarning($"No objects of type {typeof(InteractingObject).Name} found");
            return;
        }

        int success = 0, fail = 0;
        foreach (var obj in objs)
        {
            try
            {
                obj.Initialize(_defaultRaycastStartPoint);
                success++;
                totalLog += $"({success + fail}) Successfully initialized: {obj.name} | typeof {typeof(InteractingObject).Name}\n";
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize {obj.gameObject.name}: {ex}");
                fail++;
                totalLog += $"({success + fail}) Failed to initialize: {obj.name} | typeof {typeof(InteractingObject).Name}\n";
            }
        }

        totalLog += $"Initialized {success} {typeof(InteractingObject).Name}(s), failed {fail}";
        Debug.Log("[TOTAL INIT LOG]\n" + totalLog);
    }
    #endregion

    
}
