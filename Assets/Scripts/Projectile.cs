using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeTime = 5f;
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool alreadyCollided = false;

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

        if (other.CompareTag("Player") || !other.isTrigger)
        {
            alreadyCollided = true;

            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
            col.enabled = false;

            animator.SetTrigger("hasCollided");
        }
    }

    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
