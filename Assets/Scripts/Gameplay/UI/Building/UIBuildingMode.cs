using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIBuildingMode : MonoBehaviour
{
    #region Fields

    [Header("References")]
    [Tooltip("Switcher for side window.")]
    [SerializeField] private HomoObjectSwitcher _sideWindowSwitcher;

    [Header("UI Text")]
    [Tooltip("Text for disabled mode.")]
    [SerializeField] private TMP_Text _disabledModeText;
    [Tooltip("Text for enabled mode.")]
    [SerializeField] private TMP_Text _enabledModeText;
    [Tooltip("Text showing amount left.")]
    [SerializeField] private TMP_Text _amountLeftText;

    private KeyCode _buildingModeKeyCode;
    private KeyCode _rotationKeyCode;
    private KeyCode _buildKeyCode;
    private int _amountLeft = 1;

    #endregion


    #region Public Methods

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
        if (amountLeft != -1)
        {
            _amountLeft = amountLeft;
        }

        _sideWindowSwitcher.OffCurrent();
        _sideWindowSwitcher.SetOn(id);

        if (id == 0)
        {
            _disabledModeText.text = $"Build Mode: {_buildingModeKeyCode}";
        }
        else
        {
            _enabledModeText.text = $"Place: {_buildKeyCode}\nRotate: {_rotationKeyCode}\nExit Build: {_buildingModeKeyCode}";
            _amountLeftText.text = $"Objects left: {_amountLeft}";
        }
    }

    #endregion
}