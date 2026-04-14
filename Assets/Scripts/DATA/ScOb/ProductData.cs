using UnityEngine;

[CreateAssetMenu(fileName = "UIProductData", menuName = "BaseInGameData/UIProductData", order = 10)]
public class ProductData : InGameBaseData
{
    #region ProductData

    [Header("ProductData")]
    [Tooltip("Base price of the product.")]
    public int Price;

    #endregion
}