using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody rb;

    public float friction = 0.98f;
    public float stopThreshold = 0.05f;

    [Header("Arc Settings")]
    [Tooltip("Thời gian bóng bay đến mục tiêu (giây)")]
    public float flightTime = 1.2f;

    [Tooltip("Hệ số độ cong: càng cao bóng bay càng cao. Tỉ lệ theo khoảng cách.")]
    public float arcHeightPerMeter = 0.15f;

    [Tooltip("Độ cong tối thiểu dù gần đến đâu")]
    public float minArcHeight = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb.useGravity && rb.linearVelocity.magnitude > stopThreshold)
        {
            if (Mathf.Abs(transform.position.y - GetGroundY()) < 0.3f)
            {
                Vector3 vel = rb.linearVelocity;
                vel.x *= friction;
                vel.z *= friction;
                rb.linearVelocity = vel;
            }
        }

        if (rb.linearVelocity.magnitude < stopThreshold && IsNearGround())
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
    public bool IsStopped()
    {
        return rb.linearVelocity.magnitude < stopThreshold;
    }

    public Vector3 GetVelocityDirection()
    {
        if (rb.linearVelocity.magnitude < 0.01f)
            return transform.forward;
        return rb.linearVelocity.normalized;
    }

    public void KickToTarget(Vector3 targetPosition)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 startPos = transform.position;

        float horizontalDist = Vector3.Distance(
            new Vector3(startPos.x, 0, startPos.z),
            new Vector3(targetPosition.x, 0, targetPosition.z)
        );

        float tFlight = flightTime * Mathf.Clamp(horizontalDist / 10f, 0.6f, 2f);
        float arcHeight = Mathf.Max(minArcHeight, horizontalDist * arcHeightPerMeter);
        float g = Mathf.Abs(Physics.gravity.y);
        float tPeak = tFlight * 0.5f;
        float v0y = (arcHeight + 0.5f * g * tPeak * tPeak) / tPeak;
        Vector3 horizontalDisplacement = new Vector3(
            targetPosition.x - startPos.x,
            0,
            targetPosition.z - startPos.z
        );
        Vector3 v0xz = horizontalDisplacement / tFlight;
        Vector3 launchVelocity = new Vector3(v0xz.x, v0y, v0xz.z);
        rb.AddForce(launchVelocity, ForceMode.VelocityChange);

    }

    public void Kick(Vector3 direction, float force)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(direction.normalized * force, ForceMode.Impulse);
    }

    private float GetGroundY()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f))
            return hit.point.y;
        return 0f;
    }

    private bool IsNearGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.3f);
    }
}