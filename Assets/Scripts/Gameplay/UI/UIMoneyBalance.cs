using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMoneyBalance : MonoBehaviour
{
    #region Fields

    [Header("UI Elements")]
    [Tooltip("List of text bars for money display.")]
    [SerializeField] private List<TMP_Text> _moneyBars;

    #endregion


    #region Public Methods

    public void SetMoneyUI(int value)
    {
        foreach (TMP_Text bar in _moneyBars)
        {
            bar.text = value.ToString();
        }
    }

    #endregion
}