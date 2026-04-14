using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackedBoxLayout : MonoBehaviour
{
    #region Fields

    [Header("UI")]
    [Tooltip("List of icon images on the box.")]
    [SerializeField] private List<Image> _boxIcons = new();

    #endregion


    #region Public Methods

    public void Initialize(Sprite icon)
    {
        foreach (Image img in _boxIcons)
        {
            img.sprite = icon;
        }
    }

    #endregion
}