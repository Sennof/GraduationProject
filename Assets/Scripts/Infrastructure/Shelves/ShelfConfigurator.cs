using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Shelf), typeof(BuildedObject))]
public class ShelfConfigurator : MonoBehaviour
{
    #region Fields

    private Shelf _shelf;
    private BuildedObject _buildedObject;
    private BuildingManager _buildingManager;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        _shelf = GetComponent<Shelf>();
        _buildedObject = GetComponent<BuildedObject>();
    }

    private void Start()
    {
        _buildingManager = FindObjectOfType<BuildingManager>();
    }

    #endregion


    #region Public Methods

    public void ShowConfigMenu()
    {
        if (UIShelfConfig.Instance == null)
        {
            Debug.LogError("UIShelfConfig.Instance is null! Make sure UIShelfConfig is initialized.");
            return;
        }
        UIShelfConfig.Instance.Show(_shelf, this);
    }

    public void DeleteShelf()
    {
        DropAllContents();
        EventBus<RemoveBuildingEvent>.Raise(new RemoveBuildingEvent { Target = gameObject });
        Destroy(gameObject);
    }

    public void StartRelocation()
    {
        if (_buildingManager == null)
        {
            Debug.LogError("BuildingManager not found in scene!");
            return;
        }

        if (PlayerServicesManager.Instance != null)
        {
            PlayerServicesManager.Instance.SetOffTotal();
            PlayerServicesManager.Instance.TurnOnMovements();
            PlayerServicesManager.Instance.TurnOnLooking();
        }
        if (CursorManager.Instance != null)
            CursorManager.Instance.HideCursor();

        _buildingManager.StartRelocatingBuildedObject(_buildedObject, OnRelocationFinished);
    }

    #endregion


    #region Private Methods

    private void OnRelocationFinished()
    {
        if (PlayerServicesManager.Instance != null)
            PlayerServicesManager.Instance.SetOnTotal();
        if (CursorManager.Instance != null)
            CursorManager.Instance.HideCursor();
    }

    private void DropAllContents()
    {
        foreach (ShelfSlot slot in GetComponentsInChildren<ShelfSlot>())
        {
            while (slot.GetKeptObjectsCount() > 0)
            {
                GameObject item = slot.TryGetItem();
                if (item != null)
                {
                    item.SetActive(true);
                    item.transform.SetParent(null);
                    if (item.TryGetComponent(out Rigidbody rb))
                    {
                        rb.isKinematic = false;
                        rb.AddForce(Random.insideUnitSphere * 2f, ForceMode.Impulse);
                    }
                    item.transform.position = transform.position + Vector3.up;
                }
            }
        }
    }

    #endregion
}