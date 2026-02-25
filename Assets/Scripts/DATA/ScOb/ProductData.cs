using UnityEngine;

[CreateAssetMenu(fileName = "UIProductData", menuName = "BaseInGameData/UIProductData", order = 10)]
public class ProductData : InGameBaseData
{
    #region ProductData
    [Header("ProductData")]
    public int Price;
    #endregion
}
