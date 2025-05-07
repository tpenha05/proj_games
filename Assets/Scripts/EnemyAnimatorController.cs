using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetWalking(bool isWalking)
    {
        animator.SetBool("isWalking", isWalking);
    }

    public void PlayAttack()
    {
        animator.SetTrigger("attack");
    }

    public void PlayHit()
    {
        animator.SetTrigger("hit");
    }

    public void PlayDeath()
    {
        animator.SetTrigger("die");
    }
}
