using UnityEngine;

public class Crouch : MonoBehaviour
{
    #region Fields

    [Header("Input")]
    [Tooltip("Key to toggle crouch.")]
    public KeyCode Key = KeyCode.LeftControl;

    [Header("Slow Movement")]
    [Tooltip("Movement component to override speed.")]
    public FirstPersonMovement Movement;
    [Tooltip("Movement speed when crouched.")]
    public float MovementSpeed = 2;

    [Header("Low Head")]
    [Tooltip("Head transform to lower when crouched.")]
    public Transform HeadToLower;
    [HideInInspector] public float? DefaultHeadYLocalPosition;
    [Tooltip("Local Y position of the head when crouched.")]
    public float CrouchYHeadPosition = 1;

    [Tooltip("Collider to resize when crouched.")]
    public CapsuleCollider ColliderToLower;
    [HideInInspector] public float? DefaultColliderHeight;

    public bool IsCrouched { get; private set; }
    public event System.Action CrouchStart, CrouchEnd;

    private bool _enabled = true;

    #endregion


    #region Unity Methods

    private void Reset()
    {
        Movement = GetComponentInParent<FirstPersonMovement>();
        HeadToLower = Movement.GetComponentInChildren<Camera>().transform;
        ColliderToLower = Movement.GetComponentInChildren<CapsuleCollider>();
    }

    private void LateUpdate()
    {
        if (!_enabled)
        {
            return;
        }

        if (Input.GetKey(Key))
        {
            if (HeadToLower)
            {
                if (!DefaultHeadYLocalPosition.HasValue)
                {
                    DefaultHeadYLocalPosition = HeadToLower.localPosition.y;
                }

                HeadToLower.localPosition = new Vector3(HeadToLower.localPosition.x, CrouchYHeadPosition, HeadToLower.localPosition.z);
            }

            if (ColliderToLower)
            {
                if (!DefaultColliderHeight.HasValue)
                {
                    DefaultColliderHeight = ColliderToLower.height;
                }

                float loweringAmount;
                if (DefaultHeadYLocalPosition.HasValue)
                {
                    loweringAmount = DefaultHeadYLocalPosition.Value - CrouchYHeadPosition;
                }
                else
                {
                    loweringAmount = DefaultColliderHeight.Value * .5f;
                }

                ColliderToLower.height = Mathf.Max(DefaultColliderHeight.Value - loweringAmount, 0);
                ColliderToLower.center = Vector3.up * ColliderToLower.height * .5f;
            }

            if (!IsCrouched)
            {
                IsCrouched = true;
                SetSpeedOverrideActive(true);
                CrouchStart?.Invoke();
            }
        }
        else
        {
            if (IsCrouched)
            {
                if (HeadToLower)
                {
                    HeadToLower.localPosition = new Vector3(HeadToLower.localPosition.x, DefaultHeadYLocalPosition.Value, HeadToLower.localPosition.z);
                }

                if (ColliderToLower)
                {
                    ColliderToLower.height = DefaultColliderHeight.Value;
                    ColliderToLower.center = Vector3.up * ColliderToLower.height * .5f;
                }

                IsCrouched = false;
                SetSpeedOverrideActive(false);
                CrouchEnd?.Invoke();
            }
        }
    }

    #endregion


    #region Public Methods

    public void Enable() => _enabled = true;

    public void Disable() => _enabled = false;

    #endregion


    #region Speed Override

    private void SetSpeedOverrideActive(bool state)
    {
        if (!Movement)
        {
            return;
        }

        if (state)
        {
            if (!Movement.speedOverrides.Contains(SpeedOverride))
            {
                Movement.speedOverrides.Add(SpeedOverride);
            }
        }
        else
        {
            if (Movement.speedOverrides.Contains(SpeedOverride))
            {
                Movement.speedOverrides.Remove(SpeedOverride);
            }
        }
    }

    private float SpeedOverride() => MovementSpeed;

    #endregion
}