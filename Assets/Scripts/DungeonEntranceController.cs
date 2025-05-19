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
            SceneManager.LoadScene(nomeDaCenaDestino);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = true;
        uiCanvas.SetActive(true);
        uiText.text = "Entrar na Cave [M]";
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = false;
        uiCanvas.SetActive(false);
    }
}
