using UnityEngine;

public class RangeTrigger : MonoBehaviour
{
    private EnemyAttack parent;

    void Start()
    {
        parent = GetComponentInParent<EnemyAttack>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parent.SetPlayerInZone(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parent.SetPlayerInZone(false);
        }
    }
}
