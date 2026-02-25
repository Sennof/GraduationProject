using UnityEngine;

public class PlayerServicesManager : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField] private FirstPersonLook FirstPersonLook;
    [SerializeField] private Zoom Zoom;

    [SerializeField] private FirstPersonMovement FirstPersonMovement;

    [SerializeField] private FirstPersonAudio FirstPersonAudio;

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
}
