using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIPricing : MonoBehaviour, IInitializable
{
    [SerializeField] private TMP_Text _pricingText;
    [SerializeField] private Slider _slider;

    private EventBinding<OnShopStateChanging> _binding = null;

    public void Initialize()
    {
        _binding = new EventBinding<OnShopStateChanging>(HandleShopStateChange);
        EventBus<OnShopStateChanging>.Register(_binding);
    }

    private void OnDisable()
    {
        EventBus<OnShopStateChanging>.Deregister(_binding);
    }

    public void ApplyChanges()
    {
        float value = _slider.value;
        _pricingText.text = (Mathf.Round(value*100)).ToString() + "%";
        GlobalStatsBridge.Instance.SetPricingMod(value + 1);
    }

    public void LoadUI()
    {
        float value = GlobalStatsBridge.Instance.GetPricingMod() - 1;

        _slider.value = value;
        _pricingText.text = (Mathf.Round(value*100)).ToString() + "%";
    }

    private void HandleShopStateChange(OnShopStateChanging eventData)
    {
        if (eventData.isOpen) _slider.interactable = false;
        else _slider.interactable = true;
    }
}
