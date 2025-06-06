using UnityEngine;

public class DocTrigger : MonoBehaviour
{
    private bool playerInRange = false;
    public GameObject interactCanvas;

    void Start()
    {
        interactCanvas.SetActive(false);
        DocUI.Instance.ShowPrompt(false);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            playerInRange = true;
            DocUI.Instance.ShowPrompt(true);
            interactCanvas.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            DocUI.Instance.ShowPrompt(false);
            interactCanvas.SetActive(false);
        }
    }
}
