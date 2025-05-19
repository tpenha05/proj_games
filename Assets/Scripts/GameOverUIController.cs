using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUIController : MonoBehaviour
{
    public GameObject gameOverPanel;
    
    // Botões adicionais (se quiser)
    public Button respawnButton;
    public Button mainMenuButton;

    void Start()
    {
        gameOverPanel.SetActive(false); // Esconde ao iniciar
        
        // Configura os botões adicionais se existirem
        if (respawnButton != null)
            respawnButton.onClick.AddListener(Respawn);
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        
        // Opcional: pausar o jogo
        Time.timeScale = 0f;
    }

    public void Respawn()
    {
        // Retoma o jogo se estiver pausado
        Time.timeScale = 1f;
        

        // Oculta a UI
        gameOverPanel.SetActive(false);

        // Chama o revive do jogador
        FindObjectOfType<PlayerHealth>()?.Revive();
    }
    
    public void ReturnToMainMenu()
    {
        // Retoma o time scale para não afetar o menu
        Time.timeScale = 1f;
        
        // Carrega a cena do menu principal (ajuste o índice conforme necessário)
        SceneManager.LoadScene(0);
    }
}