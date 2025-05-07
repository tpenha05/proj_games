using UnityEngine;
public class EnemyShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public AudioClip attackClip;
    private AudioSource audioSource;
    public Transform firePoint;
    public float fireRate = 1.5f;
    public float projectileSpeed = 10f;
    public float attackRange = 8f;
    public Transform player;

    private float fireCooldown;
    private EnemyAnimatorController anim;

    void Start()
    {
        anim = GetComponent<EnemyAnimatorController>();
        audioSource = GetComponent<AudioSource>();
    }

    void LateUpdate()
    {
        if (!GetComponent<Animator>().enabled)
            Debug.LogError("Animator foi desativado!");
    }

    void Update()
    {
        Debug.Log($"[EnemyShooter] Ativo: {gameObject.name}, Pos: {transform.position}, Enabled: {enabled}");
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange && fireCooldown <= 0f)
        {
            anim.PlayAttack();
            Shoot();
            fireCooldown = fireRate;
        }

        fireCooldown -= Time.deltaTime;
    }

    void Shoot()
    {
        Vector2 direction = (player.position - firePoint.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * projectileSpeed;

        if (attackClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackClip);
        }
    }
}
