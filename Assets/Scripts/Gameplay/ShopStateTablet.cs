using TMPro;
using UnityEngine;

public class ShopStateTablet : MonoBehaviour, IInitializeable
{
    [SerializeField] private bool _canToggle = true;

    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private Color _openColor;
    [SerializeField] private Color _closedColor;

    private EventBinding<OnDayStateChangeEvent> _binding;

    public void Initialize()
    {
        _binding = new EventBinding<OnDayStateChangeEvent>(HandleDayStateChange);
        EventBus<OnDayStateChangeEvent>.Register(_binding);

        CloseShop();
    }

    private void OnDisable()
    {
        EventBus<OnDayStateChangeEvent>.Deregister(_binding);
    }

    public void ChangeState()
    {
        if (!_canToggle) return;

        bool state = !GlobalStatsBridge.Instance.GetShopOpenClosed();
        if(state) OpenShop();
        else CloseShop();
    }

    private void OpenShop()
    {
        _titleText.text = "Открыто";
        _titleText.color = _openColor;

        GlobalStatsBridge.Instance.SetShopOpenClosed(true);

        EventBus<OnShopStateChanging>.Raise(new OnShopStateChanging { isOpen = true });
    }

    private void CloseShop()
    {
        _titleText.text = "Закрыто";
        _titleText.color = _closedColor;

        GlobalStatsBridge.Instance.SetShopOpenClosed(false);

        EventBus<OnShopStateChanging>.Raise(new OnShopStateChanging { isOpen = false });
    }

    private void HandleDayStateChange(OnDayStateChangeEvent eventData)
    {
        if (!eventData.isDay)
        {
            CloseShop();
            _canToggle = false;
        }
        else _canToggle = true;
    }
}
