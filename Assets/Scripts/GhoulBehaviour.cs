using System;
using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Transform player;
    public Transform attackPoint; // Novo: ponto de origem do ataque
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float attackDelay = 0.5f;
    public int attackDamage = 1; // Dano do ataque

    private float lastAttackTime;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null || attackPoint == null) return;

        float distance = Vector2.Distance(attackPoint.position, player.position);

        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            animator.SetTrigger("Attack");
        }

        // Vira para o lado do player
        Vector3 scale = transform.localScale;
        if (player.position.x < transform.position.x)
            scale.x = -Mathf.Abs(scale.x); // olha para esquerda
        else
            scale.x = Mathf.Abs(scale.x);  // olha para direita
        transform.localScale = scale;
    }


   void PerformAttack()
    {
        
        float distance = Vector2.Distance(attackPoint.position, player.position);
        if (distance <= attackRange)
        {
            
            player.GetComponent<PlayerMovement>().TakeDamage(attackDamage);
        }
        else
        {
            Debug.Log("Player esquivou do ataque!");
        }
    }


    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
