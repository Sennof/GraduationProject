using UnityEngine;

[CreateAssetMenu(fileName = "InGameBaseData", menuName = "InGameBaseData", order = 10)]
public class InGameBaseData : UIBaseData
{
    [Header("InGame Base")]
    [Tooltip("Prefab used when spawning the object.")]
    public GameObject Prefab;

    [Tooltip("Name of the spawned object.")]
    public string ObjectName;

    [Tooltip("Size category of the object.")]
    public ObjectSizeEnum Size;
}