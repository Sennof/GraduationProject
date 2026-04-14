using UnityEngine;

public class CursorManager : MonoBehaviour
{
    #region Public Methods

    public void ShowCursor() => Cursor.lockState = CursorLockMode.Confined;

    public void HideCursor() => Cursor.lockState = CursorLockMode.Locked;

    #endregion
}