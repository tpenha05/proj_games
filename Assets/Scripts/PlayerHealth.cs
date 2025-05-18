using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public GameObject[] hearts;
    public GameObject explosionFXPrefab;

    private int currentHealth;
    private Animator anim;
    private PlayerMovement playerMovement;
    private Vector3 initialPosition;
    
    private bool isInvulnerable = false;
    
    public float invulnerabilityTime = 1.0f;
    private float invulnerabilityTimer = 0f;

    public GameOverUIController gameOverUIController;

    void Start()
    {
        currentHealth = hearts.Length;
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        initialPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage();
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

    public void TakeDamage()
    {
        if (isInvulnerable) return;
        
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
            anim.SetTrigger("Death");
            if (playerMovement != null)
                playerMovement.enabled = false;

            gameOverUIController.ShowGameOver(PlayerScore.runas);
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
        currentHealth = hearts.Length;

        transform.position = initialPosition;

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