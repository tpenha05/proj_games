using UnityEngine;

public class CoinFollow : MonoBehaviour
{
    private Transform player;
    private bool isFollowing = false;
    private float speed = 10f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Invoke(nameof(StartFollowing), Random.Range(0.1f, 0.3f)); // pequeno delay para visual legal
    }

    void StartFollowing()
    {
        isFollowing = true;
    }

    void Update()
    {
        if (isFollowing && player != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth stats = other.GetComponent<PlayerHealth>();
            if (stats != null)
            {
                stats.AddCoins(1);
            }
            Destroy(gameObject);
        }
    }
}
