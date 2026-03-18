using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFeedbackCard : MonoBehaviour
{
    [SerializeField] private TMP_Text _feedbackText;
    [SerializeField] private Image _iconImage;

    [SerializeField] private List<Sprite> _buyersSprites = new();

    public void Intialize(string feedback)
    {
        _feedbackText.text = feedback;
        _iconImage.sprite = _buyersSprites[Random.Range(0, _buyersSprites.Count)];
    }
}
