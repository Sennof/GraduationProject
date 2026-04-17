using UnityEngine;

public class CursorManager : MonoBehaviour, IInitializeable
{
    #region Singleton

    public static CursorManager Instance { get; private set; }

    public void Initialize()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #endregion


    #region Public Methods

    public void ShowCursor() => Cursor.lockState = CursorLockMode.Confined;
    public void HideCursor() => Cursor.lockState = CursorLockMode.Locked;

    #endregion
}