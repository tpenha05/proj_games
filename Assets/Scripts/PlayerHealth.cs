using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public GameObject[] hearts;
    public GameObject explosionFXPrefab;

    private int currentHealth;
    private Animator anim;
    private PlayerMovement playerMovement;
    private Vector3 initialPosition;
    
    private bool isInvulnerable = false;
    private bool isDead = false;
    
    public float invulnerabilityTime = 1.0f;
    private float invulnerabilityTimer = 0f;

    public GameOverUIController gameOverUIController;

    void Start()
    {
        currentHealth = hearts.Length;
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        initialPosition = transform.position;
        
        // Encontra o GameOverUIController se não foi atribuído
        if (gameOverUIController == null)
            gameOverUIController = FindObjectOfType<GameOverUIController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Revive();
        }
        
        if (isInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;
            if (invulnerabilityTimer <= 0)
            {
                isInvulnerable = false;
            }
        }
    }
    
    public void RestartAtCheckpoint()
    {
        if (CheckpointManager.I.activeCheckpoint != null)
        {
            transform.position = CheckpointManager.I.activeCheckpoint.transform.position;
        }
        else if (CheckpointManager.I.spawnCheckpoint != null)
        {
            transform.position = CheckpointManager.I.spawnCheckpoint.transform.position;
        }
    }

    public void TakeDamage()
    {
        if (isInvulnerable || isDead) return;

        if (playerMovement != null && (IsPlayerRolling() || IsPlayerDashing()))
        {
            return;
        }

        if (currentHealth <= 0) return;

        currentHealth--;

        anim.SetTrigger("Hit");

        isInvulnerable = true;
        invulnerabilityTimer = invulnerabilityTime;

        GameObject heartToRemove = hearts[currentHealth];
        if (heartToRemove != null)
        {
            Instantiate(explosionFXPrefab, heartToRemove.transform.position, Quaternion.identity);
            heartToRemove.SetActive(false);
        }

        if (currentHealth == 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        isDead = true;
        anim.SetTrigger("Death");
        
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Mostra a tela de Game Over após um pequeno delay para a animação de morte
        StartCoroutine(ShowGameOverScreen());
    }
    
    private IEnumerator ShowGameOverScreen()
    {
        yield return new WaitForSeconds(1.5f); // Tempo para a animação de morte tocar
        
        // Mostra a tela de Game Over com a quantidade de runas do jogador
        if (gameOverUIController != null)
        {
            int currentRunes = PlayerScore.GetRunas(); // Supondo que esta função exista
            gameOverUIController.ShowGameOver(currentRunes);
        }
    }

    // Método removido pois não reiniciamos mais a cena
    // private IEnumerator RestartScene()
    // {
    //     yield return new WaitForSeconds(1.5f);
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    // }
    
    private bool IsPlayerRolling()
    {
        System.Reflection.FieldInfo isRollingField =
            typeof(PlayerMovement).GetField("isRolling",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

        if (isRollingField != null)
        {
            return (bool)isRollingField.GetValue(playerMovement);
        }

        return false;
    }
    
    private bool IsPlayerDashing()
    {
        System.Reflection.FieldInfo isDashingField = 
            typeof(PlayerMovement).GetField("isDashing", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
                
        if (isDashingField != null)
        {
            return (bool)isDashingField.GetValue(playerMovement);
        }
        
        return false;
    }

    public void Revive()
    {
        isDead = false;
        currentHealth = hearts.Length;

        if (CheckpointManager.I.activeCheckpoint != null)
        {
            transform.position = CheckpointManager.I.activeCheckpoint.transform.position;
        }
        else if (CheckpointManager.I.spawnCheckpoint != null)
        {
            transform.position = CheckpointManager.I.spawnCheckpoint.transform.position;
        }

        foreach (var heart in hearts)
        {
            if (heart != null)
                heart.SetActive(true);
        }

        if (playerMovement != null)
            playerMovement.enabled = true;

        isInvulnerable = false;
    }
    
    public void Heal()
    {
        if (currentHealth == hearts.Length)
            return;

        currentHealth = hearts.Length;

        foreach (var heart in hearts)
        {
            if (heart != null)
                heart.SetActive(true);
        }

        if (playerMovement != null)
            playerMovement.enabled = true;
    }
}