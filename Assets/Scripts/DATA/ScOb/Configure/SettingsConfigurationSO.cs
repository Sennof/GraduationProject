using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsConfiguration", menuName = "Configuration/Settings")]
public class SettingsConfigurationSO : ScriptableObject
{
    #region Graphics Fields

    [Header("Graphics")]
    [Tooltip("Enable fullscreen mode.")]
    [SerializeField] private bool _fullscreenMode = true;

    [Tooltip("Screen resolution.")]
    [SerializeField] private ScreenResolutionEnum _screenResolution = ScreenResolutionEnum.Res_1920x1080;

    [Tooltip("Shadows quality level.")]
    [SerializeField] private QualityEnum _shadowsQuality = QualityEnum.High;

    [Tooltip("Textures quality level.")]
    [SerializeField] private QualityEnum _texturesQuality = QualityEnum.High;

    [Tooltip("Render distance quality.")]
    [SerializeField] private QualityEnum _renderDistance = QualityEnum.High;

    [Tooltip("Screen brightness (0 = dark, 1 = bright).")]
    [Range(0, 1)] private float _brightness = 1;

    #endregion

    #region Audio Fields

    [Space(15)]
    [Header("Audio")]
    [Tooltip("Master volume level.")]
    [SerializeField, Range(0, 1)] private float _volumeLevel = 1f;

    [Tooltip("VFX volume level.")]
    [SerializeField, Range(0, 1)] private float _vfxLevel = 1f;

    #endregion

    #region Controls Fields

    [Space(15)]
    [Header("Controls (Fixed)")]
    [ReadOnly, SerializeField] private KeyCode _forwardKey = KeyCode.W;
    [ReadOnly, SerializeField] private KeyCode _leftKey = KeyCode.A;
    [ReadOnly, SerializeField] private KeyCode _backwardKey = KeyCode.S;
    [ReadOnly, SerializeField] private KeyCode _rightKey = KeyCode.D;
    [ReadOnly, SerializeField] private KeyCode _itemActionKey = KeyCode.Mouse0;

    [Space(15)]
    [Header("Controls (Editable)")]
    [SerializeField] private KeyCode _mainActionKey = KeyCode.E;
    [SerializeField] private KeyCode _sideActionKey = KeyCode.F;
    [SerializeField] private KeyCode _throwKey = KeyCode.G;
    [SerializeField] private KeyCode _dropKey = KeyCode.Z;
    [SerializeField] private KeyCode _inventoryFSlotKey = KeyCode.Alpha1;
    [SerializeField] private KeyCode _inventorySSlotKey = KeyCode.Alpha2;
    [SerializeField] private KeyCode _buildingModeToggleKey = KeyCode.B;
    [SerializeField] private KeyCode _buildingRotationKey = KeyCode.R;
    [SerializeField] private KeyCode _buildingPlaceKey = KeyCode.P;

    #endregion

    #region Special Fields

    [Space(15)]
    [Header("Special")]
    [Tooltip("Enable cheat functionality.")]
    [SerializeField] private bool _isCheatsEnabled = false;

    #endregion

    #region Properties (Getters)

    public bool FullscreenMode => _fullscreenMode;
    public ScreenResolutionEnum ScreenResolution => _screenResolution;
    public QualityEnum ShadowsQuality => _shadowsQuality;
    public QualityEnum TexturesQuality => _texturesQuality;
    public QualityEnum RenderDistance => _renderDistance;
    public float Brightness => _brightness;
    public float VolumeLevel => _volumeLevel;
    public float VfxLevel => _vfxLevel;
    public bool IsCheatsEnabled => _isCheatsEnabled;

    #endregion

    #region Setters (Public)

    public void ToggleFullscreen(bool state) => _fullscreenMode = state;

    public void SetResolution(int index)
    {
        if (index == (int)ScreenResolutionEnum.Unknown) return;
        _screenResolution = (ScreenResolutionEnum)index;
    }

    public void SetShadows(int index) => _shadowsQuality = (QualityEnum)index;
    public void SetTextures(int index) => _texturesQuality = (QualityEnum)index;
    public void SetRenderDistance(int index) => _renderDistance = (QualityEnum)index;
    public void SetBrightness(float val) => _brightness = val;
    public void SetVolume(float val) => _volumeLevel = val;
    public void SetVFX(float val) => _vfxLevel = val;
    public void SetCheatsState(bool state) => _isCheatsEnabled = state;

    #endregion
}