using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    #region Fields

    [Header("State")]
    [Tooltip("Enable or disable movement.")]
    [SerializeField] private bool _enabled = true;

    [Header("Movement Settings")]
    [Tooltip("Base movement speed.")]
    public float Speed = 5;

    [Header("Running")]
    [Tooltip("Allow sprinting.")]
    public bool CanRun = true;
    public bool IsRunning { get; private set; }
    [Tooltip("Speed when running.")]
    public float RunSpeed = 9;
    [Tooltip("Key to hold for running.")]
    public KeyCode RunningKey = KeyCode.LeftShift;

    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    private Rigidbody _rigidbody;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_enabled == true)
        {
            IsRunning = CanRun && Input.GetKey(RunningKey);

            float targetMovingSpeed = IsRunning ? RunSpeed : Speed;
            if (speedOverrides.Count > 0)
            {
                targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
            }

            Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);

            _rigidbody.linearVelocity = transform.rotation * new Vector3(targetVelocity.x, _rigidbody.linearVelocity.y, targetVelocity.y);
        }
    }

    #endregion


    #region Public Methods

    public void Enable() => _enabled = true;

    public void Disable() => _enabled = false;

    #endregion
}