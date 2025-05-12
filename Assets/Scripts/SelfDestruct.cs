using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float delay = 1.0f; // ou o tempo da animação

    void Start()
    {
        Destroy(gameObject, delay);
    }
}
