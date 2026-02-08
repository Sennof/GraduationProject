using System;
using Unity.VisualScripting;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    // This script is used for initialization.
    // Here are all the awake and start methods. 

    private void Awake() // EARLY INITIALIZATION 
    {
        InitializeAll<HomoObjectSwitcher>();
        InitializeAll<Inventory>();
    }

    private void Start() // LATE INITIALIZATION
    {
        InitializeAll<ItemObject>();
    }

    #region Abstract
    private void InitializeAll<T>() where T : Component, IInitializable
    {
        string totalLog = string.Empty;

        T[] objs = GameObject.FindObjectsByType<T>(FindObjectsInactive.Include, 0); // пример использования нового API
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
