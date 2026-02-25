using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIWorkerCard : MonoBehaviour, IInitializable
{
    [SerializeField] private WorkerInfoWindow _infoWindow;

    [SerializeField] private WorkerData _data;

    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private Image _icon;

    public void Initialize()
    {
        if (_data == null)
            return;

        _titleText.text = _data.TitleName;
        _icon.sprite = _data.Icon;
    }

    public void SetInfo() => _infoWindow.SetData(_data);
}
