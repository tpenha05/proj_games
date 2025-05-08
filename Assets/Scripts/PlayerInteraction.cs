using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject interactionUI;
    public GameObject bookUI;
    public TextMeshProUGUI bookText;
    [TextArea(5, 10)]
    public string textoParaMostrar = "Uma vez o Henrique Turco betou, ele perdeu. Nomeado de agora HBet, futuro a HB.";

    private bool isPlayerInZone = false;

    void Start()
    {
        if (interactionUI != null) interactionUI.SetActive(false);
        if (bookUI != null) bookUI.SetActive(false);
    }

    void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E))
        {
            bookUI.SetActive(true);
            if (bookText != null)
                bookText.text = textoParaMostrar;

            interactionUI.SetActive(false);
        }
    }

    public void FecharLivro()
    {
        if (bookUI != null)
            bookUI.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player dentro da zona");

            if (interactionUI != null)
                interactionUI.SetActive(true);
            isPlayerInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (interactionUI != null)
                interactionUI.SetActive(false);
                FecharLivro();
            isPlayerInZone = false;
        }
    }
}
