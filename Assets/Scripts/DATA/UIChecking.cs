using UnityEngine;

public class UIChecking : MonoBehaviour, IInitializeable
{
    #region Fields

    [Tooltip("Current UI active state.")]
    [SerializeField] private bool _uiActive = false;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        TurnOffState();
    }

    public void TurnOnState() => _uiActive = true;

    public void TurnOffState() => _uiActive = false;

    public bool GetState() => _uiActive;

    #endregion
}