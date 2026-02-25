using UnityEngine;

[CreateAssetMenu(fileName = "InGameBaseData", menuName = "InGameBaseData", order = 10)]
public class InGameBaseData : UIBaseData
{
    [Header("InGame Base")]
    [Tooltip("When spawning an object it is used")]
    public GameObject Prefab;
    [Tooltip("The name of spawned object")]
    public string ObjectName;
}
