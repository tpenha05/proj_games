using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public GameObject[] hearts;
    public GameObject explosionFXPrefab;

    public int coinCount = 0;
    public static int coinCountStatic = 0;

    public TextMeshProUGUI coinText;

    private int currentHealth;
    private Animator anim;
    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;
    private Vector3 initialPosition;
    
    private bool isInvulnerable = false;
    private bool isDead = false;
    
    public float invulnerabilityTime = 1.0f;
    private float invulnerabilityTimer = 0f;

    public GameOverUIController gameOverUIController;

    void Start()
    {
        UpdateCoinText();
        currentHealth = hearts.Length;
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();
        initialPosition = transform.position;
        
        if (gameOverUIController == null)
            gameOverUIController = FindObjectOfType<GameOverUIController>();
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
            coinText.text = "" + coinCount;
    }

    public static int GetCoins()
    {
        return coinCountStatic;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void increaseMaxHealth(int amount=1)
    {
        GameObject[] newHearts = new GameObject[hearts.Length + amount];

        // Copia os corações existentes
        for (int i = 0; i < hearts.Length; i++)
        {
            newHearts[i] = hearts[i];
        }

        // Instancia novos corações e posiciona ao lado dos existentes
        for (int i = hearts.Length; i < newHearts.Length; i++)
        {
            GameObject lastHeart = newHearts[i - 1];
            Vector3 newPosition = lastHeart.transform.position + new Vector3(1f, 0, 0); // ajuste a distância se necessário

            GameObject newHeart = Instantiate(lastHeart, newPosition, Quaternion.identity, lastHeart.transform.parent);
            newHeart.SetActive(true);
            newHearts[i] = newHeart;
        }

        hearts = newHearts;
        currentHealth += amount;

        if (currentHealth > hearts.Length)
            currentHealth = hearts.Length;
    }


    public int getMaxHealth()
    {
        return hearts.Length;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    void Update()
    {
        coinCountStatic = coinCount;
        UpdateCoinText();
        
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
            
        if (playerCombat != null)
            playerCombat.enabled = false;

        StartCoroutine(ShowGameOverScreen());
    }
    
    private IEnumerator ShowGameOverScreen()
    {
        yield return new WaitForSeconds(1.5f);
        
        if (gameOverUIController != null)
        {
            int currentRunes = PlayerScore.GetRunas();
            gameOverUIController.ShowGameOver();
        }
    }
    
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

    public void AddCoins(int amount)
    {
        coinCount += amount;
        UpdateCoinText();
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
            
        if (playerCombat != null)
            playerCombat.enabled = true;

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
            
        if (playerCombat != null)
            playerCombat.enabled = true;
    }
}