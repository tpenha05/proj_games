using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AdController : MonoBehaviour
{
    public GameObject adPanel;


    void Start()
    {
        adPanel.SetActive(false); // Esconde ao iniciar

    }



    public void Respawn_After_AD()
    {
        // Retoma o jogo se estiver pausado
        Time.timeScale = 1f;


        // Oculta a UI
        adPanel.SetActive(false);

        // Chama o revive do jogador
        FindObjectOfType<PlayerHealth>()?.Revive();
    }

    public void CloseAd()
    {
        // Retoma o jogo se estiver pausado
        Time.timeScale = 1f;

        // Volta para o menu principal
        adPanel.SetActive(false);
    }
    
    public void WatchAdRevive()
    {
        // Retoma o jogo se estiver pausado
        Time.timeScale = 1f;

        // Oculta a UI
        adPanel.SetActive(true);

        // Chama o revive do jogador
        FindObjectOfType<PlayerHealth>()?.Revive();
    }


}