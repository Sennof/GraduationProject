using System;
using Unity.VisualScripting;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    public static EntryPoint Instance { get; private set; }

    [Header("Dependencies")]
    [Header("InteractingObjects")]
    [SerializeField] private Transform _defaultRaycastStartPoint;
    [Tooltip("Target object for penknife")]
    [SerializeField] private Transform _targetObj;

    // This script is used for initialization.
    // Here are all the awake and start methods. 
    private void Awake() // EARLY INITIALIZATION 
    {
        //SINGLETON
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        //CORE BELOW
        InitializeAll<HomoObjectSwitcher>();
        InitializeAll<Inventory>();
        InitializeAll<DeliveryManager>();
        InitializeAll<UnpackingPlot>();

        //UI BELOW
        InitializeAll<InventoryUI>();
        InitializeAll<ShopUIFactory>();
        InitializeAll<UIShopSideMenu>();
        InitializeAll<SearchEngine>();

        //UI BELOW DELETE LATER (rewrite)
        InitializeAll<UIWorkerCard>();
    }

    private void Start() // LATE INITIALIZATION
    {
        InitializeAll<ItemObject>();
        InitializeInteractingObjects();
    }

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
                obj.Initialize(_defaultRaycastStartPoint, _targetObj);
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

    #region Abstract
    private void InitializeAll<T>() where T : Component, IInitializable
    {
        string totalLog = string.Empty;

        T[] objs = GameObject.FindObjectsByType<T>(FindObjectsInactive.Include, 0);
        if (objs == null || objs.Length == 0)
        {
            Debug.LogWarning($"No objects of type {typeof(T).Name} found");
            return;
        }

        int success = 0, fail = 0;
        foreach (var obj in objs)
        {
            try
            {
                obj.Initialize();
                success++;
                totalLog += $"({success+fail}) Successfully initialized: {obj.name} | typeof {typeof(T).Name}\n";
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize {obj.gameObject.name}: {ex}");
                fail++;
                totalLog += $"({success + fail}) Failed to initialize: {obj.name} | typeof {typeof(T).Name}\n";
            }
        }

        totalLog += $"Initialized {success} {typeof(T).Name}(s), failed {fail}";
        Debug.Log("[TOTAL INIT LOG]\n" + totalLog);
    }
    #endregion
}
