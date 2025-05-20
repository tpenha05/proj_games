using UnityEngine;

/// <summary>
/// Faz a língua do boss causar dano quando o hitbox estiver ativo.
/// A ativação/desativação é chamada por eventos na animação.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class TongueHitbox : MonoBehaviour
{
    [Header("Dano")]
    [Tooltip("Quanto de vida o jogador perde ao ser atingido")]
    public int damage = 1;

    [Tooltip("Força extra para empurrar o player")]
    public float knockbackForce = 8f;

    // Cache
    BoxCollider2D hitboxCol;
    Transform boss;          // transform.parent é o boss

    void Awake()
    {
        hitboxCol = GetComponent<BoxCollider2D>();
        hitboxCol.enabled = false;       // começa desligado
        boss = transform.parent;
    }

    /* ---------- Eventos chamados pela animação ---------- */
    /// <summary>Liga o collider para começar a causar dano.</summary>
    public void EnableHitbox()  => hitboxCol.enabled = true;

    /// <summary>Desliga o collider quando o golpe terminar.</summary>
    public void DisableHitbox() => hitboxCol.enabled = false;

    /* ---------- Lógica de dano ---------- */
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hitboxCol.enabled) return;

        if (other.CompareTag("Player"))
        {
            // 1) Aplica dano
            other.GetComponent<PlayerHealth>()?.TakeDamage();   // ← sem argumento

            // 2) Knock-back opcional (mantido igual)
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb)
            {
                float dir = Mathf.Sign(other.transform.position.x - boss.position.x);
                rb.AddForce(new Vector2(dir * knockbackForce, knockbackForce * 0.5f),
                            ForceMode2D.Impulse);
            }
        }
    }

}
