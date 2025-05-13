using System.Collections.Generic;
using UnityEngine;

public class SummonerBehaviour : MonoBehaviour
{
    public AudioClip summonSound;
    private AudioSource audioSource;

    public GameObject ghoulPrefab;
    public Transform spawnRadius; // filho do Summoner
    public Transform player;      // atribuído quando o player entra
    public int maxGhouls = 3;

    private Animator animator;
    private bool isPlayerInZone = false;
    private List<GameObject> spawnedGhouls = new List<GameObject>();

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource não encontrado no Summoner.");
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            player = other.transform;
            Summon();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
        }
    }

    public void Summon()
    {
        if (!isPlayerInZone || player == null || spawnedGhouls.Count >= maxGhouls)
            return;

        // Vira o spawnRadius para o lado do player
        Vector3 scale = spawnRadius.localScale;
        scale.x = player.position.x < transform.position.x ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        spawnRadius.localScale = scale;

        animator.SetTrigger("Summon");

        // Toca som
        if (audioSource != null && summonSound != null)
        {
            audioSource.PlayOneShot(summonSound);
        }
    }




    // Este método é chamado via Animation Event no fim da animação
    public void SpawnGhouls()
    {
        spawnedGhouls.RemoveAll(g => g == null);

        if (!isPlayerInZone || player == null)
            return;

        int remaining = maxGhouls - spawnedGhouls.Count;
        if (remaining <= 0)
            return;

        BoxCollider2D box = spawnRadius.GetComponent<BoxCollider2D>();
        if (box == null)
        {
            Debug.LogError("SpawnRadius precisa de um BoxCollider2D.");
            return;
        }

        // Instância temporária para medir offset do pé
        GameObject temp = Instantiate(ghoulPrefab);
        temp.SetActive(false);

        Transform foot = temp.transform.Find("GhoulFoot");
        if (foot == null)
        {
            Debug.LogError("GhoulFoot não encontrado.");
            Destroy(temp);
            return;
        }

        float footOffset = foot.position.y - temp.transform.position.y;
        Destroy(temp);

        // Topo do spawn radius
        float spawnY = spawnRadius.position.y + box.size.y * 0.5f * spawnRadius.lossyScale.y;

        // Spawna os Ghouls que faltam
        for (int i = 0; i < remaining; i++)
        {
            float halfWidth = box.size.x * 0.5f * spawnRadius.lossyScale.x;
            float centerX = spawnRadius.position.x;
            float randomX = Random.Range(centerX - halfWidth, centerX + halfWidth);
            Vector2 spawnPos = new Vector2(randomX, spawnY - footOffset);

            GameObject ghoul = Instantiate(ghoulPrefab, spawnPos, Quaternion.identity);

            EnemyAttack behaviour = ghoul.GetComponent<EnemyAttack>();
            if (behaviour != null)
            {
                behaviour.player = player;
            }

            spawnedGhouls.Add(ghoul);
        }
    }

}
