using System;
using Unity.VisualScripting;
using UnityEngine;

public class BaseEntryPoint : MonoBehaviour
{
    #region Protected Methods

    protected void InitializeAll<T>() where T : Component, IInitializable
    {
        string totalLog = string.Empty;

        T[] objects = GameObject.FindObjectsByType<T>(FindObjectsInactive.Include, 0);
        if (objects == null || objects.Length == 0)
        {
            Debug.LogWarning($"No objects of type {typeof(T).Name} found");
            return;
        }

        int successCount = 0;
        int failCount = 0;
        foreach (var obj in objects)
        {
            try
            {
                obj.Initialize();
                successCount++;
                totalLog += $"({successCount + failCount}) Successfully initialized: {obj.name} | typeof {typeof(T).Name}\n";
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize {obj.gameObject.name}: {ex}");
                failCount++;
                totalLog += $"({successCount + failCount}) Failed to initialize: {obj.name} | typeof {typeof(T).Name}\n";
            }
        }

        totalLog += $"Initialized {successCount} {typeof(T).Name}(s), failed {failCount}";
        Debug.Log("[TOTAL INIT LOG]\n" + totalLog);
    }

    #endregion
}