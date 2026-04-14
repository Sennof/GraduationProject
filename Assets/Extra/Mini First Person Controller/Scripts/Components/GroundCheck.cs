using UnityEngine;

[ExecuteInEditMode]
public class GroundCheck : MonoBehaviour
{
    #region Fields

    [Tooltip("Maximum distance from the ground.")]
    public float DistanceThreshold = .15f;

    [Tooltip("Whether this transform is grounded now.")]
    public bool IsGrounded = true;

    public event System.Action Grounded;

    private const float OriginOffset = .001f;
    private Vector3 RaycastOrigin => transform.position + Vector3.up * OriginOffset;
    private float RaycastDistance => DistanceThreshold + OriginOffset;

    #endregion


    #region Unity Methods

    private void LateUpdate()
    {
        bool isGroundedNow = Physics.Raycast(RaycastOrigin, Vector3.down, DistanceThreshold * 2);

        if (isGroundedNow && !IsGrounded)
        {
            Grounded?.Invoke();
        }

        IsGrounded = isGroundedNow;
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawLine(RaycastOrigin, RaycastOrigin + Vector3.down * RaycastDistance, IsGrounded ? Color.white : Color.red);
    }

    #endregion
}