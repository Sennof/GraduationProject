using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class ShopStateTablet : MonoBehaviour, IInitializeable
{
    #region Fields

    [Header("State")]
    [Tooltip("Allow toggling shop state.")]
    [SerializeField] private bool _canToggle = true;

    [Header("UI")]
    [Tooltip("Text displaying shop state.")]
    [SerializeField] private TMP_Text _titleText;
    [Tooltip("Color for open state.")]
    [SerializeField] private Color _openColor;
    [Tooltip("Color for closed state.")]
    [SerializeField] private Color _closedColor;

    private EventBinding<OnDayStateChangeEvent> _dayStateBinding;
    private Coroutine _animationCoroutine = null;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        _dayStateBinding = new EventBinding<OnDayStateChangeEvent>(HandleDayStateChange);
        EventBus<OnDayStateChangeEvent>.Register(_dayStateBinding);

        CloseShop();
    }

    public void ChangeState()
    {
        if (_canToggle == false)
        {
            return;
        }

        bool state = !GlobalStatsBridge.Instance.GetShopOpenClosed();
        if (state)
        {
            OpenShop();
        }
        else
        {
            CloseShop();
        }

        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }
        _animationCoroutine = StartCoroutine(AnimCooldownRoutine());
    }

    #endregion


    #region Private Methods

    private void OpenShop()
    {
        _titleText.text = "Open";
        _titleText.color = _openColor;

        GlobalStatsBridge.Instance.SetShopOpenClosed(true);

        EventBus<OnShopStateChanging>.Raise(new OnShopStateChanging { IsOpen = true });
    }

    private void CloseShop()
    {
        _titleText.text = "Closed";
        _titleText.color = _closedColor;

        GlobalStatsBridge.Instance.SetShopOpenClosed(false);

        EventBus<OnShopStateChanging>.Raise(new OnShopStateChanging { IsOpen = false });
    }

    private void HandleDayStateChange(OnDayStateChangeEvent eventData)
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }

        if (!eventData.IsDay)
        {
            CloseShop();
            _canToggle = false;
        }
        else
        {
            _canToggle = true;
        }
    }

    #endregion


    #region Coroutines

    private IEnumerator AnimCooldownRoutine()
    {
        _canToggle = false;
        transform.DOMoveY(transform.position.y + 0.25f, 0.25f);
        yield return new WaitForSeconds(0.27f);
        transform.DOMoveY(transform.position.y - 0.25f, 0.25f);
        yield return new WaitForSeconds(0.26f);
        _canToggle = true;
    }

    #endregion


    #region Unity Methods

    private void OnDisable()
    {
        EventBus<OnDayStateChangeEvent>.Deregister(_dayStateBinding);
    }

    #endregion
}