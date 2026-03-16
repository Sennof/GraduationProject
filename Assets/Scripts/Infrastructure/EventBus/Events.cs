using System.Text;
using UnityEngine;

public interface IEvent { }

#region delivery
public struct DeliveryShopOnClickEvent : IEvent
{
    public ProductData ProductData;
}

public struct DeliveryRequestingEvent : IEvent
{
    public int Amount;
    public ProductData ProductData;
}
#endregion

#region building
public struct BuildingModeTriggerEvent : IEvent 
{
    public Transform TargetFolder;
}

public struct RemoveBuildingEvent : IEvent
{
    public GameObject Target;
}
#endregion

#region initializers
public struct ShelfDataRequestingEvent : IEvent
{
    public GameObject Target;
}

public struct ShelfDataResponsingEvent : IEvent
{
    public GameObject Target;
    public Inventory Inventory;
}

public struct PenKnifeDataRequestingEvent : IEvent
{
    public GameObject Target;
}

public struct PenKnifeResponsingEvent : IEvent
{
    public GameObject Target;
    public Transform RaycastFolder;
}

public struct BuildingWrenchRequestingEvent : IEvent
{
    public GameObject Target;
}

public struct BuildingWrenchResponsingEvent : IEvent
{
    public GameObject Target;
    public Transform RaycastFolder;
}
#endregion

#region buying
public struct ShelfVisitPointTranslatingEvent : IEvent
{
    public GameObject _target;
}
#endregion

#region DayCycle
public struct OnDayStateChangeEvent : IEvent
{
    public bool isDay;
}
#endregion

#region Buying & buyers
public struct OnShelfInitializationEvent : IEvent
{
    public Vector3 GlobalPosition;
    public bool Adding;

    public Shelf Shelf;
}

public struct PaymentRequestEvent : IEvent
{
    public GameObject[] Products;
}

public struct UIPaymentCardOperation : IEvent
{
    public bool isPlus;
    public int Price;
}

public struct PaymentResponseEvent : IEvent {}
#endregion

#region else
public struct UnpackingEvent : IEvent
{
    public float Distance;
}

public struct ItemPickUpEvent : IEvent
{
    public ItemObject ItemObjectData;
    public GameObject ItemGameObject;
}

public struct OnShopStateChanging : IEvent
{
    public bool isOpen;
}
#endregion