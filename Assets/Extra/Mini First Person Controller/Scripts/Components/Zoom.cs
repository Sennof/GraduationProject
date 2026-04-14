using UnityEngine;

[ExecuteInEditMode]
public class Zoom : MonoBehaviour
{
    #region Fields

    [Header("State")]
    [Tooltip("Enable or disable zoom.")]
    [SerializeField] private bool _enabled = true;

    [Header("Settings")]
    [Tooltip("Default field of view.")]
    public float DefaultFOV = 60;
    [Tooltip("Maximum zoom field of view.")]
    public float MaxZoomFOV = 15;
    [Tooltip("Current zoom amount (0 to 1).")]
    [Range(0, 1)] public float CurrentZoom;
    [Tooltip("Zoom sensitivity.")]
    public float Sensitivity = 1;

    private Camera _camera;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        if (_camera)
        {
            DefaultFOV = _camera.fieldOfView;
        }
    }

    private void Update()
    {
        if (enabled)
        {
            CurrentZoom += Input.mouseScrollDelta.y * Sensitivity * .05f;
            CurrentZoom = Mathf.Clamp01(CurrentZoom);
            _camera.fieldOfView = Mathf.Lerp(DefaultFOV, MaxZoomFOV, CurrentZoom);
        }
    }

    #endregion


    #region Public Methods

    public void Enable() => _enabled = true;

    public void Disable() => _enabled = false;

    #endregion
}