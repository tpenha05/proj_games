using UnityEngine;

public class SimpleFireflies : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public Transform player;
    public float followDistance = 30f;
    public float followSpeed = 1f;
    public bool followPlayer = true;
    
    [Header("Comportamento dos Vagalumes")]
    public float blinkSpeed = 1f;
    public float blinkIntensity = 0.8f;
    
    private ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;
    
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    
    void Update()
    {
        if (followPlayer && player != null)
        {
            transform.position = Vector3.Lerp(transform.position, player.position, followSpeed * Time.deltaTime);
        }
        
        int numParticlesAlive = particleSystem.GetParticles(particles);
        
        for (int i = 0; i < numParticlesAlive; i++)
        {
            float blink = Mathf.PingPong(Time.time * blinkSpeed + particles[i].randomSeed * 0.1f, blinkIntensity);
            
            Color color = particles[i].startColor;
            color.a = Mathf.Lerp(0.1f, 0.7f, blink);
            particles[i].startColor = color;
            
            if (Random.Range(0f, 1f) < 0.05f)
            {
                Vector3 randomDir = Random.insideUnitSphere * 0.1f;
                particles[i].velocity += randomDir;
            }
        }
        
        particleSystem.SetParticles(particles, numParticlesAlive);
    }
}