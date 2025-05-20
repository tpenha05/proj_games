using System.Collections;
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
    private SpriteRenderer sr;
    private Coroutine idleLoopCoroutine;
    private bool playerNearby;
    private Transform player;

    [Header("Debug")]
    [ReadOnly] public bool isActiveCheckpoint;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        uiText = uiCanvas.GetComponentInChildren<TextMeshProUGUI>();
        uiCanvas.SetActive(false);

    }



    void Update()
    {

        if (isBossKilled)
        {
            Debug.Log("Boss killed");
            checkpoint.SetActive(true);
        }
        else
        {
            checkpoint.SetActive(false);
        }
        if (!playerNearby) return;
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (isBossKilled && youWinPanel != null)
            {
                youWinPanel.SetActive(true);
            }
        }
    }

    


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = true;
        player = other.transform;

        // Só mostra o canvas se o boss tiver sido derrotado
        if (isBossKilled)
            uiCanvas.SetActive(true);
    }


    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = false;
        uiCanvas.SetActive(false);
    }

    public void ForceOn()
    {
        StopAllCoroutines();
    }

    public void ForceOff()
    {
        StopAllCoroutines();
    }





}
