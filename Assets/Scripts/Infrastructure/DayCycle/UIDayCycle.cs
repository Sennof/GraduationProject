using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDayCycle : MonoBehaviour
{
    #region Fields

    [Header("UI Elements")]
    [Tooltip("List of text bars for time display.")]
    [SerializeField] private List<TMP_Text> _uiBars = new();

    #endregion


    #region Public Methods

    public void UpdateText(int mins, int secs)
    {
        string secsText = secs.ToString();
        if (secs < 10)
        {
            secsText = $"0{secs}";
        }

        foreach (TMP_Text bar in _uiBars)
        {
            bar.text = $"{mins + 10}:{secsText}";
        }
    }

    #endregion
}