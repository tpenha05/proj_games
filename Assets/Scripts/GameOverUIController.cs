using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUIController : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TextMeshProUGUI runesText;

    void Start()
    {
        gameOverPanel.SetActive(false); // Esconde ao iniciar
    }

    public void ShowGameOver(int runes)
    {
        runesText.text = runes.ToString();
        gameOverPanel.SetActive(true);
    }

    public void Respawn()
    {
        PlayerScore.ResetRunas();

        // Oculta a UI
        gameOverPanel.SetActive(false);

        // Chama o revive do jogador
        Object.FindFirstObjectByType<PlayerHealth>()?.Revive();

    }


}
