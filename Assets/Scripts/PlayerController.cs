using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    public Rigidbody rb;
    public float speed = 5f;

    private Animator animator;

    private bool isKicking = false;
    private bool isMovingToKick = false;

    [Header("Camera")]
    public Transform cameraTransform;

    private Vector3 defaultCameraOffset;
    private Vector3 cameraVelocity = Vector3.zero;

    private bool isBallCam = false;
    private float ballStoppedTimer = 0f;

    [Header("Ball")]
    public BallController ball;
    public Transform kickPoint;
    public Transform goal;

    public float kickRange = 1f;
    public float kickDistance = 0.8f;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (cameraTransform != null)
        {
            defaultCameraOffset =
                cameraTransform.position - transform.position;
        }
    }

    private void Update()
    {
        if (isMovingToKick)
        {
            MoveToBall();
        }
        else
        {
            HandleMovement();
        }
    }

    private void LateUpdate()
    {
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        if (cameraTransform == null)
            return;

        if (isBallCam && ball != null)
        {
            Vector3 desiredPos =
                ball.transform.position +
                Vector3.up * 2f +
                Vector3.back * 5f;

            cameraTransform.position = Vector3.SmoothDamp(
                cameraTransform.position,
                desiredPos,
                ref cameraVelocity,
                0.15f
            );

            cameraTransform.LookAt(ball.transform.position);

            if (ball.IsStopped())
            {
                ballStoppedTimer += Time.deltaTime;

                if (ballStoppedTimer >= 1f)
                {
                    isBallCam = false;
                    ballStoppedTimer = 0f;
                }
            }
            else
            {
                ballStoppedTimer = 0f;
            }
        }
        else
        {
            cameraTransform.position =
                transform.position + defaultCameraOffset;

            cameraTransform.LookAt(transform);

            cameraVelocity = Vector3.zero;
        }
    }

    private void HandleMovement()
    {
        if (isKicking)
        {
            animator.SetFloat("Running", 0);
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement =
            new Vector3(horizontal, 0, vertical);

        if (movement != Vector3.zero)
        {
            transform.rotation =
                Quaternion.LookRotation(movement);
        }

        rb.MovePosition(
            rb.position +
            movement.normalized *
            speed *
            Time.deltaTime
        );

        animator.SetFloat("Running", movement.magnitude);
    }

    private void MoveToBall()
    {
        if (ball == null || kickPoint == null)
            return;

        Vector3 direction =
            ball.transform.position - transform.position;

        direction.y = 0;

        float distance =
            Vector3.Distance(
                kickPoint.position,
                ball.transform.position
            );

        if (distance <= kickDistance)
        {
            isMovingToKick = false;

            transform.rotation =
                Quaternion.LookRotation(direction);

            animator.SetFloat("Running", 0);

            isKicking = true;
            animator.SetTrigger("Kick");

            return;
        }

        direction.Normalize();

        transform.rotation =
            Quaternion.LookRotation(direction);

        rb.MovePosition(
            rb.position +
            direction *
            speed *
            Time.deltaTime
        );

        animator.SetFloat("Running", 1);
    }

    public void KickBall()
    {
        if (ball == null || goal == null)
            return;

        Vector3 ballToGoal =
            goal.position - ball.transform.position;

        Vector3 ballToPlayer =
            transform.position - ball.transform.position;

        float dot = Vector3.Dot(
            ballToGoal.normalized,
            ballToPlayer.normalized
        );

        if (dot > 0.5f)
        {
            Vector3 kickTarget =
                ball.transform.position +
                transform.forward * 20f;

            ball.KickToTarget(kickTarget);
        }
        else
        {
            ball.KickToTarget(GetAimTarget());
        }

        isBallCam = true;
        ballStoppedTimer = 0f;
    }

    private Vector3 GetAimTarget()
    {
        return goal.position +
               new Vector3(
                   Random.Range(-0.5f, 0.5f),
                   Random.Range(0.3f, 1.2f),
                   Random.Range(-0.3f, 0.3f)
               );
    }

    public void EndKick()
    {
        isKicking = false;
    }

    public bool CanKick()
    {
        if (ball == null || kickPoint == null)
            return false;

        float distance =
            Vector3.Distance(
                kickPoint.position,
                ball.transform.position
            );

        return distance <= kickRange
               && !isKicking
               && !isMovingToKick;
    }

    public void BeginKick()
    {
        isMovingToKick = true;
    }

    public bool IsBallCam()
    {
        return isBallCam;
    }

    public void SetConfettiInstance(GameObject confetti)
    {
        StartCoroutine(HideConfettiWhenCamBack(confetti));
    }

    private System.Collections.IEnumerator HideConfettiWhenCamBack(GameObject confetti)
    {
        yield return new WaitUntil(() => !isBallCam);

        if (confetti != null)
        {
            Destroy(confetti);
        }
    }
}