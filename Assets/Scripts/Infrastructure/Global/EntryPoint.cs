using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    // This script is used for initialization.
    // Here are all the awake and start methods. 

    private void Awake() // EARLY INITIALIZATION 
    {
        EIHomoObjectSwitcher();
    }

    private void Start() // LATE INITIALIZATION
    {
        
    }

    #region EARLY INITIALIZATION
    private void EIHomoObjectSwitcher()
    {
        HomoObjectSwitcher[] objs = GameObject.FindObjectsByType<HomoObjectSwitcher>(0);
        if (objs == null)
        {
            Debug.LogError("Failed to get HomoObjectSwitcher | EntryPoint");
            return;
        }

        int sCounter = 0;
        int fCounter = 0;
        foreach(HomoObjectSwitcher obj in objs)
        {
            try
            {
                obj.Initialize();
                sCounter++;
            }
            catch 
            {
                Debug.LogError($"Failed to initialize HomoObjectSwitcher({obj.gameObject.name}) | EntryPoint" +
                    $"\nError #{fCounter}");
                fCounter++;
            }
        }
        Debug.Log($"Initialized {sCounter} HomoObjectSwitchers " +
            $"\nFailed to initialize {fCounter}" +
            $"\n| EntryPoint |\n");
    }

    #endregion


    #region LATE INITIALIZATION

    #endregion
}
