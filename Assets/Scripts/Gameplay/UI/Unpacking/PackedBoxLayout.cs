using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackedBoxLayout : MonoBehaviour
{
    [SerializeField] private List<Image> _boxIcons = new();

    public void Initialize(Sprite icon)
    {
         foreach(Image img in _boxIcons)
            img.sprite = icon;
    }
}
