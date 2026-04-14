using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIPricing : MonoBehaviour, IInitializable
{
    #region Fields

    [Header("UI Elements")]
    [Tooltip("Text displaying pricing percentage.")]
    [SerializeField] private TMP_Text _pricingText;
    [Tooltip("Slider for pricing modifier.")]
    [SerializeField] private Slider _slider;

    private EventBinding<OnShopStateChanging> _shopStateBinding = null;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        _shopStateBinding = new EventBinding<OnShopStateChanging>(HandleShopStateChange);
        EventBus<OnShopStateChanging>.Register(_shopStateBinding);
    }

    public void ApplyChanges()
    {
        float value = _slider.value;
        _pricingText.text = (Mathf.Round(value * 100)).ToString() + "%";
        GlobalStatsBridge.Instance.SetPricingMod(value + 1);
    }

    public void LoadUI()
    {
        float value = GlobalStatsBridge.Instance.GetPricingMod() - 1;
        _slider.value = value;
        _pricingText.text = (Mathf.Round(value * 100)).ToString() + "%";
    }

    #endregion


    #region Event Handlers

    private void HandleShopStateChange(OnShopStateChanging eventData)
    {
        if (eventData.IsOpen)
        {
            _slider.interactable = false;
        }
        else
        {
            _slider.interactable = true;
        }
    }

    #endregion


    #region Unity Methods

    private void OnDisable()
    {
        EventBus<OnShopStateChanging>.Deregister(_shopStateBinding);
    }

    #endregion
}