using UnityEngine;

public interface IEvent { }

public struct ItemPickUpEvent : IEvent 
{
    public ItemObject ItemObjectData;
    public GameObject ItemGameObject;
}