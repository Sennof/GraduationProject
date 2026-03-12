using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIBuildingMode : MonoBehaviour
{
    [SerializeField] private HomoObjectSwitcher _sideWindowSwitcher;

    [SerializeField] private TMP_Text _disabledModeText;
    [SerializeField] private TMP_Text _enabledModeText;
    [SerializeField] private TMP_Text _amountLeftText;

    private KeyCode _buildingModeKeyCode;
    private KeyCode _rotationKeyCode;
    private KeyCode _buildKeyCode;

    private int _amountLeft = 1;

    public void Initialize(KeyCode buildingMode, KeyCode rotation, KeyCode build)
    {
        _buildingModeKeyCode = buildingMode;
        _rotationKeyCode = rotation;
        _buildKeyCode = build;
    }

    public void TurnOffUI()
    {
        _sideWindowSwitcher.OffAll();
    }

    public void SetUI(int id, int amountLeft)
    {
        if(amountLeft != -1)
            _amountLeft = amountLeft;

        _sideWindowSwitcher.OffCurrent();
        _sideWindowSwitcher.SetOn(id);

        if(id == 0)
        {
            _disabledModeText.text = $"Режим строительства: {_buildingModeKeyCode}";
        }
        else 
        {
            _enabledModeText.text = $"Поставить: {_buildKeyCode}" +
                $"\nПоворот: {_rotationKeyCode}" +
                $"\nПерестать строить: {_buildingModeKeyCode}";
            _amountLeftText.text = $"Осталось объектов: {_amountLeft}";
        }
    }
}
