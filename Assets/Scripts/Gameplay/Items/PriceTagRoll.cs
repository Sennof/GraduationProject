using UnityEngine;

public class PriceTagRoll : MonoBehaviour
{
    #region Fields

    [Header("Settings")]
    [Tooltip("Number of blank tags this roll adds to a PriceTagMaker.")]
    [SerializeField] private int _tagsCount = 20;
    [Tooltip("Key to use the roll on the PriceTagMaker held in the other inventory slot.")]
    [SerializeField] private KeyCode _useKey = KeyCode.F;

    private Inventory _inventory;

    #endregion


    #region Public Methods

    public int GetTagsCount() => _tagsCount;

    #endregion


    #region Private Methods

    private Inventory GetInventory()
    {
        if (_inventory == null)
            _inventory = FindFirstObjectByType<Inventory>();
        return _inventory;
    }

    private bool IsInHands()
    {
        Inventory inv = GetInventory();
        return inv != null && inv.GetCurrentItemManager()?.gameObject == gameObject;
    }

    private void TryRefillMaker()
    {
        Inventory inv = GetInventory();
        if (inv == null) return;

        ItemObject otherItem = inv.GetOtherSlotItemManager();
        if (otherItem == null) return;

        PriceTagMaker maker = otherItem.GetComponent<PriceTagMaker>();
        if (maker == null) return;

        maker.RefillCapacity(_tagsCount);
        inv.DestroySlot();
    }

    #endregion


    #region Unity Methods

    private void Update()
    {
        if (!IsInHands()) return;

        if (Input.GetKeyDown(_useKey))
            TryRefillMaker();
    }

    #endregion
}
