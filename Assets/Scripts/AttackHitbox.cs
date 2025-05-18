using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackHitbox : MonoBehaviour
{
    [Header("Parâmetros de Dano")]
    [SerializeField] private int lightDamage = 1;
    [SerializeField] private int powerDamage = 3;

    private int      currentDamage;
    private Collider2D hitbox;

    void Awake()
    {
        hitbox = GetComponent<Collider2D>();
        hitbox.enabled = false;
    }

    public void EnableLightHitbox()
    {
        currentDamage = lightDamage;
        hitbox.enabled = true;
    }

    public void EnablePowerHitbox()
    {
        currentDamage = powerDamage;
        hitbox.enabled = true;
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
    }
}
