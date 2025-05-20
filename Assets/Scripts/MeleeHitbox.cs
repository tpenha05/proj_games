using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MeleeHitbox : MonoBehaviour
{
    [Header("Melee Settings")]
    [Tooltip("Damage dealt to the player on hit")]
    public int damage = 1;
    [Tooltip("Optional force to knock the player back")]
    public float knockbackForce = 8f;

    private BoxCollider2D hitbox;
    private Transform boss;

    private void Awake()
    {
        hitbox = GetComponent<BoxCollider2D>();
        hitbox.isTrigger = true;
        hitbox.enabled = false;
        boss = transform.parent;
    }

    /// <summary>
    /// Called by Animation Event: enables the melee hitbox
    /// </summary>
    public void EnableMelee()
    {
        hitbox.enabled = true;
    }

    /// <summary>
    /// Called by Animation Event: disables the melee hitbox
    /// </summary>
    public void DisableMelee()
    {
        hitbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hitbox.enabled) return;
        if (other.CompareTag("Player"))
        {
            // Get the PlayerHealth component
            var hp = other.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                // Call TakeDamage without parameters
                hp.TakeDamage();
            }

            // Apply optional knockback
            var rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float dir = Mathf.Sign(other.transform.position.x - boss.position.x);
                rb.AddForce(new Vector2(dir * knockbackForce, knockbackForce * 0.5f),
                            ForceMode2D.Impulse);
            }
        }
    }
}
