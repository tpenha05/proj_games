using System.Collections;
using UnityEngine;
using TMPro;

public class CheckpointController : MonoBehaviour
{
    [Header("Sprites e tempo de animação")]
    public Sprite offSprite;
    public Sprite[] activateSprites;
    public Sprite[] onSprites;
    public float frameTime = 0.1f;

    [Header("Referências")]
    public bool isSpawn = false;
    public Transform arrow;
    public GameObject uiCanvas;
    public GameObject interactCanvas;

    private TextMeshProUGUI uiText;
    private SpriteRenderer sr;
    private Coroutine idleLoopCoroutine;
    private bool playerNearby;
    private Transform player;
    private string lastUIText = "";

    [Header("Debug")]
    [ReadOnly] public bool isActiveCheckpoint;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        uiText = uiCanvas.GetComponentInChildren<TextMeshProUGUI>();
        uiCanvas.SetActive(false);
        interactCanvas.SetActive(false);

        if (isSpawn && arrow != null)
            arrow.gameObject.SetActive(false);
    }

    void Start()
    {
        if (isSpawn)
        {
            CheckpointManager.I.RegisterSpawn(this);
        }
        else
        {
            sr.sprite = offSprite;
        }
    }

    void Update()
    {
        if (!playerNearby) return;

        UpdateUIText();
        UpdateArrowDirection();
    }

    public void OnInteractButtonPressed()
    {
        Debug.Log("Botão clicado!");
        
        if (isSpawn)
        {
            if (CheckpointManager.I.activeCheckpoint != null)
                player.position = CheckpointManager.I.activeCheckpoint.transform.position;
        }
        else
        {
            if (CheckpointManager.I.activeCheckpoint == this)
                player.position = CheckpointManager.I.spawnCheckpoint.transform.position;
            else
                CheckpointManager.I.SetActive(this);
        }
    }

    private void UpdateUIText()
    {
        if (!uiCanvas.activeSelf) return;

        string desiredText = "";

        if (isSpawn)
        {
            desiredText = (CheckpointManager.I.activeCheckpoint != null)
                ? "Teleportar"
                : "Necessita de um totem ativado";
        }
        else
        {
            desiredText = (CheckpointManager.I.activeCheckpoint == this)
                ? "Teleportar"
                : "Ativar";
        }

        if (desiredText != lastUIText)
        {
            uiText.text = desiredText;
            lastUIText = desiredText;
        }
    }

    private void UpdateArrowDirection()
    {
        if (!isSpawn || arrow == null) return;

        var active = CheckpointManager.I.activeCheckpoint;

        if (active != null)
        {
            arrow.gameObject.SetActive(true);
            Vector3 dir = (active.transform.position - arrow.position).normalized;
            float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrow.rotation = Quaternion.Euler(0, 0, ang - 90f);
        }
        else
        {
            arrow.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = true;
        player = other.transform;
        uiCanvas.SetActive(true);
        interactCanvas.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = false;
        uiCanvas.SetActive(false);
        interactCanvas.SetActive(false);
    }

    public void ForceOn()
    {
        StopAllCoroutines();
        sr.sprite = onSprites.Length > 0 ? onSprites[0] : offSprite;
        idleLoopCoroutine = StartCoroutine(LoopSprites(onSprites, true));
    }

    public void ForceOff()
    {
        StopAllCoroutines();
        sr.sprite = offSprite;
    }

    public void PlayActivateAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(ActivateRoutine());
    }

    private IEnumerator ActivateRoutine()
    {
        yield return LoopSprites(activateSprites, false);
        idleLoopCoroutine = StartCoroutine(LoopSprites(onSprites, true));
    }

    private IEnumerator LoopSprites(Sprite[] sprites, bool loop)
    {
        if (sprites == null || sprites.Length == 0) yield break;

        do
        {
            foreach (var s in sprites)
            {
                sr.sprite = s;
                yield return new WaitForSeconds(frameTime);
            }
        } while (loop);
    }
}