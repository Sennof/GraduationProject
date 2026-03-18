using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class UIGlobalStatistics : MonoBehaviour
{
    [SerializeField] private TMP_Text _mainText;

    public void SetUI()
    {
        _mainText.text = $"Всего посетителей: {GlobalStatsBridge.Instance.GetTotalBuyers()}\n" +
            $"Всего товаров продано: {GlobalStatsBridge.Instance.GetTotalProducts()}\n\n" +
            $"Всего доставок: {GlobalStatsBridge.Instance.GetTotalDeliveries()}\n\n" +
            $"Самый крупный чек: {GlobalStatsBridge.Instance.GetMaxEarned()}\n" +
            $"Всего заработано: {GlobalStatsBridge.Instance.GetTotalEarned()}\n" +
            $"Всего потрачено: {GlobalStatsBridge.Instance.GetTotalSpent()}";
    }
}
