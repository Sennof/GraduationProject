using UnityEngine;
using UnityEngine.Audio;

public class SettingsApplier : MonoBehaviour
{
    #region Fields

    [Header("Configuration")]
    [Tooltip("Settings configuration asset.")]
    [SerializeField] private SettingsConfigurationSO _settings;

    [Header("Audio Resources")]
    [Tooltip("Main audio mixer.")]
    [SerializeField] private AudioMixer _mainMixer;
    [Tooltip("Exposed parameter name for master volume.")]
    [SerializeField] private string _volumeParameter = "MasterVolume";
    [Tooltip("Exposed parameter name for VFX volume.")]
    [SerializeField] private string _vfxParameter = "VFXVolume";

    [Header("Brightness Reference")]
    [Tooltip("UI Image overlay for brightness control.")]
    [SerializeField] private UnityEngine.UI.Image _brightnessOverlay;

    #endregion


    #region Unity Methods

    private void Start()
    {
        ApplyAllSettings();
    }

    #endregion


    #region Public Methods

    public void ApplyAllSettings()
    {
        ApplyGraphics();
        ApplyAudio();
        ApplySpecial();
    }

    #endregion


    #region Private Methods

    private void ApplyGraphics()
    {
        Screen.fullScreen = _settings.FullscreenMode;

        switch (_settings.ScreenResolution)
        {
            case ScreenResolutionEnum.Res_800x600: Screen.SetResolution(800, 600, _settings.FullscreenMode); break;
            case ScreenResolutionEnum.Res_1536x864: Screen.SetResolution(1536, 864, _settings.FullscreenMode); break;
            case ScreenResolutionEnum.Res_1280x720: Screen.SetResolution(1280, 720, _settings.FullscreenMode); break;
            case ScreenResolutionEnum.Res_1920x1080: Screen.SetResolution(1920, 1080, _settings.FullscreenMode); break;
        }

        switch (_settings.TexturesQuality)
        {
            case QualityEnum.Low: QualitySettings.globalTextureMipmapLimit = 2; break;
            case QualityEnum.Medium: QualitySettings.globalTextureMipmapLimit = 1; break;
            case QualityEnum.High: QualitySettings.globalTextureMipmapLimit = 0; break;
        }

        switch (_settings.ShadowsQuality)
        {
            case QualityEnum.Low:
                QualitySettings.shadowResolution = ShadowResolution.Low;
                QualitySettings.shadowDistance = 20f;
                break;
            case QualityEnum.Medium:
                QualitySettings.shadowResolution = ShadowResolution.Medium;
                QualitySettings.shadowDistance = 50f;
                break;
            case QualityEnum.High:
                QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                QualitySettings.shadowDistance = 100f;
                break;
        }

        if (_brightnessOverlay != null)
        {
            Color color = _brightnessOverlay.color;
            color.a = 1f - _settings.Brightness;
            _brightnessOverlay.color = color;
        }

        Debug.Log($"[GRAPHICS SETTINGS LOG]\nFullScreen: {_settings.FullscreenMode}\nResolution: {_settings.ScreenResolution}\nTextures: {_settings.TexturesQuality}\nShadows: {_settings.ShadowsQuality}\nBrightness: {1 - _settings.Brightness}");
    }

    private void ApplyAudio()
    {
        if (_mainMixer != null)
        {
            _mainMixer.SetFloat(_volumeParameter, Mathf.Log10(Mathf.Clamp(_settings.VolumeLevel, 0.0001f, 1f)) * 20);
            _mainMixer.SetFloat(_vfxParameter, Mathf.Log10(Mathf.Clamp(_settings.VfxLevel, 0.0001f, 1f)) * 20);
        }

        Debug.Log($"[AUDIO SETTINGS LOG]\nMAIN: {_settings.VolumeLevel}\nVFX: {_settings.VfxLevel}");
    }

    private void ApplySpecial()
    {
        Debug.Log($"[SPECIAL SETTINGS LOG]\nCheats: {_settings.IsCheatsEnabled}");
    }

    #endregion
}