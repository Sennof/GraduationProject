using UnityEngine;

public class PlayerServicesManager : MonoBehaviour, IInitializeable
{
    #region Singleton

    public static PlayerServicesManager Instance { get; private set; }

    #endregion


    #region Fields

    [Header("Player Components")]
    [Tooltip("First person movement component.")]
    [SerializeField] private FirstPersonMovement _firstPersonMovement;
    [Tooltip("First person audio component.")]
    [SerializeField] private FirstPersonAudio _firstPersonAudio;
    [Tooltip("First person look component.")]
    [SerializeField] private FirstPersonLook _firstPersonLook;
    [Tooltip("Crouch component.")]
    [SerializeField] private Crouch _crouch;
    [Tooltip("Jump component.")]
    [SerializeField] private Jump _jump;
    [Tooltip("Zoom component.")]
    [SerializeField] private Zoom _zoom;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        SetOnTotal();
    }

    public void SetOffTotal()
    {
        TurnOffLooking();
        TurnOffMovements();
        TurnOffAudio();
        TurnOffJumping();
        TurnOffCrouching();
    }

    public void SetOnTotal()
    {
        TurnOnLooking();
        TurnOnMovements();
        TurnOnAudio();
        TurnOnJumping();
        TurnOnCrouching();
    }

    public void TurnOffLooking()
    {
        _firstPersonLook.Disable();
        if (_zoom != null) _zoom.Disable();
    }

    public void TurnOnLooking()
    {
        _firstPersonLook.Enable();
        if (_zoom != null) _zoom.Enable();
    }

    public void TurnOffMovements() => _firstPersonMovement.Disable();
    public void TurnOnMovements() => _firstPersonMovement.Enable();
    public void TurnOffAudio() => _firstPersonAudio.Disable();
    public void TurnOnAudio() => _firstPersonAudio.Enable();
    public void TurnOffJumping() => _jump.SetDisabled();
    public void TurnOnJumping() => _jump.SetEnabled();
    public void TurnOffCrouching() => _crouch.Disable();
    public void TurnOnCrouching() => _crouch.Enable();

    #endregion
}