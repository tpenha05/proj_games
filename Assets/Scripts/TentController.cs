using UnityEngine;

public class TentController : MonoBehaviour
{
    public GameObject pressETextUI; // Objeto de UI "Descansar [E]"
    public GameObject healingFXPrefab; // Prefab de brilho
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
            if (playerHealth != null)
            {
                playerHealth.Heal();

                if (healingFXPrefab != null)
                {
                    GameObject fx = Instantiate(healingFXPrefab, playerHealth.transform.position, Quaternion.identity);
                    Destroy(fx, 1.5f); // ajuste conforme a duração real do FX
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
