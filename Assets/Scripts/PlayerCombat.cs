using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float lightAttackCooldown = 0.5f;
    [SerializeField] private float powerCooldown       = 1.5f;

    private Animator    animator;
    private Rigidbody2D rb;

    private bool  nextSlashIsOne      = true;
    private float nextLightAttackTime = 0f;
    private float nextPowerTime       = 0f;

    public AttackHitbox attackHitbox;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb       = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleLightAttack();
        HandleHeavyAttack();
    }

    private void HandleLightAttack()
    {
        if (!InputManager.AttackWasPressed || Time.time < nextLightAttackTime)
            return;

        if (nextSlashIsOne)
        {
            animator.SetTrigger("Slash1");
            nextSlashIsOne = false;
        }
        else
        {
            animator.SetTrigger("Slash2");
            nextSlashIsOne      = true;
            nextLightAttackTime = Time.time + lightAttackCooldown;
        }
    }

    private void HandleHeavyAttack()
    {
        if (InputManager.PowerWasPressed && Time.time >= nextPowerTime)
        {
            animator.SetTrigger("Power");
            nextPowerTime = Time.time + powerCooldown;
        }
    }
    public void EnableLightHitbox()  => attackHitbox.EnableLightHitbox();
    public void EnablePowerHitbox()  => attackHitbox.EnablePowerHitbox();
    public void DisableHitbox()      => attackHitbox.DisableHitbox();
}
