using UnityEngine;
using System.Collections;

public class SpikeTrap : MonoBehaviour
{
    public float damageDelay = 1f;       // Tempo antes do primeiro dano
    public float damageInterval = 1f;    // Intervalo entre danos após o primeiro
    public int damageAmount = 1;

    private bool playerOnTrap = false;
    private Coroutine damageCoroutine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && damageCoroutine == null)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                damageCoroutine = StartCoroutine(StartDamageAfterDelay(playerHealth));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnTrap = false;

            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator StartDamageAfterDelay(PlayerHealth playerHealth)
    {
        playerOnTrap = true;

        // Espera antes do primeiro dano
        yield return new WaitForSeconds(damageDelay);

        // Começa o dano contínuo
        while (playerOnTrap && playerHealth != null && playerHealth.enabled)
        {
            playerHealth.TakeDamage();
            yield return new WaitForSeconds(damageInterval);
        }

        damageCoroutine = null;
    }
}
