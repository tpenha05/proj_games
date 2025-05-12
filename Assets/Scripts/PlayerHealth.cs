using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public GameObject[] hearts;
    public GameObject explosionFXPrefab;

    private int currentHealth;
    private Animator anim;
    private PlayerMovement playerMovement;
    private Vector3 initialPosition;

    public GameOverUIController gameOverUIController;

    void Start()
    {
        currentHealth = hearts.Length;
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        initialPosition = transform.position; // Salva a posição inicial
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        if (currentHealth <= 0) return;

        currentHealth--;

        anim.SetTrigger("Hit");

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

    public void Revive()
    {
        // Restaura a vida
        currentHealth = hearts.Length;

        // Reposiciona o jogador
        transform.position = initialPosition;

        // Reativa todos os corações
        foreach (var heart in hearts)
        {
            if (heart != null)
                heart.SetActive(true);
        }

        // Reativa o movimento
        if (playerMovement != null)
            playerMovement.enabled = true;
    }

}
