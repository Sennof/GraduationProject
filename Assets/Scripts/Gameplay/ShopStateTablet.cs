using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class ShopStateTablet : MonoBehaviour, IInitializeable
{
    [SerializeField] private bool _canToggle = true;

    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private Color _openColor;
    [SerializeField] private Color _closedColor;

    private EventBinding<OnDayStateChangeEvent> _binding;
    private Coroutine _animCor = null;

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
        if (_canToggle == false) return;

        bool state = !GlobalStatsBridge.Instance.GetShopOpenClosed();
        if(state) OpenShop();
        else CloseShop();

        if(_animCor != null)
        {
            StopCoroutine(_animCor);
            _animCor = null; 
        }
        _animCor = StartCoroutine(AnimCooldownRoutine());
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
        if(_animCor != null)
        {
            StopCoroutine(_animCor);
            _animCor = null;
        }

        if (!eventData.isDay)
        {
            CloseShop();
            _canToggle = false;
        }
        else _canToggle = true;
    }

    private IEnumerator AnimCooldownRoutine()
    {
        _canToggle = false;
        transform.DOMoveY(transform.position.y + 0.25f, 0.25f);
        yield return new WaitForSeconds(0.27f);
        transform.DOMoveY(transform.position.y - 0.25f, 0.25f);
        yield return new WaitForSeconds(0.26f);
        _canToggle = true;
    }
}
