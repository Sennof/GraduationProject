using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFeedbackCard : MonoBehaviour
{
    #region Fields

    [Header("UI Elements")]
    [Tooltip("Text for feedback.")]
    [SerializeField] private TMP_Text _feedbackText;
    [Tooltip("Image for buyer icon.")]
    [SerializeField] private Image _iconImage;

    [Header("Sprites")]
    [Tooltip("List of possible buyer sprites.")]
    [SerializeField] private List<Sprite> _buyersSprites = new();

    #endregion


    #region Public Methods

    public void Intialize(string feedback)
    {
        _feedbackText.text = feedback;
        _iconImage.sprite = _buyersSprites[Random.Range(0, _buyersSprites.Count)];
    }

    #endregion
}