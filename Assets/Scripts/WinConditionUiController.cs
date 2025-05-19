using UnityEngine;
using UnityEngine.SceneManagement;

public class WinConditionUiController : MonoBehaviour
{

    public void ReturnToMainMenu()
    {
        Debug.Log("Retornando ao menu principal...");
        
        SceneManager.LoadScene(0);
    }
}