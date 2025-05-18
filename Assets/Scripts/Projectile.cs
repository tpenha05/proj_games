using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeTime = 5f;
    public LayerMask groundLayer;
    
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool alreadyCollided;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (alreadyCollided) return;

        // Colisão com o chão/terreno
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            animator.SetTrigger("hasCollided");
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Colisão com o player
        if (other.CompareTag("Player"))
        {
            // Causar dano ao player
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }
            
            // Efeitos visuais e lógica de colisão
            animator.SetTrigger("hasCollided");
            alreadyCollided = true;
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
            col.enabled = false;
        }
    }

    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}