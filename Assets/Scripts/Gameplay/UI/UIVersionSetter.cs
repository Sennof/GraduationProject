using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIVersionSetter : MonoBehaviour, IInitializeable
{
    [SerializeField] private List<TMP_Text> _texts = new();

    public void Initialize()
    {
        SetVersion();
    }

    public void SetVersion()
    {
        foreach (TMP_Text text in _texts)
            text.text = $"ver: {Application.version}";
    }
}
