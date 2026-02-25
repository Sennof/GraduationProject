using TMPro;
using UnityEngine;

public class UIAccountingLineSetter : MonoBehaviour
{
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _price;

    public void SetData(string title, int price)
    {
        _title.text = title;
        _price.text = price.ToString();
    }
}
