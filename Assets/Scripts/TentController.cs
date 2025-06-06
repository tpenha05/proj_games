using UnityEngine;

public class TentController : MonoBehaviour
{
    public GameObject pressETextUI; // Objeto de UI "Descansar [E]"
    public GameObject healingFXPrefab; // Prefab de brilho

    public GameObject adPanel;
    private bool isPlayerNearby = false;
    private PlayerHealth playerHealth;

    void Start()
    {
        pressETextUI.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            adPanel.SetActive(true);
            if (playerHealth != null)
            {
                if (playerHealth.getMaxHealth() == playerHealth.GetCurrentHealth())
                {
                    playerHealth.increaseMaxHealth(1);
                }
                else
                {
                    int currentHealth = playerHealth.GetCurrentHealth();
                    int max = playerHealth.getMaxHealth();
                    for (int i = currentHealth; i < max; i++)
                    {
                        playerHealth.Heal();
                    if (healingFXPrefab != null)
                    {
                        GameObject fx = Instantiate(healingFXPrefab, playerHealth.transform.position, Quaternion.identity);
                        Destroy(fx, 1.5f); // ajuste conforme a duração real do FX
                    }
                    }
                }

                pressETextUI.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                isPlayerNearby = true;
                pressETextUI.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            pressETextUI.SetActive(false);
        }
    }
}
