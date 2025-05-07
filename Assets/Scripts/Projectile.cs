using UnityEngine;
public class Projectile : MonoBehaviour
{
    public float lifeTime = 5f;
    public LayerMask groundLayer;    // atribua no Inspector ao layer “Ground”

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

        // Colidiu com chão/tilemap?
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            Destroy(gameObject);
            return;
        }

        // Colidiu com player?
        if (other.CompareTag("Player"))
        {
            alreadyCollided = true;
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
            col.enabled = false;
            animator.SetTrigger("hasCollided");
            // aqui você pode chamar dano no player...
        }
    }

    // Chamado pela animação “hasCollided” no fim dela
    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
