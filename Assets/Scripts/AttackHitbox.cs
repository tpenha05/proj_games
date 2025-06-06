using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackHitbox : MonoBehaviour
{
    [Header("Parâmetros de Dano")]
    [SerializeField] private int lightDamage = 1;
    [SerializeField] private int powerDamage = 3;

    private int currentDamage;
    private Collider2D hitbox;
    private PlayerMovement playerMovement;

    void Awake()
    {
        hitbox = GetComponent<Collider2D>();
        hitbox.enabled = false;
        
        // Obter referência ao PlayerMovement
        playerMovement = GetComponentInParent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("AttackHitbox: Não foi possível encontrar o componente PlayerMovement no parent!");
        }
    }

    public void EnableLightHitbox()
    {
        // Só ativamos a hitbox se estiver no chão
        if (playerMovement != null && playerMovement.isGrounded)
        {
            currentDamage = lightDamage;
            hitbox.enabled = true;
        }
    }

    public void EnablePowerHitbox()
    {
        // Só ativamos a hitbox se estiver no chão
        if (playerMovement != null && playerMovement.isGrounded)
        {
            currentDamage = powerDamage;
            hitbox.enabled = true;
        }
    }

    public void DisableHitbox()
    {
        hitbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hitbox.enabled) return;
        if (!other.CompareTag("Enemy")) return;

        var eh = other.GetComponent<EnemyHealth>();
        if (eh != null)
            eh.TakeDamage(currentDamage);

        // Pega diretamente o seu boss
        var boss = other.GetComponent<TarnishedWidowBoss>();
        if (boss != null)
            boss.TakeDamage(currentDamage);
    }
}