using UnityEngine;

[RequireComponent(typeof(ItemObject))]
public class BuildingObject : MonoBehaviour
{
    #region Fields

    [Header("Settings")]
    [Tooltip("Determines if the object can be built.")]
    [SerializeField] private bool _canBuild;
    [Tooltip("Number of builds remaining.")]
    [SerializeField] private int _amount = 1;

    [Header("Prefab")]
    [Tooltip("Prefab that will be instantiated when building.")]
    [SerializeField] private GameObject _prefab;

    private Transform _targetFolder;

    #endregion


    #region Public Methods

    public void Initialize(Transform folder)
    {
        _targetFolder = folder;
    }

    public void SetInHands()
    {
        EventBus<BuildingModeTriggerEvent>.Raise(new BuildingModeTriggerEvent { TargetFolder = _targetFolder });
    }

    public void SetOutHands()
    {
        EventBus<BuildingModeTriggerEvent>.Raise(new BuildingModeTriggerEvent { TargetFolder = null });
    }

    public GameObject GetPrefab() => _prefab;

    public void DecreaseAmount() => _amount -= 1;

    public int GetAmount() => _amount;

    #endregion
}