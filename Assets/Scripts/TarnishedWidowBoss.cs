using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class TarnishedWidowBoss : MonoBehaviour
{
    /* ─────────── INSPECTOR ─────────── */
    [Header("Referências")]
    public Transform player;
    public Transform groundCheck;
    public TongueHitbox tongue;
    public MeleeHitbox meleeHitbox;
    public GameObject shadowPrefab;
    public LayerMask groundLayer;

    [Header("Movimento")]
    [Tooltip("Velocidade de perseguição até entrar no alcance de combo")]
    public float walkSpeed = 2.5f;
    [Tooltip("Distância máxima para iniciar combos")]
    public float comboRange = 6f;

    [Header("Combos")]
    [Tooltip("Tempo entre cada ataque no combo")]
    public float attackInterval = 1f;
    [Tooltip("Tempo em Idle após completar um combo")]
    public float idlePause = 2f;
    private readonly string[][] combos = new string[][] {
        new string[]{ "AttackMelee", "AttackFar" },
        new string[]{ "AttackMelee", "AttackMelee", "AttackFar" },
        new string[]{ "AttackMelee" },
        new string[]{ "AttackFar" },
        new string[]{ "AttackFar", "AttackFar", "AttackMelee" }
    };

    [Header("Jump-Attack")]
    public float vanishTime = 0.6f;
    public float dropHeight = 6f;
    public float smashRadius = 2.4f;

    [Header("Cooldowns")]
    public float jumpCooldown = 20f;
    public float farAttackCooldown = 5f;

    [Header("Vida")]
    public int maxHealth = 10;

    // AnimationEvent proxies
    public void EnableHitbox()  { if (tongue != null) tongue.EnableHitbox(); }
    public void DisableHitbox() { if (tongue != null) tongue.DisableHitbox(); }
    public void EnableMelee()   { if (meleeHitbox != null) meleeHitbox.EnableMelee(); }
    public void DisableMelee()  { if (meleeHitbox != null) meleeHitbox.DisableMelee(); }

    /* ─────────── PRIVADOS ─────────── */
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private Collider2D[] cols;

    private bool facingRight;
    private int currentHealth;
    private bool isDead;

    private bool isPerformingCombo;
    private int comboCounter;
    private float lastJumpTime = -Mathf.Infinity;
    private float lastFarAttackTime = -Mathf.Infinity;

    /* ─────────── UNITY ─────────── */
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        cols = GetComponents<Collider2D>();
        currentHealth = maxHealth;
        facingRight = transform.localScale.x > 0;
    }

    void Update()
    {
        if (isDead) return;

        // Atualiza animações de movimento
        anim.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool("isGrounded", IsGrounded());

        // Inicia combo se não estiver executando
        if (!isPerformingCombo)
            StartCoroutine(PerformComboRoutine());
    }

    /* ─────────── COMBO ROUTINE ─────────── */
    IEnumerator PerformComboRoutine()
    {
        isPerformingCombo = true;

        // Persegue enquanto estiver fora do comboRange
        while (Vector2.Distance(transform.position, player.position) > comboRange)
        {
            WalkTowardsPlayer();
            yield return null;
        }
        rb.linearVelocity = Vector2.zero;

        // Executa o combo atual
        string[] combo = combos[comboCounter % combos.Length];
        foreach (var trigger in combo)
        {
            if (trigger == "AttackFar" && Time.time - lastFarAttackTime < farAttackCooldown)
                continue;

            if (trigger == "AttackFar")
                lastFarAttackTime = Time.time;

            anim.SetTrigger(trigger);
            yield return new WaitForSeconds(attackInterval);
        }
        comboCounter++;

        // Idle pause após combo
        yield return new WaitForSeconds(idlePause);

        // Jump-Attack a cada 3 ou 4 combos
        if (comboCounter % 3 == 0 || comboCounter % 4 == 0)
        {
            StartJumpAttack();
            yield return new WaitUntil(IsGrounded);
            yield return new WaitForSeconds(idlePause);
        }

        isPerformingCombo = false;
    }

    /* ─────────── MOVIMENTO ─────────── */
    void WalkTowardsPlayer()
    {
        FacePlayer();
        rb.linearVelocity = new Vector2((facingRight ? 1 : -1) * walkSpeed, rb.linearVelocity.y);
    }

    /* ─────────── JUMP ATTACK (TELEPORT) ─────────── */
    public void StartJumpAttack()
    {
        if (!IsGrounded() || Time.time - lastJumpTime < jumpCooldown)
            return;

        lastJumpTime = Time.time;
        StartCoroutine(JumpAttackRoutine());
    }

    IEnumerator JumpAttackRoutine()
    {
        anim.SetTrigger("JumpAttack");
        yield return new WaitForSeconds(attackInterval);

        Vector3 targetPos = player.position;
        ToggleColliders(false);
        sr.enabled = false;
        GameObject shadow = shadowPrefab ? Instantiate(shadowPrefab, new Vector3(targetPos.x, targetPos.y, transform.position.z), Quaternion.identity) : null;

        yield return new WaitForSeconds(vanishTime);

        transform.position = new Vector3(targetPos.x, targetPos.y + dropHeight, transform.position.z);
        FacePlayer();
        ToggleColliders(true);
        sr.enabled = true;
        if (shadow) Destroy(shadow);

        anim.SetTrigger("DoAttackFall");
        yield return new WaitUntil(IsGrounded);
    }

    public void DoGroundSmash()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, smashRadius, LayerMask.GetMask("Player"));
        if (hit) hit.GetComponent<PlayerHealth>()?.TakeDamage();
    }

    /* ─────────── DANO & MORTE ─────────── */
    public void TakeDamage(int dmg)
    {
        if (isDead) return;
        currentHealth -= dmg;
        anim.SetTrigger("Hit");
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        anim.SetBool("isDead", true);
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        ToggleColliders(false);
    }

    /* ─────────── UTILITIES ─────────── */
    bool IsGrounded() => Physics2D.OverlapCircle(groundCheck.position, 0.05f, groundLayer);
    void ToggleColliders(bool state) { foreach (var c in cols) c.enabled = state; }
    void FacePlayer() { bool toR = player.position.x > transform.position.x; if (toR != facingRight) Flip(); }
    void Flip() { facingRight = !facingRight; Vector3 s = transform.localScale; s.x *= -1; transform.localScale = s; }

    void OnDrawGizmosSelected()
    {
        if (meleeHitbox != null)
            Gizmos.DrawWireSphere(meleeHitbox.transform.position, meleeHitbox.knockbackForce);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, smashRadius);
    }
}