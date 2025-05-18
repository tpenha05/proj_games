using UnityEngine;
using System.Collections;

public class SpikeTrap : MonoBehaviour
{
    public float damageInterval = 1f; // Tempo entre danos consecutivos
    private bool playerOnTrap = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ApplyDamageOverTime(other.GetComponent<PlayerHealth>()));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnTrap = false;
        }
    }

    private IEnumerator ApplyDamageOverTime(PlayerHealth playerHealth)
    {
        playerOnTrap = true;

        while (playerOnTrap && playerHealth != null && playerHealth.enabled)
        {
            playerHealth.TakeDamage();
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
