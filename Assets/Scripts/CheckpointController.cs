// CheckpointController.cs
using UnityEngine;
using TMPro;

public class CheckpointController : MonoBehaviour
{
    [Header("Referências")]
    // public Animator lightAnimator;
    public GameObject uiCanvas;        // Canvas/TextInteract
    public bool isSpawn = false;
    public Transform arrow;            // só para spawn

    private bool playerNearby = false;
    private Transform player;

    void Start()
    {
        // estado inicial
        // lightAnimator.Play(isSpawn ? "Shining" : "Weak");
        uiCanvas.SetActive(false);
        if (isSpawn)
            arrow.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!playerNearby) return;

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (isSpawn)
            {
                // Teleport do player para o checkpoint ativo
                var cp = CheckpointManager.I.activeCheckpoint;
                if (cp != null && cp != this)
                    player.position = cp.transform.position;
            }
            else
            {
                // Ativa este checkpoint
                CheckpointManager.I.SetActive(this);
            }
        }

        // se for spawn, faz a seta apontar
        if (isSpawn && CheckpointManager.I.activeCheckpoint != null)
        {
            Vector3 dir = (CheckpointManager.I.activeCheckpoint.transform.position 
                           - arrow.position).normalized;
            float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrow.rotation = Quaternion.Euler(0, 0, ang - 90f);
        }
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.CompareTag("Player"))
        {
            playerNearby = true;
            player = c.transform;
            uiCanvas.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D c)
    {
        if (c.CompareTag("Player"))
        {
            playerNearby = false;
            uiCanvas.SetActive(false);
        }
    }

    // Chamado pelo Manager
    public void Activate()
    {
        // lightAnimator.Play("Shining");
    }

    public void Deactivate()
    {
        // lightAnimator.Play("Weak");
    }
}
