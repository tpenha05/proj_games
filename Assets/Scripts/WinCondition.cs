// WinConditionController.cs
using UnityEngine;
using TMPro;

public class WinConditionController : MonoBehaviour
{
    [Header("Condição de vitória")]
    public bool isBossKilled = false;
    public GameObject youWinPanel; // painel que será ativado

    [Header("Referências")]
    public GameObject uiCanvas;
    public GameObject checkpoint;

    private TextMeshProUGUI uiText;
    private bool playerNearby;

    void Awake()
    {
        uiText = uiCanvas.GetComponentInChildren<TextMeshProUGUI>();
        uiCanvas.SetActive(false);
        checkpoint.SetActive(false);
        if (youWinPanel != null)
            youWinPanel.SetActive(false);
    }

    void Update()
    {
        checkpoint.SetActive(isBossKilled);

        if (!playerNearby) return;

        if (Input.GetKeyDown(KeyCode.M) && isBossKilled && youWinPanel != null)
            youWinPanel.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = true;
        uiCanvas.SetActive(isBossKilled);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = false;
        uiCanvas.SetActive(false);
    }
}
