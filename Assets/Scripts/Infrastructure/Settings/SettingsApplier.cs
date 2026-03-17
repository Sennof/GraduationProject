using UnityEngine;
using UnityEngine.Audio;

public class SettingsApplier : MonoBehaviour
{
    #region Fields
    [Header("Configuration")]
    [SerializeField] private SettingsConfigurationSO _settings;

    [Header("Audio Resources")]
    [SerializeField] private AudioMixer _mainMixer;
    [SerializeField] private string _volumeParameter = "MasterVolume";
    [SerializeField] private string _vfxParameter = "VFXVolume";

    [Header("Brightness Reference")]
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
        switch (_settings.TexturesQuality)
        {
            case QualityEnum.Low: QualitySettings.globalTextureMipmapLimit = 2; break;
            case QualityEnum.Medium: QualitySettings.globalTextureMipmapLimit = 1; break;
            case QualityEnum.High: QualitySettings.globalTextureMipmapLimit = 0; break;
        }

        switch (_settings.ShadowsQuality)
        {
            case QualityEnum.Low: QualitySettings.shadowResolution = ShadowResolution.Low; QualitySettings.shadowDistance = 20f; break;
            case QualityEnum.Medium: QualitySettings.shadowResolution = ShadowResolution.Medium; QualitySettings.shadowDistance = 50f; break;
            case QualityEnum.High: QualitySettings.shadowResolution = ShadowResolution.VeryHigh; QualitySettings.shadowDistance = 100f; break;
        }

        if (_brightnessOverlay != null)
        {
            Color color = _brightnessOverlay.color;
            color.a = 1f - _settings.Brightness;
            _brightnessOverlay.color = color;
        }
    }

    private void ApplyAudio()
    {
        if (_mainMixer != null)
        {
            _mainMixer.SetFloat(_volumeParameter, Mathf.Log10(Mathf.Clamp(_settings.VolumeLevel, 0.0001f, 1f)) * 20);
            _mainMixer.SetFloat(_vfxParameter, Mathf.Log10(Mathf.Clamp(_settings.VfxLevel, 0.0001f, 1f)) * 20);
        }
    }

    private void ApplySpecial()
    {
        Debug.Log($"Cheats enabled status: {_settings.IsCheatsEnabled}");
    }
    #endregion
}
