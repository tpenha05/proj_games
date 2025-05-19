using UnityEngine;
using UnityEngine.SceneManagement;

public class WinConditionUiController : MonoBehaviour
{

    public void ReturnToMainMenu()
    {
        Debug.Log("Retornando ao menu principal...");
        
        // Carrega a cena do menu principal (ajuste o índice conforme necessário)
        SceneManager.LoadScene(0);
    }
}