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

        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            animator.SetTrigger("hasCollided");
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Player"))
        {
            animator.SetTrigger("hasCollided");
            alreadyCollided = true;
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
            col.enabled = false;
            // aqui você pode chamar dano no player...
        }
    }

    public void DestroyProjectile()
    {
        Debug.Log("Destroying projectile");
        Destroy(gameObject);
    }
}
