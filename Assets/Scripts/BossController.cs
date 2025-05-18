using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public Animator animator;

    [Header("Detecção")]
    public float detectionRange = 10f;
    public float wakeUpDelay = 1.5f;
    
    [Header("Movimento")]
    public float moveSpeed = 3f;
    public float minDistanceFromPlayer = 2f;
    public float maxDistanceFromPlayer = 6f;
    public float retreatDistance = 4f;
    public float spinDuration = 2f;

    [Header("Combate")]
    public float meleeAttackRange = 3f;
    public float rangedAttackRange = 8f;
    public float superAttackRange = 6f;
    public float attackCooldown = 2f;
    public float meleeAttackChance = 0.4f;
    public float rangedAttackChance = 0.3f;
    public float superAttackChance = 0.2f;
    public float buffChance = 0.1f;
    public int superAttackHealthThreshold = 50;
    public float spinAttackCooldown = 10f;

    [Header("Estado")]
    public int maxHealth = 100;
    private int currentHealth;
    private bool isAwake = false;
    private bool isAttacking = false;
    private float lastAttackTime = 0f;
    private float lastSpinAttackTime = -10f;
    private bool isFacingRight = true;

    private enum BossState
    {
        Static,
        Waking,
        Idle,
        Moving,
        Turning,
        MeleeAttack,
        RangedAttack,
        SuperAttack,
        Spinning,
        SpinEnding,
        Buffing,
        Dead
    }

    private BossState currentState;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
            
        if (animator == null)
            animator = GetComponent<Animator>();
            
        currentHealth = maxHealth;
        currentState = BossState.Static;
    }

    void Update()
    {
        if (currentState == BossState.Static)
        {
            CheckForPlayerInRange();
            return;
        }
        
        if (currentState == BossState.Dead)
            return;
            
        if (!isAwake)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        CheckFacingDirection();
        
        switch (currentState)
        {
            case BossState.Idle:
                DecideNextAction(distanceToPlayer);
                break;
                
            case BossState.Moving:
                MoveTowardsPlayer(distanceToPlayer);
                break;
                
            case BossState.MeleeAttack:
            case BossState.RangedAttack:
            case BossState.SuperAttack:
            case BossState.Spinning:
            case BossState.SpinEnding:
            case BossState.Buffing:
            case BossState.Turning:
                break;
        }
    }

    private void CheckForPlayerInRange()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= detectionRange)
        {
            WakeUp();
        }
    }

    private void WakeUp()
    {
        currentState = BossState.Waking;
        animator.SetTrigger("wake");
        StartCoroutine(FinishWakingUp());
    }

    private IEnumerator FinishWakingUp()
    {
        yield return new WaitForSeconds(wakeUpDelay);
        isAwake = true;
        currentState = BossState.Idle;
    }

    private void CheckFacingDirection()
    {
        bool shouldFaceRight = player.position.x > transform.position.x;
        
        if (shouldFaceRight != isFacingRight && currentState != BossState.Turning && 
            currentState != BossState.Spinning && currentState != BossState.SpinEnding)
        {
            StartCoroutine(TurnTowardsPlayer(shouldFaceRight));
        }
    }

    private IEnumerator TurnTowardsPlayer(bool toRight)
    {
        if (currentState == BossState.Turning)
            yield break;
            
        currentState = BossState.Turning;
        
        if (toRight)
            animator.SetTrigger("turn_right");
        else
            animator.SetTrigger("turn_left");
            
        yield return new WaitForSeconds(0.5f);
        
        isFacingRight = toRight;
        
        if (currentState == BossState.Turning)
        {
            currentState = BossState.Idle;
        }
    }

    private void DecideNextAction(float distanceToPlayer)
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;
            
        if (Random.value < buffChance && currentHealth < maxHealth * 0.7f)
        {
            PerformBuff();
            return;
        }
            
        if (Time.time - lastSpinAttackTime > spinAttackCooldown && Random.value < 0.15f)
        {
            PerformSpinAttack();
            return;
        }
        
        if (distanceToPlayer < meleeAttackRange)
        {
            PerformMeleeAttack();
            return;
        }
        

        if (distanceToPlayer < rangedAttackRange)
        {
            float decider = Random.value;
            
            if (decider < meleeAttackChance)
            {
                StartMoving();
            }
            else if (decider < meleeAttackChance + rangedAttackChance)
            {
                PerformRangedAttack();
            }
            else if (decider < meleeAttackChance + rangedAttackChance + superAttackChance && 
                     currentHealth <= superAttackHealthThreshold)
            {
                PerformSuperAttack();
            }
            else
            {
                StartMoving();
            }
            return;
        }
        
        StartMoving();
    }
    
    private void StartMoving()
    {
        currentState = BossState.Moving;
        animator.SetBool("isMoving", true);
    }

    private void MoveTowardsPlayer(float currentDistance)
    {
        if (currentDistance <= meleeAttackRange)
        {
            StopMoving();
            PerformMeleeAttack();
            return;
        }
        
        Vector2 direction = (player.position - transform.position).normalized;
        GetComponent<Rigidbody2D>().MovePosition(Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime));
    }

    private void StopMoving()
    {
        if (currentState == BossState.Moving)
        {
            currentState = BossState.Idle;
            animator.SetBool("isMoving", false);
        }
    }

    private void PerformMeleeAttack()
    {
        currentState = BossState.MeleeAttack;
        animator.SetTrigger("melee");
        lastAttackTime = Time.time;
    }

    private void PerformRangedAttack()
    {
        currentState = BossState.RangedAttack;
        animator.SetTrigger("range");
        lastAttackTime = Time.time;
    }

    private void PerformSuperAttack()
    {
        currentState = BossState.SuperAttack;
        animator.SetTrigger("super");
        lastAttackTime = Time.time;
    }
    
    private void PerformBuff()
    {
        currentState = BossState.Buffing;
        animator.SetTrigger("buff");
        lastAttackTime = Time.time;
    }
    
    private void PerformSpinAttack()
    {
        currentState = BossState.Spinning;
        animator.SetTrigger("spin");
        lastSpinAttackTime = Time.time;
        StartCoroutine(FinishSpinAttack());
    }
    
    private IEnumerator FinishSpinAttack()
    {
        yield return new WaitForSeconds(spinDuration);
        
        if (currentState == BossState.Spinning)
        {
            currentState = BossState.SpinEnding;
            animator.SetTrigger("spin_end");
        }
    }

    public void OnAttackFinished()
    {
        if (currentState == BossState.Dead)
            return;
        
        isAttacking = false;
        
        if (currentState == BossState.MeleeAttack || 
            currentState == BossState.RangedAttack || 
            currentState == BossState.SuperAttack ||
            currentState == BossState.Buffing)
        {
            currentState = BossState.Idle;
        }
    }
    
    public void OnSpinEndFinished()
    {
        if (currentState == BossState.SpinEnding)
        {
            currentState = BossState.Idle;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
            return;
        }
    }

    private void Die()
    {
        currentState = BossState.Dead;
        animator.SetTrigger("death");
        GetComponent<Collider2D>().enabled = false;
    }
    
    public void FireProjectile()
    {
        // Chamado em um frame específico da animação de range attack
        // Implemente a lógica de disparo de projétil aqui
    }
    
    public void ApplyMeleeDamage()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= meleeAttackRange)
        {
            // player.GetComponent<PlayerHealth>().TakeDamage(1);

        }
    }
    
    public void ApplyBuffEffect()
    {

        currentHealth += maxHealth / 5;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
}