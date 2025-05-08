using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementStats MoveStats;
    public Transform groundCheck; 

    Rigidbody2D rb;
    Animator    anim;

    /* Estado */
    bool  isGrounded;
    bool  isFacingRight = true;
    Vector2 moveInput;

    /* Timers */
    float coyoteCounter;
    float jumpBufferCounter;

    /* ─────────────────────────────────────────────────────────── */
    void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        rb.freezeRotation = true;
        rb.interpolation  = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        HandleTimers();
        ReadInput();
        CheckGround();
        HandleJump();
        UpdateAnimator();
    }

    void FixedUpdate() => ApplyHorizontalMovement();

    /* Timers & Input */
    void HandleTimers()
    {
        coyoteCounter     -= Time.deltaTime;
        jumpBufferCounter -= Time.deltaTime;
    }

    void ReadInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = MoveStats.JumpBufferTime;

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new(rb.linearVelocity.x,
                              rb.linearVelocity.y * MoveStats.JumpCutMultiplier);
    }

    /* Ground Check */
    void CheckGround()
    {
        bool overlap = Physics2D.OverlapBox(
            groundCheck.position,
            MoveStats.GroundCheckSize,
            0f,
            MoveStats.GroundLayer);

        /* Só considera grounded se NÃO estiver subindo */
        isGrounded = overlap && rb.linearVelocity.y <= 0.01f;

        if (isGrounded) coyoteCounter = MoveStats.CoyoteTime;
    }


    /* Jump */
    void HandleJump()
    {
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            jumpBufferCounter = 0f;
            coyoteCounter     = 0f;

            rb.linearVelocity = new(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * MoveStats.JumpForce, ForceMode2D.Impulse);
            anim.SetTrigger("Jump");
        }
    }

    /* Horizontal Movement */
    void ApplyHorizontalMovement()
    {
        float maxSpeed = Input.GetKey(KeyCode.LeftShift)
                       ? MoveStats.RunSpeed
                       : MoveStats.WalkSpeed;

        float targetSpeed = moveInput.x * maxSpeed;
        float accel = Mathf.Abs(targetSpeed) > 0.1f ? MoveStats.Acceleration
                                                    : MoveStats.Deceleration;

        float speed = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed,
                                        accel * Time.fixedDeltaTime);

        rb.linearVelocity = new(speed,
                          Mathf.Max(rb.linearVelocity.y, -MoveStats.MaxFallSpeed));

        if (targetSpeed != 0f) HandleFlip(targetSpeed);
    }

    void HandleFlip(float targetSpeed)
    {
        bool faceRight = targetSpeed > 0f;
        if (faceRight != isFacingRight)
        {
            isFacingRight = faceRight;
            Vector3 s = transform.localScale;
            s.x *= -1f;
            transform.localScale = s;
        }
    }

    /* Animator */
    void UpdateAnimator()
    {
        anim.SetBool ("isGrounded", isGrounded);
        anim.SetBool ("isFalling",  rb.linearVelocity.y < -0.1f && !isGrounded);
        anim.SetBool ("isRunning",  Mathf.Abs(rb.linearVelocity.x) > MoveStats.WalkSpeed + 0.1f);
        anim.SetBool ("isWalking",  Mathf.Abs(rb.linearVelocity.x) > 0.1f &&
                                    Mathf.Abs(rb.linearVelocity.x) <= MoveStats.WalkSpeed + 0.1f);
        anim.SetFloat("VerticalVelocity", rb.linearVelocity.y);
    }

    /* Gizmos (debug) */
    void OnDrawGizmosSelected()
    {
        if (!groundCheck || MoveStats == null || !MoveStats.DebugShowGroundBox) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(groundCheck.position, MoveStats.GroundCheckSize);
    }
}
