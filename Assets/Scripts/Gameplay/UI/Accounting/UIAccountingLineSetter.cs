using TMPro;
using UnityEngine;

public class UIAccountingLineSetter : MonoBehaviour
{
    #region Fields

    [Header("UI Elements")]
    [Tooltip("Text for the line title.")]
    [SerializeField] private TMP_Text _title;
    [Tooltip("Text for the line price.")]
    [SerializeField] private TMP_Text _price;

    #endregion


    #region Public Methods

    public void SetData(string title, int price)
    {
        _title.text = title;
        _price.text = price.ToString();
    }

    #endregion
}