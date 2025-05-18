using UnityEngine;

public class ProjectilePoint : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public float lifetime = 2f;
    private float timer = 0f;
    public bool isActive = false; // Alterado para público para acesso externo
    private Vector2 moveDirection;
    
    void Update()
    {
        if (!isActive)
            return;
            
        transform.Translate(moveDirection * speed * Time.deltaTime);
        
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Deactivate();
        }
    }
    
    public void Activate(bool facingRight)
    {
        timer = 0f;
        isActive = true;
        
        moveDirection = facingRight ? Vector2.right : Vector2.left;
        
        GetComponent<Collider2D>().enabled = true;
        
        Debug.Log("Projétil ativado, direção: " + (facingRight ? "direita" : "esquerda"));
    }
    
    public void Deactivate()
    {
        isActive = false;
        
        GetComponent<Collider2D>().enabled = false;
        
        transform.localPosition = Vector3.zero;
        
        Debug.Log("Projétil desativado");
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive)
            return;
            
        Debug.Log("Projétil colidiu com: " + other.name);
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("Projétil acertou o player!");
            // Aplicar dano ao player
            // other.GetComponent<PlayerHealth>().TakeDamage(damage);
            
            Deactivate();
        }
        // Verificar se colidiu com o cenário
        else if (!other.isTrigger && !other.CompareTag("Boss"))
        {
            Debug.Log("Projétil colidiu com cenário");
            Deactivate();
        }
    }
}