using UnityEngine;

public class PlayerServicesManager : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField] private FirstPersonMovement FirstPersonMovement;
    [SerializeField] private FirstPersonAudio FirstPersonAudio;
    [SerializeField] private FirstPersonLook FirstPersonLook;
    [SerializeField] private Crouch Crouch;
    [SerializeField] private Jump Jump;
    [SerializeField] private Zoom Zoom;

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
        FirstPersonLook.Disable();

        if(Zoom != null)
            Zoom.Disable();
    }

    public void TurnOnLooking()
    {
        FirstPersonLook.Enable();

        if (Zoom != null)
            Zoom.Enable();
    }

    public void TurnOffMovements() => FirstPersonMovement.Disable();

    public void TurnOnMovements() => FirstPersonMovement.Enable();

    public void TurnOffAudio() => FirstPersonAudio.Disable();

    public void TurnOnAudio() => FirstPersonAudio.Enable();

    public void TurnOffJumping() => Jump.SetDisabled();

    public void TurnOnJumping() => Jump.SetEnabled();

    public void TurnOffCrouching() => Crouch.Disable();

    public void TurnOnCrouching() => Crouch.Enable();
}
