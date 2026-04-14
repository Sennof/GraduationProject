using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIWorkerCard : MonoBehaviour, IInitializable
{
    #region Fields

    [Header("References")]
    [Tooltip("Worker info window.")]
    [SerializeField] private WorkerInfoWindow _infoWindow;

    [Header("Data")]
    [Tooltip("Worker data.")]
    [SerializeField] private WorkerData _data;

    [Header("UI Elements")]
    [Tooltip("Text for worker title.")]
    [SerializeField] private TMP_Text _titleText;
    [Tooltip("Image for worker icon.")]
    [SerializeField] private Image _icon;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        if (_data == null)
        {
            return;
        }

        _titleText.text = _data.TitleName;
        _icon.sprite = _data.Icon;
    }

    public void SetInfo() => _infoWindow.SetData(_data);

    #endregion
}