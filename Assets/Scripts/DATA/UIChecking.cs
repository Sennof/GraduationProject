using UnityEngine;

public class UIChecking : MonoBehaviour, IInitializeable
{
    [SerializeField] private bool _uiActive = false;

    public void Initialize()
    {
        TurnOffState();
    }

    public void TurnOnState() => _uiActive = true;

    public void TurnOffState() => _uiActive = false;

    public bool GetState() => _uiActive;
}
