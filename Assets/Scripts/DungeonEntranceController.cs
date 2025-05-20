using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DungeonEntranceController : MonoBehaviour
{
    [Header("UI")]
    public GameObject uiCanvas;
    private TextMeshProUGUI uiText;

    [Header("Configuração")]
    public string nomeDaCenaDestino;

    [Header("Requisitos")]
    [Tooltip("Quantidade mínima de runas necessárias para entrar")]
    public int runasNecessarias = 10;

    private bool playerNearby = false;

    void Awake()
    {
        uiText = uiCanvas.GetComponentInChildren<TextMeshProUGUI>();
        uiCanvas.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.M))
        {
            int runasAtuais = PlayerHealth.GetCoins(); // Supondo que você tenha uma variável estática para runas no PlayerHealth
            if (runasAtuais >= runasNecessarias)
            {
                SceneManager.LoadScene(nomeDaCenaDestino);
            }
            else
            {
                uiText.text = $"Você tem {runasAtuais} runas (precisa de {runasNecessarias})";
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = true;
        uiCanvas.SetActive(true);

        int runasAtuais = PlayerHealth.GetCoins(); // Supondo que você tenha um método para obter runas no PlayerHealth
        if (runasAtuais >= runasNecessarias)
            uiText.text = "Entrar na Cave [M]";
        else
            uiText.text = $"Você tem {runasAtuais} runas (precisa de {runasNecessarias})";
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = false;
        uiCanvas.SetActive(false);
    }
}
