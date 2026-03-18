using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private int _timeModDefault = 1;

    public void SetSpeedModifier(int modifier)
    {
        Time.timeScale = modifier;
    }

    private void OnDisable()
    {
        SetSpeedModifier(_timeModDefault);
    }
}
