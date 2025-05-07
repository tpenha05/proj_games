using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Transform player;
    public Transform attackPoint;
    public LayerMask playerLayer;

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
    }
    public void OnAttackAnimationEnd()
    {
        Debug.Log("Fim da animação de ataque");
    }

    void Update()
    {
        Debug.Log($"Can attack? {!isAttacking && Time.time >= lastAttackTime + attackCooldown}");

        if (player == null || attackPoint == null) return;

        // Movimento horizontal se player estiver na zona
        float distance = Vector2.Distance(attackPoint.position, player.position);
        if (isPlayerInZone && isAwaken)
        {
            float distanceX = player.position.x - transform.position.x;

            if (Mathf.Abs(distanceX) > 0.1f  && distance >= attackRange)
            {
                Vector3 direction = new Vector3(Mathf.Sign(distanceX), 0f, 0f);
                transform.position += direction * moveSpeed * Time.deltaTime;

                isWalking = true;
            }
            else
            {
                isWalking = false;
            }

            animator.SetBool("isWalking", isWalking);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown && isAwaken && !isAttacking)
        {
            isAttacking = true; // Marca que está atacando
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
        // Verifica se já passou o cooldown para garantir ainda mais segurança
        if (Time.time < lastAttackTime) return;

        PlayAttackSound(); // Chama o som do ataque
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
        if (hitPlayer != null)
        {
            hitPlayer.GetComponent<PlayerMovement>()?.TakeDamage(attackDamage);
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
    public AudioSource audioSource;
    public AudioClip attackSound;

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
        animator.SetTrigger(inZone ? "Wake" : "Sleep");
    }

}
