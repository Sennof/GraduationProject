using UnityEngine;

public class Jump : MonoBehaviour
{
    private bool _enabled = true;
    Rigidbody rigidbody;
    public float jumpStrength = 2;
    public event System.Action Jumped;

    [SerializeField, Tooltip("Prevents jumping when the transform is in mid-air.")]
    GroundCheck groundCheck;


    void Reset()
    {
        // Try to get groundCheck.
        groundCheck = GetComponentInChildren<GroundCheck>();
    }

    void Awake()
    {
        // Get rigidbody.
        rigidbody = GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        if (!_enabled) return;
        // Jump when the Jump button is pressed and we are on the ground.
        if (Input.GetButtonDown("Jump") && (!groundCheck || groundCheck.isGrounded))
        {
            rigidbody.AddForce(Vector3.up * rigidbody.mass * 100 * jumpStrength);
            Jumped?.Invoke();
        }
    }

    public void SetEnabled() => _enabled = true;

    public void SetDisabled() => _enabled = false;
}
