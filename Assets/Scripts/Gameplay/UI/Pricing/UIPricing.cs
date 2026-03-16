using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIPricing : MonoBehaviour
{
    [SerializeField] private TMP_Text _pricingText;
    [SerializeField] private Slider _slider;

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
}
