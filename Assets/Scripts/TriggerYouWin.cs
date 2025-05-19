using UnityEngine;

public class TriggerYouWin : MonoBehaviour
{
    public GameObject youWinCanvas;

    private void Start()
    {
        if (youWinCanvas != null)
        {
            youWinCanvas.SetActive(false); // Garante que começa desativado
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && youWinCanvas != null)
        {
            youWinCanvas.SetActive(true);
            Time.timeScale = 0f; // Pausa o jogo (opcional)
        }
    }
}
