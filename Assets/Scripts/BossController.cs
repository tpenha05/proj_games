using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Projétil")]
    public ProjectilePoint firePoint;
    
    [Header("Referências")]
    public Transform player;
    public Animator animator;
    public Transform spriteTransform;

    [Header("Detecção")]
    public float detectionRange = 10f;
    public float wakeUpDelay = 1.5f;
    
    [Header("Movimento")]
    public float moveSpeed = 3f;
    public float minDistanceFromPlayer = 2f;
    public float maxDistanceFromPlayer = 6f;
    public float retreatDistance = 4f;
    public bool flipSpriteOnTurn = true;

    [Header("Combate")]
    public float rangedAttackRange = 8f;
    public float superAttackRange = 6f;
    public float attackCooldown = 2f;
    public float rangedAttackChance = 0.6f;
    public float superAttackChance = 0.3f;
    public float buffChance = 0.1f;
    public int superAttackHealthThreshold = 50;

    [Header("Estado")]
    public int maxHealth = 100;
    private int currentHealth;
    private bool isAwake = false;
    private bool isAttacking = false;
    private float lastAttackTime = 0f;
    public bool isFacingRight = true; // Alterado para público para acesso pelo ProjectilePoint
    private bool isActionCooldown = false;
    private float idleTimer = 0f;
    private float maxIdleTime = 2f;
    private bool forceNewAction = false;
    private float stateTimer = 0f;
    private string currentAnimationState = "";

    private enum BossState
    {
        Static,
        Waking,
        Idle,
        Moving,
        Turning,
        RangedAttack,
        SuperAttack,
        Buffing,
        Dead
    }

    private BossState currentState;
    private Rigidbody2D rb;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
            
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (spriteTransform == null)
            spriteTransform = transform;
        
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        currentState = BossState.Static;
        
        // Verificar referência do firePoint
        if (firePoint == null)
        {
            Debug.LogError("FirePoint não atribuído no inspetor!");
        }
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
        
        stateTimer += Time.deltaTime;
        
        // Monitorar a animação atual
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        foreach (string stateName in new[] {"idle", "range", "super", "buff", "moving", "turn_left", "turn_right", "death", "wake"})
        {
            if (stateInfo.IsName(stateName))
            {
                if (currentAnimationState != stateName)
                {
                    currentAnimationState = stateName;
                    Debug.Log("Animação mudou para: " + currentAnimationState);
                }
                break;
            }
        }
        
        // Verificar se está na animação de range attack e garantir que o projétil seja ativado
        if (currentState == BossState.RangedAttack)
        {
            if (stateInfo.IsName("range") && stateInfo.normalizedTime > 0.5f && 
                firePoint != null && !firePoint.isActive)
            {
                Debug.Log("Forçando ativação do projétil em normalizedTime: " + stateInfo.normalizedTime);
                FireProjectile();
            }
        }
        
        switch (currentState)
        {
            case BossState.Idle:
                idleTimer += Time.deltaTime;
                if (idleTimer > maxIdleTime || forceNewAction)
                {
                    forceNewAction = false;
                    idleTimer = 0f;
                    DecideNextAction(distanceToPlayer);
                }
                else if (!isActionCooldown)
                {
                    DecideNextAction(distanceToPlayer);
                }
                break;
                
            case BossState.Moving:
                MoveTowardsPlayer(distanceToPlayer);
                break;
                
            case BossState.RangedAttack:
            case BossState.SuperAttack:
            case BossState.Buffing:
            case BossState.Turning:
                idleTimer += Time.deltaTime;
                if (idleTimer > 3f)
                {
                    ForceReturnToIdle();
                }
                break;
        }
    }

    void LateUpdate()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        // Verificar se a animação e o estado lógico estão em sincronia
        if (stateInfo.IsName("idle") && currentState != BossState.Idle && stateTimer > 1.0f)
        {
            Debug.Log("Animação de Idle tocando mas estado não é idle!");
            if (!isActionCooldown)
            {
                forceNewAction = true;
            }
        }
        
        // Verificar se está preso em transição por muito tempo
        if (animator.IsInTransition(0) && stateTimer > 2.0f)
        {
            Debug.Log("Animador preso em transição por tempo demais!");
            ForceReturnToIdle();
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
        stateTimer = 0f;
    }

    private void CheckFacingDirection()
    {
        bool shouldFaceRight = player.position.x > transform.position.x;
        
        if (shouldFaceRight != isFacingRight && currentState != BossState.Turning)
        {
            StartCoroutine(TurnTowardsPlayer(shouldFaceRight));
        }
    }

    private IEnumerator TurnTowardsPlayer(bool toRight)
    {
        if (currentState == BossState.Turning)
            yield break;
            
        currentState = BossState.Turning;
        stateTimer = 0f;
        
        ResetAllTriggers();
        
        if (toRight)
            animator.SetTrigger("turn_right");
        else
            animator.SetTrigger("turn_left");
            
        yield return new WaitForSeconds(0.5f);
        
        isFacingRight = toRight;
        UpdateSpriteDirection(isFacingRight);
        
        if (currentState == BossState.Turning)
        {
            currentState = BossState.Idle;
            stateTimer = 0f;
        }
    }

    public void UpdateSpriteDirection(bool facingRight)
    {
        if (flipSpriteOnTurn)
        {
            Vector3 scale = spriteTransform.localScale;
            scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            spriteTransform.localScale = scale;
        }
    }

    public void FlipSprite()
    {
        UpdateSpriteDirection(isFacingRight);
    }

    private void ResetAllTriggers()
    {
        animator.ResetTrigger("wake");
        animator.ResetTrigger("range");
        animator.ResetTrigger("super");
        animator.ResetTrigger("buff");
        animator.ResetTrigger("turn_right");
        animator.ResetTrigger("turn_left");
        animator.ResetTrigger("death");
    }

    private void ForceReturnToIdle()
    {
        Debug.Log("Forçando retorno para idle - animação travada");
        idleTimer = 0f;
        stateTimer = 0f;
        currentState = BossState.Idle;
        isAttacking = false;
        isActionCooldown = false;
        forceNewAction = true;
        
        ResetAllTriggers();
        animator.SetBool("isMoving", false);
        
        // Garante que está usando o nome correto do estado idle (com 'i' minúsculo)
        animator.Play("idle", 0, 0f);
        
        rb.linearVelocity = Vector2.zero;
        
        // Se o projétil estiver ativo, desative-o
        if (firePoint != null && firePoint.isActive)
        {
            firePoint.Deactivate();
        }
    }

    private void DecideNextAction(float distanceToPlayer)
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;
        
        Debug.Log("Decidindo próxima ação. Distância: " + distanceToPlayer);
            
        if (Random.value < buffChance && currentHealth < maxHealth * 0.7f)
        {
            PerformBuff();
            return;
        }
        
        if (distanceToPlayer < rangedAttackRange)
        {
            float decider = Random.value;
            
            if (decider < rangedAttackChance)
            {
                PerformRangedAttack();
            }
            else if (decider < rangedAttackChance + superAttackChance && 
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
        stateTimer = 0f;
        idleTimer = 0f;
        ResetAllTriggers();
        animator.SetBool("isMoving", true);
    }

    private void MoveTowardsPlayer(float currentDistance)
    {
        // Manter distância ideal para ataques
        if (currentDistance <= rangedAttackRange * 0.7f)
        {
            StopMoving();
            PerformRangedAttack();
            return;
        }
        
        Vector2 direction = (player.position - transform.position).normalized;
        
        if (currentDistance < minDistanceFromPlayer)
        {
            rb.linearVelocity = -direction * moveSpeed;
        }
        else if (currentDistance > maxDistanceFromPlayer)
        {
            rb.linearVelocity = direction * moveSpeed;
        }
        else if (Random.value < 0.01f)
        {
            StopMoving();
            
            float attackChoice = Random.value;
            if (attackChoice < 0.7f)
                PerformRangedAttack();
            else
                PerformSuperAttack();
        }
        else
        {
            rb.linearVelocity = direction * moveSpeed;
        }
    }

    private void StopMoving()
    {
        if (currentState == BossState.Moving)
        {
            rb.linearVelocity = Vector2.zero;
            currentState = BossState.Idle;
            stateTimer = 0f;
            idleTimer = 0f;
            animator.SetBool("isMoving", false);
        }
    }

    private void PerformRangedAttack()
    {
        StopAllCoroutines();
        
        currentState = BossState.RangedAttack;
        stateTimer = 0f;
        idleTimer = 0f;
        
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);
        
        ResetAllTriggers();
        animator.SetTrigger("range");
        
        lastAttackTime = Time.time;
        
        // Adicione esta linha para prevenir interrupção
        StartCoroutine(PreventRangeAttackInterruption());
        
        StartCoroutine(ActionCooldown(0.8f));
    }
    
    private IEnumerator PreventRangeAttackInterruption()
    {
        bool attackAnimationStarted = false;
        float waitTime = 0f;
        
        // Esperar até que a animação de ataque comece
        while (!attackAnimationStarted && waitTime < 0.5f)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("range"))
            {
                attackAnimationStarted = true;
            }
            waitTime += Time.deltaTime;
            yield return null;
        }
        
        if (attackAnimationStarted)
        {
            // Obter a duração da animação e esperar quase até o fim
            float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
            Debug.Log("Duração da animação de range attack: " + animationLength);
            
            // Esperar até um ponto específico da animação (70% do caminho)
            yield return new WaitForSeconds(animationLength * 0.7f);
            
            // Se ainda não disparou, forçar o disparo
            if (firePoint != null && !firePoint.isActive)
            {
                Debug.Log("Forçando ativação do projétil no final da corrotina de prevenção");
                FireProjectile();
            }
        }
    }

    private void PerformSuperAttack()
    {
        StopAllCoroutines();
        
        currentState = BossState.SuperAttack;
        stateTimer = 0f;
        idleTimer = 0f;
        
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);
        
        ResetAllTriggers();
        animator.SetTrigger("super");
        
        lastAttackTime = Time.time;
        StartCoroutine(ActionCooldown(1.2f));
    }
    
    private void PerformBuff()
    {
        StopAllCoroutines();
        
        currentState = BossState.Buffing;
        stateTimer = 0f;
        idleTimer = 0f;
        
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);
        
        ResetAllTriggers();
        animator.SetTrigger("buff");
        
        lastAttackTime = Time.time;
        StartCoroutine(ActionCooldown(1.0f));
    }

    private IEnumerator ActionCooldown(float duration)
    {
        isActionCooldown = true;
        yield return new WaitForSeconds(duration);
        isActionCooldown = false;
        
        if (currentState != BossState.Idle && currentState != BossState.Moving && 
            currentState != BossState.Turning)
        {
            Debug.Log("Estado preso após cooldown: " + currentState);
            ForceReturnToIdle();
        }
    }

    public void OnAttackFinished()
    {
        if (currentState == BossState.Dead)
            return;
        
        Debug.Log("OnAttackFinished chamado!");
        
        isAttacking = false;
        idleTimer = 0f;
        
        if (currentState == BossState.RangedAttack || 
            currentState == BossState.SuperAttack ||
            currentState == BossState.Buffing)
        {
            currentState = BossState.Idle;
            stateTimer = 0f;
            
            ResetAllTriggers();
            animator.SetBool("isMoving", false);
            
            StartCoroutine(ActionCooldown(0.3f));
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
        
        if (Random.value < 0.2f)
        {
            ForceReturnToIdle();
        }
    }

    private void Die()
    {
        currentState = BossState.Dead;
        stateTimer = 0f;
        
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);
        
        ResetAllTriggers();
        animator.SetTrigger("death");
        
        GetComponent<Collider2D>().enabled = false;
        
        // Desativar o projétil se estiver ativo
        if (firePoint != null && firePoint.isActive)
        {
            firePoint.Deactivate();
        }
    }
    
    public void FireProjectile()
    {
        if (firePoint == null)
        {
            Debug.LogWarning("FirePoint não configurado!");
            return;
        }
        
        Debug.Log("FireProjectile chamado - Frame:" + Time.frameCount + " - Estado:" + currentState);
        
        // Ativar o projétil
        firePoint.Activate(isFacingRight);
    }
    
    public void ApplyBuffEffect()
    {
        currentHealth += maxHealth / 5;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
    
    // Método para debug visual
    private void OnDrawGizmos()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.transform.position, 0.2f);
            
            Vector3 direction = isFacingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawRay(firePoint.transform.position, direction * 3f);
        }
    }
}