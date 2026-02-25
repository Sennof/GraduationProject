using UnityEngine;

public class UIChecking : MonoBehaviour
{
    [SerializeField] private bool _uiActive = false;

    public void TurnOnState() => _uiActive = true;

    public void TurnOffState() => _uiActive = false;

    public bool GetState() => _uiActive;
}
