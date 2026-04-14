using UnityEngine;
using UnityEngine.Events;

public class DayCycleManager : MonoBehaviour, IInitializeable
{
    #region Fields

    [Header("UI References")]
    [Tooltip("UI element showing day time.")]
    [SerializeField] private UIDayCycle _ui;
    [Tooltip("Sun angle controller.")]
    [SerializeField] private SunAngleSetter _sunAngleSetter;

    [Space(15)]
    [Header("State")]
    [Tooltip("Current day/night state.")]
    [SerializeField] private bool _isDay = true;

    [Space(15)]
    [Header("Settings")]
    [Tooltip("Enable day cycle progression.")]
    [SerializeField] private bool _enabled = true;
    [Tooltip("Time multiplier.")]
    [SerializeField] private int _modifier = 1;
    [Tooltip("Day duration in minutes.")]
    [SerializeField] private int _dayDuration = 8;

    [Space(15)]
    [Header("Events")]
    [Tooltip("Events invoked at day start.")]
    [SerializeField] private UnityEvent _startDayEvents;
    [Tooltip("Events invoked at day end.")]
    [SerializeField] private UnityEvent _endDayEvents;

    private int _minutes = 0;
    private float _seconds = 0;

    public static DayCycleManager Instance { get; private set; }

    #endregion


    #region Public Methods

    public void Initialize()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        StartDay();
    }

    public bool GetDayState() => _isDay;

    #endregion


    #region Private Methods

    private void EndDay()
    {
        _isDay = false;
        _enabled = false;
        _sunAngleSetter.Sunset();
        _endDayEvents?.Invoke();

        EventBus<OnDayStateChangeEvent>.Raise(new OnDayStateChangeEvent { IsDay = false });
    }

    private void StartDay()
    {
        _isDay = true;
        _enabled = true;
        _seconds = 0;
        _minutes = 0;
        _sunAngleSetter.Sunrise();
        _startDayEvents?.Invoke();

        EventBus<OnDayStateChangeEvent>.Raise(new OnDayStateChangeEvent { IsDay = true });
    }

    #endregion


    #region Unity Methods

    private void Update()
    {
        if (_enabled == false)
        {
            return;
        }

        _seconds += Time.deltaTime * _modifier;

        if (_seconds >= 60)
        {
            _minutes++;
            _seconds = 0;
        }

        if (_minutes >= _dayDuration)
        {
            EndDay();
        }

        _ui.UpdateText(_minutes, (int)_seconds);
    }

    #endregion
}