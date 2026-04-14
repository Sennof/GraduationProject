using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    #region Fields

    [Header("State")]
    [Tooltip("Enable or disable mouse look.")]
    [SerializeField] private bool _enabled = true;

    [Header("References")]
    [Tooltip("Character transform to rotate horizontally.")]
    [SerializeField] private Transform _character;

    [Header("Settings")]
    [Tooltip("Mouse sensitivity.")]
    public float Sensitivity = 2;
    [Tooltip("Smoothing factor for mouse input.")]
    public float Smoothing = 1.5f;

    private Vector2 _velocity;
    private Vector2 _frameVelocity;
    private Vector3 _lastRotation = Vector3.zero;

    #endregion


    #region Unity Methods

    private void Reset()
    {
        _character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (_enabled)
        {
            Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * Sensitivity);
            _frameVelocity = Vector2.Lerp(_frameVelocity, rawFrameVelocity, 1 / Smoothing);
            _velocity += _frameVelocity;
            _velocity.y = Mathf.Clamp(_velocity.y, -90, 90);

            transform.localRotation = Quaternion.AngleAxis(-_velocity.y, Vector3.right);
            _character.localRotation = Quaternion.AngleAxis(_velocity.x, Vector3.up);
        }
        else
        {
            transform.eulerAngles = _lastRotation;
        }
    }

    #endregion


    #region Public Methods

    public void Enable() => _enabled = true;

    public void Disable()
    {
        _enabled = false;
        _lastRotation = transform.eulerAngles;
    }

    #endregion
}