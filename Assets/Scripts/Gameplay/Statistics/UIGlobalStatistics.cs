using TMPro;
using UnityEngine;

public class UIGlobalStatistics : MonoBehaviour
{
    #region Fields

    [Header("UI Elements")]
    [Tooltip("Main text for statistics.")]
    [SerializeField] private TMP_Text _mainText;

    #endregion


    #region Public Methods

    public void SetUI()
    {
        _mainText.text = $"Total visitors: {GlobalStatsBridge.Instance.GetTotalBuyers()}\n" +
            $"Total products sold: {GlobalStatsBridge.Instance.GetTotalProducts()}\n\n" +
            $"Total deliveries: {GlobalStatsBridge.Instance.GetTotalDeliveries()}\n\n" +
            $"Largest receipt: {GlobalStatsBridge.Instance.GetMaxEarned()}\n" +
            $"Total earned: {GlobalStatsBridge.Instance.GetTotalEarned()}\n" +
            $"Total spent: {GlobalStatsBridge.Instance.GetTotalSpent()}";
    }

    #endregion
}