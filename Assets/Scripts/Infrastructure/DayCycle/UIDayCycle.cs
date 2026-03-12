using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDayCycle : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> _uiBars = new();

    public void UpdateText(int mins, int secs)
    {
        string secsText = secs.ToString();  
        if (secs < 10) secsText = $"0{secs}";

        foreach (TMP_Text bar in _uiBars)
        {
            bar.text = $"{mins+10}:{secsText}";
        }
    }
}
