using UnityEngine;

[RequireComponent(typeof(EnemyAnimatorController))]
public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    private EnemyAnimatorController anim;
    private Collider2D col;
    private Rigidbody2D rb;
    private bool isDead = false;
    public AudioClip hitClip;
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<EnemyAnimatorController>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnDestroy()
    {
        Debug.LogWarning($"[OnDestroy] Inimigo destruído! isDead: {isDead}, currentHealth: {currentHealth}");
    }
    
    void OnDisable()
    {
        Debug.LogWarning("Enemy foi DESATIVADO, não destruído.");
    }

    public void TakeDamage(int amount)
    {
        Debug.Log("Enemy took damage: " + amount);
        if (isDead) return;

        currentHealth -= amount;
        anim.PlayHit();

        if (hitClip != null)
        {
            audioSource.PlayOneShot(hitClip);
        }


        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died");
        isDead = true;
        anim.PlayDeath();

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        col.enabled = false;

        var spitter = GetComponent<WalkingSpitter>();
        if (spitter != null)
            spitter.enabled = false;

        var shooter = GetComponent<EnemyShooter>();
        if (shooter != null)
            shooter.enabled = false;
        Destroy(gameObject, 1.2f);
    }


}
