using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject mainMenuPanel;
    
    private void Start()
    {
        // Garante que o painel de tutorial começa desativado
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
            
        // Garante que o menu principal começa ativado
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }
    
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    
    public void ShowTutorial()
    {
        // Ativa o painel de tutorial e desativa o menu principal
        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);
            
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
    }
    
    public void CloseTutorial()
    {
        // Desativa o painel de tutorial e ativa o menu principal
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
            
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}