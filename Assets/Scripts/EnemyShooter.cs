using UnityEngine;
public class EnemyShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public AudioClip attackClip;
    public Transform firePoint;
    public float fireRate = 1.5f;
    public float projectileSpeed = 10f;
    public float attackRange = 8f;
    public Transform player;

    private float fireCooldown;
    private EnemyAnimatorController anim;
    private AudioSource audioSource;

    void Start()
    {
        anim = GetComponent<EnemyAnimatorController>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        // só atira se dentro do alcance, cooldown zerado E player NÃO estiver acima do firePoint
        if (distance <= attackRange 
            && fireCooldown <= 0f 
            && player.position.y <= firePoint.position.y)
        {
            anim.PlayAttack();
            Shoot();
            fireCooldown = fireRate;
        }

        fireCooldown -= Time.deltaTime;
    }

    void Shoot()
    {
        // Direção horizontal pura: +1 para direita, −1 para esquerda
        float dirX = Mathf.Sign(player.position.x - firePoint.position.x);
        Vector2 direction = new Vector2(dirX, 0f);

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * projectileSpeed;

        // Opcional: vira o sprite do projétil se for para a esquerda
        var sr = proj.GetComponent<SpriteRenderer>();
        if (sr != null) sr.flipX = dirX < 0;

        if (attackClip != null) 
            audioSource.PlayOneShot(attackClip);
    }
}
