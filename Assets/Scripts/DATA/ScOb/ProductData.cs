using UnityEngine;

[CreateAssetMenu(fileName = "UIProductData", menuName = "BaseInGameData/UIProductData", order = 10)]
public class ProductData : InGameBaseData
{
    #region Fields

    [Header("ProductData")]
    [Tooltip("Base price of the product (without markup).")]
    public int Price;

    [Tooltip("Default markup applied to this product (0.2 = +20%).")]
    [Range(0f, 2f)]
    public float DefaultMarkup = 0.15f;

    #endregion


    #region Public Methods

    /// <summary>
    /// Returns the current selling price with applied markup.
    /// </summary>
    public int GetPriceWithMarkup()
    {
        float markup = GlobalStatsBridge.Instance.GetProductMarkup(TitleName);
        return Mathf.RoundToInt(Price * (1f + markup));
    }

    #endregion
}