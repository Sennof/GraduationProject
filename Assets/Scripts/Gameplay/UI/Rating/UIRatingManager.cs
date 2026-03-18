using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIRatingManager : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> _bars = new();
     
    public void SetText(float value)
    {
        string text = "";
        if (value != 5) text = value.ToString();
        else text = "макс.";

        foreach (TMP_Text bar in _bars) bar.text = text;
    }
}
