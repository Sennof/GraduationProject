using UnityEngine;

public class TimeManager : MonoBehaviour
{
    #region Fields

    [Header("Settings")]
    [Tooltip("Default time scale.")]
    [SerializeField] private int _timeModDefault = 1;

    #endregion


    #region Public Methods

    public void SetSpeedModifier(int modifier)
    {
        Time.timeScale = modifier;
    }

    #endregion


    #region Unity Methods

    private void OnDisable()
    {
        SetSpeedModifier(_timeModDefault);
    }

    #endregion
}