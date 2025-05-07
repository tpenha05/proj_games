using UnityEngine;

public class PlayerSwordMaster : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float jumpForce = 10f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.4f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    private bool isGrounded;
    private bool isJumping;
    private bool isRolling;

    private float horizontalInput;
    private bool isRunning;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isRunning = Input.GetKey(KeyCode.LeftShift);
        isRolling = Input.GetKeyDown(KeyCode.C);

        // Flip
        if (horizontalInput != 0)
            sr.flipX = horizontalInput < 0;

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded && !isRolling)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = true;
        }

        // ROLL (interrompe movimento)
        if (isRolling && isGrounded)
        {
            anim.SetTrigger("Roll");
            rb.linearVelocity = new Vector2((sr.flipX ? -1 : 1) * runSpeed * 1.5f, rb.linearVelocity.y);
        }

        // Animações
        bool isWalking = horizontalInput != 0 && !isRunning && !isRolling;

        anim.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isJumping", isJumping);
        anim.SetBool("isRunning", isRunning);
        anim.SetBool("isGrounded", isGrounded);
    }

    void FixedUpdate()
    {
        // Verifica se está no chão usando uma caixa
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        Debug.Log("isGrounded: " + isGrounded);

        if (isGrounded && rb.linearVelocity.y <= 0)
        {
            isJumping = false;
        }

        // Movimento
        if (!isRolling)
        {
            float currentSpeed = isRunning ? runSpeed : walkSpeed;
            rb.linearVelocity = new Vector2(horizontalInput * currentSpeed, rb.linearVelocity.y);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }
}
