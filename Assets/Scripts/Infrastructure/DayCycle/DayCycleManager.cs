using UnityEngine;
using UnityEngine.Events;

public class DayCycleManager : MonoBehaviour, IInitializeable
{
    [SerializeField] private UIDayCycle _ui;
    [SerializeField] private SunAngleSetter _sunAngleSetter;
    [Space(15)]
    [SerializeField] private bool _enabled = true;
    [SerializeField] private int _modifier = 1;
    [SerializeField] private int _dayDuration = 8;
    [Space(15)]
    [SerializeField] private UnityEvent _endDayEvents; 

    private int _mins = 0;
    private float _secs = 0;

    public void Initialize()
    {
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

    private void EndDay()
    {
        _enabled = false;
        _sunAngleSetter.Sunset();
        _endDayEvents?.Invoke();
    }

    private void StartDay()
    {
        _enabled = true;
        _sunAngleSetter.Sunrise();
        _secs = 0;
        _mins = 0;
    }
}
