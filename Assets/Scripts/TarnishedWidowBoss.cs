// TarnishedWidowBoss.cs
// Controlador de comportamento do chefe 'Tarnished Widow'
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class TarnishedWidowBoss : MonoBehaviour
{
    /* ─────────── INSPECTOR ─────────── */
    [Header("Referências")]
    // Referência ao transform do jogador
    public Transform player;
    // Ponto auxiliar para verificar se o chefe está no chão
    public Transform groundCheck;
    // Componente responsável pelo ataque com língua
    public TongueHitbox tongue;
    // Componente responsável pelo ataque corpo-a-corpo
    public MeleeHitbox meleeHitbox;
    // Prefab da sombra utilizada durante ataque de salto
    public GameObject shadowPrefab;
    // Camada considerada como solo (para detecção de colisão)
    public LayerMask groundLayer;

    [Header("Movimento")]
    [Tooltip("Velocidade de perseguição até entrar no alcance de combo")]
    public float walkSpeed = 2.5f;
    [Tooltip("Distância máxima para iniciar combos")]
    public float comboRange = 6f;

    [Header("Combos")]
    [Tooltip("Tempo entre cada ataque no combo")]
    public float attackInterval = 1f;
    [Tooltip("Tempo em Idle após completar um combo")]
    public float idlePause = 2f;
    // Sequências de triggers de animação que compõem cada combo
    private readonly string[][] combos = new string[][] {
        new string[]{ "AttackMelee", "AttackFar" },
        new string[]{ "AttackMelee", "AttackMelee", "AttackFar" },
        new string[]{ "AttackMelee" },
        new string[]{ "AttackMelee", "AttackMelee" },
        new string[]{ "AttackFar" },
        new string[]{ "AttackFar", "AttackMelee" },
        new string[]{ "AttackFar", "AttackFar", "AttackMelee" }
    };

    [Header("Jump-Attack")]
    // Tempo até reaparecer após sumir
    public float vanishTime = 0.6f;
    // Altura do salto antes de cair sobre o jogador
    public float dropHeight = 6f;
    // Raio de impacto do smash ao aterrissar
    public float smashRadius = 2.4f;

    [Header("Cooldowns")]
    // Intervalo mínimo entre ataques de salto
    public float jumpCooldown = 20f;
    // Intervalo mínimo entre ataques à distância
    public float farAttackCooldown = 5f;

    [Header("Vida")]
    // Vida máxima do chefe
    public int maxHealth = 10;

    [Header("Condição de Vitória")]
    [Tooltip("Referência ao controlador de vitória na cena")]
    public WinConditionController winController;

    // Métodos invocados por eventos de animação para habilitar/desabilitar colisores
    public void EnableHitbox()  { if (tongue != null) tongue.EnableHitbox(); }
    public void DisableHitbox() { if (tongue != null) tongue.DisableHitbox(); }
    public void EnableMelee()   { if (meleeHitbox != null) meleeHitbox.EnableMelee(); }
    public void DisableMelee()  { if (meleeHitbox != null) meleeHitbox.DisableMelee(); }

    /* ─────────── PRIVADOS ─────────── */
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private Collider2D[] cols;

    // Estado de direção (true = olhando para a direita)
    private bool facingRight;
    private int currentHealth;
    private bool isDead;

    // Controle de execução de combos
    private bool isPerformingCombo;
    private int comboCounter;
    // Timestamp dos últimos ataques especiais
    private float lastJumpTime = -Mathf.Infinity;
    private float lastFarAttackTime = -Mathf.Infinity;

    // Inicialização de referências e variáveis
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        cols = GetComponents<Collider2D>();
        currentHealth = maxHealth;
        // Detecta escala inicial para definir direção
        facingRight = transform.localScale.x > 0;

        // Busca automatica do controlador de vitória se não estiver atribuído
        if (winController == null)
        {
            winController = FindObjectOfType<WinConditionController>();
            if (winController == null)
                Debug.LogWarning("WinConditionController não encontrado na cena!");
        }
    }

    // Atualizado a cada frame para lidar com animações e iniciar combos
    void Update()
    {
        if (isDead) return;

        // Atualiza parâmetros de animação
        anim.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool("isGrounded", IsGrounded());

        // Se não estiver executando um combo, inicia a rotina
        if (!isPerformingCombo)
            StartCoroutine(PerformComboRoutine());
    }

    // Rotina de execução de um combo completo
    IEnumerator PerformComboRoutine()
    {
        if (isDead) yield break;
        isPerformingCombo = true;

        // Persegue jogador até ficar dentro do alcance de combo
        while (Vector2.Distance(transform.position, player.position) > comboRange)
        {
            if (isDead) yield break;
            WalkTowardsPlayer();
            yield return null;
        }
        // Para o movimento antes de atacar
        rb.linearVelocity = Vector2.zero;

        // Seleciona e executa o combo atual
        string[] combo = combos[comboCounter % combos.Length];
        foreach (var trigger in combo)
        {
            if (isDead) yield break;
            // Respeita cooldown do ataque à distância
            if (trigger == "AttackFar" && Time.time - lastFarAttackTime < farAttackCooldown)
                continue;
            if (trigger == "AttackFar")
                lastFarAttackTime = Time.time;

            anim.SetTrigger(trigger);
            yield return new WaitForSeconds(attackInterval);
        }
        comboCounter++;

        if (isDead) yield break;
        // Pausa após o combo
        yield return new WaitForSeconds(idlePause);

        // A cada 3 ou 4 combos, realiza um ataque de salto
        if (!isDead && (comboCounter % 3 == 0 || comboCounter % 4 == 0))
        {
            StartJumpAttack();
            // Aguarda até aterrissar
            yield return new WaitUntil(() => IsGrounded() || isDead);
            if (isDead) yield break;
            yield return new WaitForSeconds(idlePause);
        }

        isPerformingCombo = false;
    }

    // Muda velocidade para perseguir jogador
    void WalkTowardsPlayer()
    {
        FacePlayer();
        rb.linearVelocity = new Vector2((facingRight ? 1 : -1) * walkSpeed, rb.linearVelocity.y);
    }

    // Inicia ataque de salto se condições forem atendidas
    public void StartJumpAttack()
    {
        if (isDead || !IsGrounded() || Time.time - lastJumpTime < jumpCooldown)
            return;

        lastJumpTime = Time.time;
        StartCoroutine(JumpAttackRoutine());
    }

    // Sequência de ataque de salto: some, reaparece acima do jogador e cai
    IEnumerator JumpAttackRoutine()
    {
        anim.SetTrigger("JumpAttack");
        yield return new WaitForSeconds(attackInterval);

        Vector3 target = player.position;
        // Desabilita colisores e sprite para sumir
        ToggleColliders(false);
        sr.enabled = false;
        if (shadowPrefab)
            Instantiate(shadowPrefab, new Vector3(target.x, target.y, transform.position.z), Quaternion.identity);

        yield return new WaitForSeconds(vanishTime);

        // Reposiciona acima do jogador e reaparece
        transform.position = new Vector3(target.x, target.y + dropHeight, transform.position.z);
        FacePlayer();
        ToggleColliders(true);
        sr.enabled = true;

        anim.SetTrigger("DoAttackFall");
        // Aguarda aterrissagem
        yield return new WaitUntil(() => IsGrounded() || isDead);
    }

    // Efeito de smash no chão, causa dano ao jogador se estiver dentro do raio
    public void DoGroundSmash()
    {
        if (isDead) return;
        Collider2D hit = Physics2D.OverlapCircle(transform.position, smashRadius, LayerMask.GetMask("Player"));
        hit?.GetComponent<PlayerHealth>()?.TakeDamage();
    }

    // Recebe dano e verifica morte
    public void TakeDamage(int dmg)
    {
        if (isDead) return;
        currentHealth -= dmg;
        anim.SetTrigger("Hit");
        if (currentHealth <= 0)
            Die();
    }

    // Sequência de morte do chefe
    void Die()
    {
        isDead = true;

        StopAllCoroutines();
        anim.ResetTrigger("Hit");
        anim.ResetTrigger("AttackMelee");
        anim.ResetTrigger("AttackFar");
        anim.ResetTrigger("JumpAttack");
        anim.ResetTrigger("DoAttackFall");

        anim.Play("Death", 0, 0f);

        ToggleColliders(false);
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        enabled = false;

        Debug.Log("Boss morreu de vez!");

        if (winController != null)
            winController.isBossKilled = true;
    }

    bool IsGrounded()
        => Physics2D.OverlapCircle(groundCheck.position, 0.05f, groundLayer);

    void ToggleColliders(bool state)
    {
        foreach (var c in cols)
            c.enabled = state;
    }

    void FacePlayer()
    {
        bool toRight = player.position.x > transform.position.x;
        if (toRight != facingRight)
            Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }

    void OnDrawGizmosSelected()
    {
        if (meleeHitbox != null)
            Gizmos.DrawWireSphere(meleeHitbox.transform.position, meleeHitbox.knockbackForce);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, smashRadius);
    }
}
