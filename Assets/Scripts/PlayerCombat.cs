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

    private bool nextSlashIsOne = true;
    private float nextLightAttackTime = 0f;
    private float nextPowerTime = 0f;
    
    // Variáveis para controlar ataques aéreos
    private bool attackTriggeredInAir = false;
    private string currentAirAttack = "";
    private bool wasGrounded = false;

    public AttackHitbox attackHitbox;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        
        if (playerMovement == null)
        {
            Debug.LogError("PlayerCombat: Não foi possível encontrar o componente PlayerMovement!");
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
        HandleLightAttack();
        HandleHeavyAttack();
        
        // Detectar quando o jogador pousa após um ataque aéreo
        CheckAirAttackOnLanding();
        
        // Guardar estado do grounded para o próximo frame
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
            
            // Se estamos no ar, registramos o ataque para quando pousar
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
            
            // Se estamos no ar, registramos o ataque para quando pousar
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
            
            // Se estamos no ar, registramos o ataque para quando pousar
            if (playerMovement != null && !playerMovement.isGrounded)
            {
                attackTriggeredInAir = true;
                currentAirAttack = "power";
            }
        }
    }
    
    private void CheckAirAttackOnLanding()
    {
        // Se não há ataque registrado no ar, pule a verificação
        if (!attackTriggeredInAir)
            return;
            
        // Verificamos se acabamos de pousar (transição de !grounded para grounded)
        if (playerMovement != null && playerMovement.isGrounded && !wasGrounded)
        {
            // Ativamos a hitbox com um pequeno atraso para parecer mais natural
            StartCoroutine(ActivateAirAttackHitbox());
        }
    }
    
    private IEnumerator ActivateAirAttackHitbox()
    {
        // Pequeno atraso antes de ativar a hitbox após pouso
        yield return new WaitForSeconds(airAttackActivationDelay);
        
        // Ativamos a hitbox apropriada baseada no tipo de ataque
        if (currentAirAttack == "light1" || currentAirAttack == "light2")
        {
            attackHitbox.EnableLightHitbox();
        }
        else if (currentAirAttack == "power")
        {
            attackHitbox.EnablePowerHitbox();
        }
        
        // Desativamos a hitbox após a duração do ataque
        yield return new WaitForSeconds(airAttackDuration);
        attackHitbox.DisableHitbox();
        
        // Resetamos as flags de ataque aéreo
        attackTriggeredInAir = false;
        currentAirAttack = "";
    }
    
    // Métodos chamados pela animação (eventos de animação)
    public void EnableLightHitbox() 
    {
        // A verificação de grounded está sendo feita dentro do AttackHitbox
        attackHitbox.EnableLightHitbox();
    }
    
    public void EnablePowerHitbox() 
    {
        // A verificação de grounded está sendo feita dentro do AttackHitbox
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