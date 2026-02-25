using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour, IInitializable
{
    #region Variables
    [SerializeField] private Color _defaultFrameColor;

    [SerializeField] private List<Image> _frameImages;
    [SerializeField] private List<Image> _iconImages;
    #endregion 

    // Sequence of methods:
    // 1. Checking if any data missing
    // 2. Settings the slots' frame color to default (unselected)
    // 3. Selecting default slot (0)
    public void Initialize()
    {
        if (_frameImages.Count != 2 || _iconImages.Count != 2 || _defaultFrameColor == null)
            Debug.LogError($"Some necessary data is missing. Instability is possible | {name}");

        foreach (Image image in _frameImages)
        {
            image.color = _defaultFrameColor;
        }

        SelectSlot(0);
    }

    // Sets the sprite to the icon's image
    public void SetIcon(Sprite sprite, int index)
    {
        if (index > 1 || index < 0)
        {
            Debug.LogError($"Invalid inventory slot index | {name}");
            return;
        }

        _iconImages[index].sprite = sprite;
        _iconImages[index].color = Color.white;
    }

    // Clear the icon's image
    // Then makes it transparent
    public void ClearIcon(int index)
    {
        _iconImages[index].sprite = null;
        _iconImages[index].color = new Color(0, 0, 0, 0);
    }

    // Sets the color of the frame
    // Default OR Selected
    public void SelectSlot(int index) 
    {
        int disableSlot = 1 - index;

        _frameImages[disableSlot].color = _defaultFrameColor;
        _frameImages[index].color = new Color(_defaultFrameColor.r, _defaultFrameColor.g, _defaultFrameColor.b - 35, _defaultFrameColor.a);
    }
}
