using UnityEngine;

[RequireComponent(typeof(ItemObject))]
public class BuildingObject : MonoBehaviour
{
    [SerializeField] private bool _canBuild;
    [SerializeField] private int _amount = 1;

    [SerializeField] private GameObject _prefab;

    private Transform _targetFolder;

    public void Initialize(Transform folder)
    {
        _targetFolder = folder;
    }

    public void SetInHands()
    {
        EventBus<BuildingModeTriggerEvent>.Raise(new BuildingModeTriggerEvent { TargetFolder = _targetFolder});
    }

    public void SetOutHands()
    {
        EventBus<BuildingModeTriggerEvent>.Raise(new BuildingModeTriggerEvent { TargetFolder = null });
    }

    public GameObject GetPrefab() => _prefab;

    public void DecreaseAmount() => _amount -= 1;

    public int GetAmount() => _amount;
}
