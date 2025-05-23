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
    }

    void OnDisable()
    {
    }

    public void TakeDamage(int amount)
    {
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

public GameObject coinPrefab; // Prefab da moeda a ser dropada
public int coinAmount = 5;    // Quantidade de moedas dropadas

    private void Die()
    {
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

        DropCoins();

        Destroy(gameObject, 1.5f);
    }

    private void DropCoins()
    {
        for (int i = 0; i < coinAmount; i++)
        {
            Vector2 randomOffset = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
            Instantiate(coinPrefab, (Vector2)transform.position + randomOffset, Quaternion.identity);
        }
    }
    public bool IsDead()
    {
        return isDead;
    }


}
