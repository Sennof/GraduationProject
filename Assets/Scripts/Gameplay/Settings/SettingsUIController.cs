using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUIController : MonoBehaviour
{
    #region Fields
    [Header("References")]
    [SerializeField] private SettingsConfigurationSO _settingsSO;
    [SerializeField] private SettingsApplier _applier;

    [Header("Graphics UI")]
    [SerializeField] private bool _initializingGraphics = true;
    [SerializeField] private TMP_Dropdown _resDropdown;
    [SerializeField] private TMP_Dropdown _shadowsDropdown;
    [SerializeField] private TMP_Dropdown _texturesDropdown;
    [SerializeField] private TMP_Dropdown _renderDistDropdown;
    [SerializeField] private Slider _brightnessSlider;

    [Header("Audio UI")]
    [SerializeField] private bool _initializingAudio = true;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private Slider _vfxSlider;

    [Header("Special")]
    [SerializeField] private bool _initializingSpecial = true;
    [SerializeField] private Toggle _cheatsToggle;
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        InitializeUIValues();
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }
    #endregion

    #region Subscription Logic
    private void SubscribeToEvents()
    {
        if (_initializingGraphics)
        {
            _resDropdown.onValueChanged.AddListener(HandleResolutionChange);
            _shadowsDropdown.onValueChanged.AddListener(HandleShadowsChange);
            _texturesDropdown.onValueChanged.AddListener(HandleTexturesChange);
            _renderDistDropdown.onValueChanged.AddListener(HandleRenderDistChange);
            _brightnessSlider.onValueChanged.AddListener(HandleBrightnessChange);
        }


        if (_initializingAudio)
        {
            _volumeSlider.onValueChanged.AddListener(HandleVolumeChange);
            _vfxSlider.onValueChanged.AddListener(HandleVfxChange);
        }

        if(_initializingSpecial)
            _cheatsToggle.onValueChanged.AddListener(HandleCheatsChange);
    }

    private void UnsubscribeFromEvents()
    {
        if (_initializingGraphics)
        {
            _resDropdown.onValueChanged.RemoveListener(HandleResolutionChange);
            _shadowsDropdown.onValueChanged.RemoveListener(HandleShadowsChange);
            _texturesDropdown.onValueChanged.RemoveListener(HandleTexturesChange);
            _renderDistDropdown.onValueChanged.RemoveListener(HandleRenderDistChange);
            _brightnessSlider.onValueChanged.RemoveListener(HandleBrightnessChange);
        }


        if (_initializingAudio)
        {
            _volumeSlider.onValueChanged.RemoveListener(HandleVolumeChange);
            _vfxSlider.onValueChanged.RemoveListener(HandleVfxChange);
        }

        if(_initializingSpecial)
            _cheatsToggle.onValueChanged.RemoveListener(HandleCheatsChange);
    }
    #endregion

    #region Event Handlers
    private void HandleResolutionChange(int index) => UpdateSetting(() => _settingsSO.SetResolution(index));
    private void HandleShadowsChange(int index) => UpdateSetting(() => _settingsSO.SetShadows(index));
    private void HandleTexturesChange(int index) => UpdateSetting(() => _settingsSO.SetTextures(index));
    private void HandleRenderDistChange(int index) => UpdateSetting(() => _settingsSO.SetRenderDistance(index));
    private void HandleBrightnessChange(float val) => UpdateSetting(() => _settingsSO.SetBrightness(val));
    private void HandleVolumeChange(float val) => UpdateSetting(() => _settingsSO.SetVolume(val));
    private void HandleVfxChange(float val) => UpdateSetting(() => _settingsSO.SetVFX(val));
    private void HandleCheatsChange(bool state) => UpdateSetting(() => _settingsSO.SetCheatsState(state));

    private void UpdateSetting(System.Action settingAction)
    {
        settingAction.Invoke();
        _applier.ApplyAllSettings();
    }
    #endregion

    #region UI Initialization
    private void InitializeUIValues()
    {
        if (_initializingGraphics)
        {
            _resDropdown.value = (int)_settingsSO.ScreenResolution;
            _shadowsDropdown.value = (int)_settingsSO.ShadowsQuality;
            _texturesDropdown.value = (int)_settingsSO.TexturesQuality;
            _renderDistDropdown.value = (int)_settingsSO.RenderDistance;
            _brightnessSlider.value = _settingsSO.Brightness;
        }


        if (_initializingAudio)
        {
            _volumeSlider.value = _settingsSO.VolumeLevel;
            _vfxSlider.value = _settingsSO.VfxLevel;
        }

        if(_initializingSpecial)
            _cheatsToggle.isOn = _settingsSO.IsCheatsEnabled;
    }
    #endregion
}
