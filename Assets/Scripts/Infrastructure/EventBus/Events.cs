using System.Text;
using UnityEngine;

public interface IEvent { }

#region Delivery

public struct DeliveryShopOnClickEvent : IEvent
{
    public ProductData ProductData;
}

public struct DeliveryRequestingEvent : IEvent
{
    public int Amount;
    public ProductData ProductData;
}

public struct DeliveryResponseEvent : IEvent
{
    public bool IsSuccess;
}

#endregion

#region Building

public struct BuildingModeTriggerEvent : IEvent
{
    public Transform TargetFolder;
}

public struct RemoveBuildingEvent : IEvent
{
    public GameObject Target;
}

#endregion

#region Initializers

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

#region Buying

public struct ShelfVisitPointTranslatingEvent : IEvent
{
    public GameObject Target;
}

#endregion

#region DayCycle

public struct OnDayStateChangeEvent : IEvent
{
    public bool IsDay;
}

#endregion

#region BuyingAndBuyers

public struct OnShelfInitializationEvent : IEvent
{
    public Vector3 GlobalPosition;
    public bool Adding;
    public Shelf Shelf;
}

public struct PaymentRequestEvent : IEvent
{
    public GameObject[] Products;
    public AICustomer Customer; // Added to track which customer is paying
}

public struct UIPaymentCardOperation : IEvent
{
    public bool IsPlus;
    public int Price;
}

public struct PaymentResponseEvent : IEvent { }

#endregion

#region PriceTags

public struct PriceTagMakerDataRequestingEvent : IEvent
{
    public GameObject Target;
}

public struct PriceTagMakerDataResponsingEvent : IEvent
{
    public GameObject Target;
    public Transform RaycastStartPoint;
    public Inventory Inventory;
}

public struct CreatePriceTagsRequestEvent : IEvent
{
    public ProductData ProductData;
    public float Markup;
    public int Quantity;
    public PriceTagMaker TargetMaker;
}

#endregion

#region Else

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
    public bool IsOpen;
}

public struct OnRatingLevelChange : IEvent
{
    public LevelsEnum Level;
}

#endregion