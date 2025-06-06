using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float lightAttackCooldown = 0.5f;
    [SerializeField] private float powerCooldown = 1.5f;
    [Header("Audio")]
    [SerializeField] private AudioClip[] slashSounds;
    [SerializeField] private AudioClip[] powerSlashSounds;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.1f;
    
    [Header("Ataque Aéreo")]
    [SerializeField] private float airAttackActivationDelay = 0.1f;
    [SerializeField] private float airAttackDuration = 0.2f;

    private Animator animator;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;

    private bool nextSlashIsOne = true;
    private float nextLightAttackTime = 0f;
    private float nextPowerTime = 0f;
    
    private bool attackTriggeredInAir = false;
    private string currentAirAttack = "";
    private bool wasGrounded = false;

    public AttackHitbox attackHitbox;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        
        if (playerMovement == null)
        {
            Debug.LogError("PlayerCombat: Não foi possível encontrar o componente PlayerMovement!");
        }
        
        if (playerHealth == null)
        {
            Debug.LogError("PlayerCombat: Não foi possível encontrar o componente PlayerHealth!");
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();

                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 0f;
                audioSource.volume = 0.8f;
            }
        }
    }
    
    void Update()
    {
        if (playerHealth != null && playerHealth.IsDead())
            return;
            
        HandleLightAttack();
        HandleHeavyAttack();
        
        CheckAirAttackOnLanding();
        
        wasGrounded = playerMovement != null ? playerMovement.isGrounded : false;
    }

    private void HandleLightAttack()
    {
        if (!InputManager.AttackWasPressed || Time.time < nextLightAttackTime)
            return;

        if (nextSlashIsOne)
        {
            animator.SetTrigger("Slash1");
            nextSlashIsOne = false;
            PlaySlashSound();
            
            if (playerMovement != null && !playerMovement.isGrounded)
            {
                attackTriggeredInAir = true;
                currentAirAttack = "light1";
            }
        }
        else
        {
            animator.SetTrigger("Slash2");
            nextSlashIsOne = true;
            nextLightAttackTime = Time.time + lightAttackCooldown;
            PlaySlashSound();
            
            if (playerMovement != null && !playerMovement.isGrounded)
            {
                attackTriggeredInAir = true;
                currentAirAttack = "light2";
            }
        }
    }

    private void HandleHeavyAttack()
    {
        if (InputManager.PowerWasPressed && Time.time >= nextPowerTime)
        {
            animator.SetTrigger("Power");
            nextPowerTime = Time.time + powerCooldown;
            PlayPowerSound();
            
            if (playerMovement != null && !playerMovement.isGrounded)
            {
                attackTriggeredInAir = true;
                currentAirAttack = "power";
            }
        }
    }
    
    private void CheckAirAttackOnLanding()
    {
        if (!attackTriggeredInAir)
            return;
            
        if (playerMovement != null && playerMovement.isGrounded && !wasGrounded)
        {
            StartCoroutine(ActivateAirAttackHitbox());
        }
    }
    
    private IEnumerator ActivateAirAttackHitbox()
    {
        yield return new WaitForSeconds(airAttackActivationDelay);
        
        if (currentAirAttack == "light1" || currentAirAttack == "light2")
        {
            attackHitbox.EnableLightHitbox();
        }
        else if (currentAirAttack == "power")
        {
            attackHitbox.EnablePowerHitbox();
        }
        
        yield return new WaitForSeconds(airAttackDuration);
        attackHitbox.DisableHitbox();
        
        attackTriggeredInAir = false;
        currentAirAttack = "";
    }
    
    public void EnableLightHitbox() 
    {
        attackHitbox.EnableLightHitbox();
    }
    
    public void EnablePowerHitbox() 
    {
        attackHitbox.EnablePowerHitbox();
    }
    
    public void DisableHitbox() => attackHitbox.DisableHitbox();

    private void PlaySlashSound()
    {
        if (slashSounds == null || slashSounds.Length == 0 || audioSource == null)
            return;

        AudioClip soundToPlay = slashSounds[Random.Range(0, slashSounds.Length)];

        audioSource.pitch = Random.Range(minPitch, maxPitch);

        audioSource.PlayOneShot(soundToPlay);
    }

    private void PlayPowerSound()
    {
        if (powerSlashSounds == null || powerSlashSounds.Length == 0 || audioSource == null)
            return;
            
        AudioClip soundToPlay = powerSlashSounds[Random.Range(0, powerSlashSounds.Length)];
        
        audioSource.pitch = Random.Range(minPitch * 0.9f, maxPitch * 0.9f);
        
        audioSource.PlayOneShot(soundToPlay, 1.1f);
    }
}