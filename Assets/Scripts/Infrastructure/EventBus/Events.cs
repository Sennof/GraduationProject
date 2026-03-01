using UnityEngine;

public interface IEvent { }

public struct ItemPickUpEvent : IEvent 
{
    public ItemObject ItemObjectData;
    public GameObject ItemGameObject;
}

public struct DeliveryShopOnClickEvent : IEvent
{
    public ProductData ProductData;
}

public struct DeliveryRequestingEvent : IEvent
{
    public int Amount;
    public ProductData ProductData;
}

public struct UnpackingEvent : IEvent
{
    public float Distance;
}