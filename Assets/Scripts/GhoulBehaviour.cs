using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Transform player;
    public Transform attackPoint;
    public LayerMask playerLayer;
    public AudioSource audioSource;
    public AudioClip attackSound;

    private bool isAwaken = false;

    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float attackDelay = 0.5f;
    public int attackDamage = 1;
    public float moveSpeed = 2f;

    private float lastAttackTime;
    private bool isAttacking = false;

    private Animator animator;
    private bool isPlayerInZone = false;
    private bool isWalking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Verifica se os objetos necessários estão configurados
        if (player == null)
            Debug.LogWarning("Player reference not set in " + gameObject.name);
        
        if (attackPoint == null)
            Debug.LogWarning("Attack Point reference not set in " + gameObject.name);
    }

    public void OnAttackAnimationEnd()
    {
        Debug.Log("Fim da animação de ataque");
    }

    void Update()
    {
        // Verificação de null para evitar NullReferenceException
        if (player == null || attackPoint == null) return;

        // Movimento horizontal se player estiver na zona
        float distance = Vector2.Distance(attackPoint.position, player.position);
        if (isPlayerInZone && isAwaken)
        {
            float distanceX = player.position.x - transform.position.x;

            if (Mathf.Abs(distanceX) > 0.1f && distance >= attackRange)
            {
                Vector3 direction = new Vector3(Mathf.Sign(distanceX), 0f, 0f);
                transform.position += direction * moveSpeed * Time.deltaTime;

                isWalking = true;
            }
            else
            {
                isWalking = false;
            }

            // Verifica se o animator existe antes de usar
            if (animator != null)
                animator.SetBool("isWalking", isWalking);
        }
        else
        {
            if (animator != null)
                animator.SetBool("isWalking", false);
        }

        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown && isAwaken && !isAttacking)
        {
            isAttacking = true; // Marca que está atacando
            if (animator != null)
                animator.SetTrigger("Attack");
        }

        // Vira para o lado do player
        Vector3 scale = transform.localScale;
        if (player.position.x < transform.position.x)
            scale.x = -Mathf.Abs(scale.x);
        else
            scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void PerformAttack()
    {
        if (Time.time < lastAttackTime) return;

        // Verifica se o attackPoint existe antes de usar
        if (attackPoint == null) return;

        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
        if (hitPlayer != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage();
        }
        isAttacking = false;
        lastAttackTime = Time.time;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void WakeUp()
    {
        isAwaken = true;
    }

    public void Sleep()
    {
        isAwaken = false;
    }

    public void PlayAttackSound()
    {
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    public void SetPlayerInZone(bool inZone)
    {
        isPlayerInZone = inZone;
        if (animator != null)
            animator.SetTrigger(inZone ? "Wake" : "Sleep");
    }
}