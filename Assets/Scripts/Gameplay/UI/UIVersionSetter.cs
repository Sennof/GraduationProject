using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIVersionSetter : MonoBehaviour, IInitializeable
{
    #region Fields

    [Header("UI Elements")]
    [Tooltip("List of text elements to display version.")]
    [SerializeField] private List<TMP_Text> _texts = new();

    #endregion


    #region Public Methods

    public void Initialize()
    {
        SetVersion();
    }

    public void SetVersion()
    {
        foreach (TMP_Text text in _texts)
        {
            text.text = $"ver: {Application.version}";
        }
    }

    #endregion
}