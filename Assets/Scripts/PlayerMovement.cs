using UnityEngine;
using System.Collections;   

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementStats MoveStats;
    public Transform groundCheck;
    public Transform ledgeCheckLower;
    public Transform ledgeCheckUpper;
    public LayerMask wallLayer;

    Rigidbody2D rb;
    Animator anim;

    bool isGrounded;
    bool isFacingRight = true;
    bool isHanging = false;
    Vector2 moveInput;

    float coyoteCounter;
    float jumpBufferCounter;
    float defaultGravityScale;

    bool isRolling = false;
    bool isDashing = false;
    float rollTimer = 0f;
    float dashTimer = 0f;
    float rollCooldown = 0f;
    float dashCooldown = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        defaultGravityScale = rb.gravityScale;
    }

    void Update()
    {
        if (isHanging)
        {
            if (InputManager.JumpWasPressed)
                StartCoroutine(ClimbLedge());
            else if (InputManager.RunIsHeld)
                DropFromLedge();
            return;
        }

        HandleTimers();
        ReadInput();
        CheckGround();
        CheckForLedge();
        HandleJump();
        HandleRoll();
        HandleDash();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (!isRolling && !isDashing && !isHanging)
            ApplyHorizontalMovement();
    }

    void HandleTimers()
    {
        coyoteCounter     -= Time.deltaTime;
        jumpBufferCounter -= Time.deltaTime;
        rollTimer         -= Time.deltaTime;
        dashTimer         -= Time.deltaTime;
        rollCooldown      -= Time.deltaTime;
        dashCooldown      -= Time.deltaTime;

        if (rollTimer <= 0f) isRolling = false;
        if (dashTimer <= 0f && isDashing)
        {
            isDashing = false;
            rb.gravityScale = defaultGravityScale;
        }
    }

    void ReadInput()
    {
        moveInput = InputManager.Movement;

        if (InputManager.JumpWasPressed)
            jumpBufferCounter = MoveStats.JumpBufferTime;

        if (InputManager.JumpWasReleased && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new(rb.linearVelocity.x, rb.linearVelocity.y * MoveStats.JumpCutMultiplier);

        if (InputManager.RollWasPressed) TryRoll();
        if (InputManager.DashWasPressed) TryDash();
    }

    void CheckGround()
    {
        bool overlap = Physics2D.OverlapBox(
            groundCheck.position,
            MoveStats.GroundCheckSize,
            0f,
            MoveStats.GroundLayer);

        isGrounded = overlap && rb.linearVelocity.y <= 0.01f;

        if (isGrounded)
            coyoteCounter = MoveStats.CoyoteTime;
    }

    void CheckForLedge()
    {
        bool lowerHit = Physics2D.OverlapCircle(ledgeCheckLower.position, 0.1f, wallLayer);
        bool upperHit = Physics2D.OverlapCircle(ledgeCheckUpper.position, 0.1f, wallLayer);

        if (lowerHit && !upperHit && !isGrounded && rb.linearVelocity.y < 0f)
        {
            EnterLedgeHang();
        }
    }

    void EnterLedgeHang()
    {
        isHanging = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        anim.SetTrigger("LedgeHold");

        // Ajuste fino da posição do personagem (ajuste conforme necessário)
        Vector3 hangOffset = new Vector3(isFacingRight ? -0.2f : 0.2f, -0.3f, 0f);
        transform.position += hangOffset;

    }

    IEnumerator ClimbLedge()
    {
        anim.SetTrigger("LedgeClimb");

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(isFacingRight ? 1f : -1f, 1.5f, 0f);

        float duration = 0.5f;
        float elapsed = 0f;

        isHanging = false;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        rb.gravityScale = defaultGravityScale;
    }

    void DropFromLedge()
    {
        isHanging = false;
        rb.gravityScale = defaultGravityScale;
    }

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

    void ApplyHorizontalMovement()
    {
        float maxSpeed = InputManager.RunIsHeld ? MoveStats.RunSpeed : MoveStats.WalkSpeed;
        float targetSpeed = moveInput.x * maxSpeed;
        float accel = Mathf.Abs(targetSpeed) > 0.1f ? MoveStats.Acceleration : MoveStats.Deceleration;

        float speed = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accel * Time.fixedDeltaTime);
        rb.linearVelocity = new(speed, Mathf.Max(rb.linearVelocity.y, -MoveStats.MaxFallSpeed));

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

    void TryRoll()
    {
        if (!isRolling && isGrounded && rollCooldown <= 0f)
        {
            isRolling = true;
            rollTimer = MoveStats.RollDuration;
            rollCooldown = MoveStats.RollCooldown;
            anim.SetTrigger("Roll");

            float dir = isFacingRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(dir * MoveStats.RollSpeed, 0f);
        }
    }

    void HandleRoll()
    {
        if (isRolling)
        {
            float dir = isFacingRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(dir * MoveStats.RollSpeed, rb.linearVelocity.y);
        }
    }

    void TryDash()
    {
        if (!isDashing && dashCooldown <= 0f)
        {
            float dir = isFacingRight ? 1f : -1f;

            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(dir * MoveStats.DashSpeed, 0f);

            isDashing = true;
            dashTimer = MoveStats.DashDuration;
            dashCooldown = MoveStats.DashCooldown;

            anim.SetTrigger("Dash");
        }
    }

    void HandleDash()
    {
        if (isDashing)
        {
            float dir = isFacingRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(dir * MoveStats.DashSpeed, rb.linearVelocity.y);
        }
    }

    void UpdateAnimator()
    {
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isFalling", rb.linearVelocity.y < -0.1f && !isGrounded);
        anim.SetBool("isRunning", Mathf.Abs(rb.linearVelocity.x) > MoveStats.WalkSpeed + 0.1f);
        anim.SetBool("isWalking", Mathf.Abs(rb.linearVelocity.x) > 0.1f &&
                                  Mathf.Abs(rb.linearVelocity.x) <= MoveStats.WalkSpeed + 0.1f);
    }

    void OnDrawGizmosSelected()
    {
        if (!groundCheck || MoveStats == null || !MoveStats.DebugShowGroundBox) return;

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(groundCheck.position, MoveStats.GroundCheckSize);

        Gizmos.color = Color.yellow;
        if (ledgeCheckLower != null)
            Gizmos.DrawWireSphere(ledgeCheckLower.position, 0.1f);
        if (ledgeCheckUpper != null)
            Gizmos.DrawWireSphere(ledgeCheckUpper.position, 0.1f);
    }
}
