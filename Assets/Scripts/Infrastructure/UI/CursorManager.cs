using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public void ShowCursor() => Cursor.lockState = CursorLockMode.Confined;
    
    public void HideCursore() => Cursor.lockState = CursorLockMode.Locked;
}
