using UnityEngine;

public class Jump : MonoBehaviour
{
    #region Fields

    [Header("Settings")]
    [Tooltip("Strength of the jump impulse.")]
    public float JumpStrength = 2;

    [Tooltip("Prevents jumping when not grounded.")]
    [SerializeField] private GroundCheck _groundCheck;

    public event System.Action Jumped;

    private bool _enabled = true;
    private Rigidbody _rigidbody;

    #endregion


    #region Unity Methods

    private void Reset()
    {
        _groundCheck = GetComponentInChildren<GroundCheck>();
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void LateUpdate()
    {
        if (!_enabled)
        {
            return;
        }

        if (Input.GetButtonDown("Jump") && (!_groundCheck || _groundCheck.IsGrounded))
        {
            _rigidbody.AddForce(Vector3.up * _rigidbody.mass * 100 * JumpStrength);
            Jumped?.Invoke();
        }
    }

    #endregion


    #region Public Methods

    public void SetEnabled() => _enabled = true;

    public void SetDisabled() => _enabled = false;

    #endregion
}