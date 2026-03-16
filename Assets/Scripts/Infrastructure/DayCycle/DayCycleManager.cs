using UnityEngine;
using UnityEngine.Events;

public class DayCycleManager : MonoBehaviour, IInitializeable
{
    public static DayCycleManager Instance { get; private set; }

    [SerializeField] private UIDayCycle _ui;
    [SerializeField] private SunAngleSetter _sunAngleSetter;
    [Space(15)]
    [SerializeField] private bool _isDay = true;
    [Space(15)]
    [SerializeField] private bool _enabled = true;
    [SerializeField] private int _modifier = 1;
    [SerializeField] private int _dayDuration = 8;
    [Space(15)]
    [SerializeField] private UnityEvent _startDayEvents;
    [SerializeField] private UnityEvent _endDayEvents;

    private int _mins = 0;
    private float _secs = 0;

    public void Initialize()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        StartDay();
    }

    private void Update()
    {
        if (_enabled == false) return;

        _secs += Time.deltaTime * _modifier;

        if(_secs >= 60)
        {
            _mins++;
            _secs = 0;
        }

        if(_mins >= _dayDuration)
        {
            EndDay();
        }

        _ui.UpdateText(_mins, (int)_secs);
    }

    public bool GetDayState() => _isDay;

    private void EndDay()
    {
        _isDay = false;
        _enabled = false;
        _sunAngleSetter.Sunset();
        _endDayEvents?.Invoke();

        EventBus<OnDayStateChangeEvent>.Raise(new OnDayStateChangeEvent { isDay = false });
    }

    private void StartDay()
    {
        _isDay = true;
        _enabled = true;
        _secs = 0;
        _mins = 0;
        _sunAngleSetter.Sunrise();
        _startDayEvents?.Invoke();

        EventBus<OnDayStateChangeEvent>.Raise(new OnDayStateChangeEvent { isDay = true });
    }
}
