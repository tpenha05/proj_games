using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WalkingSpitter : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float stopDistance = 6f;

    private Rigidbody2D rb;
    private EnemyAnimatorController anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<EnemyAnimatorController>();
    }

    void FixedUpdate()
    {   
        Debug.Log("Scale: " + transform.localScale);
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > stopDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
            anim.SetWalking(true);
        }
        else
        {
            anim.SetWalking(false);
        }

        Vector3 scale = transform.localScale;
        scale.x = (player.position.x > transform.position.x) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}
