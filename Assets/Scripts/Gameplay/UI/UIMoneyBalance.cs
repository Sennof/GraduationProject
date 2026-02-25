using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMoneyBalance : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> _moneyBars;

    public void SetMoneyUI(int value)
    {
        foreach(TMP_Text bar in _moneyBars)
        {
            bar.text = value.ToString();
        }
    }
}
