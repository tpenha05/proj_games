using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WalkingSpitter : MonoBehaviour
{
    [Header("Movimento")]
    public Transform player;
    public float moveSpeed = 3f;
    public float stopDistance = 6f;
    
    [Header("Detecção de Terreno")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.3f;
    public float edgeCheckDistance = 0.5f;
    public bool avoidFalling = true; // Define se o inimigo evita cair de plataformas
    
    [Header("Limites do Mundo")]
    public float fallThreshold = -20f; // Limite vertical para destruição/reset
    public bool destroyOnFall = false; // Se falso, reposiciona na posição inicial
    
    private Rigidbody2D rb;
    private EnemyAnimatorController anim;
    private Vector2 initialPosition;
    private Vector2 targetVelocity;
    private bool isFacingRight;
    private bool isGrounded;
    
    // Propriedade apenas para leitura
    public bool IsGrounded => isGrounded;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<EnemyAnimatorController>();
        initialPosition = transform.position;
        
        // Garantir que a configuração física está correta
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 2f; // Ajuste conforme necessário
        
        // Inicializar estado
        isFacingRight = true;
    }
    
    void Update()
    {
        // Verificar se caiu para fora do mundo
        if (transform.position.y < fallThreshold)
        {
            HandleFallOutOfBounds();
        }
        
        // Verificar se está no chão
        CheckGrounded();
    }
    
    void FixedUpdate()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float directionToPlayer = player.position.x - transform.position.x;
        
        // Definir para onde o sprite deve olhar
        UpdateFacingDirection(directionToPlayer);
        
        // Verificar se deve se mover
        bool shouldMove = distanceToPlayer > stopDistance;
        
        // Se deve evitar cair de plataformas, verificar se há chão à frente
        if (shouldMove && avoidFalling && isGrounded)
        {
            // Verificar se tem chão à frente na direção do movimento
            Vector2 edgeCheckPosition = new Vector2(
                transform.position.x + (isFacingRight ? edgeCheckDistance : -edgeCheckDistance), 
                transform.position.y
            );
            
            bool hasGroundAhead = Physics2D.Raycast(
                edgeCheckPosition,
                Vector2.down,
                groundCheckDistance,
                groundLayer
            );
            
            // Se não houver chão à frente, parar
            if (!hasGroundAhead)
            {
                shouldMove = false;
            }
            
            // Para debug
            Debug.DrawRay(
                edgeCheckPosition,
                Vector2.down * groundCheckDistance,
                hasGroundAhead ? Color.green : Color.red
            );
        }
        
        // Aplicar movimento apenas se estiver no chão e deve se mover
        if (isGrounded && shouldMove)
        {
            // Move apenas no eixo X
            Vector2 direction = new Vector2(Mathf.Sign(directionToPlayer), 0);
            targetVelocity = direction * moveSpeed;
            
            // Atualizar o animator
            anim.SetWalking(true);
        }
        else
        {
            // Parar horizontalmente
            targetVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetWalking(false);
        }
        
        // Aplicar velocidade (permite que a física vertical continue funcionando)
        rb.linearVelocity = new Vector2(targetVelocity.x, rb.linearVelocity.y);
    }
    
    private void CheckGrounded()
    {
        isGrounded = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
        
        Debug.DrawRay(
            transform.position,
            Vector2.down * groundCheckDistance,
            isGrounded ? Color.green : Color.red
        );
    }
    
    private void UpdateFacingDirection(float directionToPlayer)
    {
        if ((directionToPlayer > 0 && !isFacingRight) || (directionToPlayer < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
    
    private void HandleFallOutOfBounds()
    {
        if (destroyOnFall)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position = initialPosition;
            rb.linearVelocity = Vector2.zero;
            
            if (anim != null)
            {
                anim.SetWalking(false);
            }
        }
    }

}