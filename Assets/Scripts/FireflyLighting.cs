using UnityEngine;

public class FireflyLighting : MonoBehaviour
{
    public Color lightColor = new Color(0.7f, 1f, 0.3f);
    public float minIntensity = 0.2f;
    public float maxIntensity = 0.8f;
    public float pulseSpeed = 2f;
    public float lightRadius = 1.5f;
    public int maxLights = 10;

    private ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;
    private Light[] lights;
    private int numLightsActive = 0;
    
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        int maxParticles = particleSystem.main.maxParticles;
        particles = new ParticleSystem.Particle[maxParticles];
        
        lights = new Light[Mathf.Min(maxLights, maxParticles)];
        
        for (int i = 0; i < lights.Length; i++)
        {
            GameObject lightObj = new GameObject("FireflyLight_" + i);
            lightObj.transform.parent = transform;
            
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = lightColor;
            light.intensity = 0;
            light.range = lightRadius;
            
            light.shadows = LightShadows.None;
            light.renderMode = LightRenderMode.Auto;
            
            lights[i] = light;
            lightObj.SetActive(false);
        }
    }
    
    void Update()
    {
        int numParticles = particleSystem.GetParticles(particles);
        numLightsActive = 0;
        
        for (int i = 0; i < numParticles && numLightsActive < lights.Length; i += numParticles / lights.Length)
        {
            Light light = lights[numLightsActive];
            light.gameObject.SetActive(true);
            light.transform.position = particles[i].position;
            
            float pulseValue = Mathf.PingPong(Time.time * pulseSpeed + particles[i].randomSeed % 5, 1f);
            light.intensity = Mathf.Lerp(minIntensity, maxIntensity, pulseValue);
            
            numLightsActive++;
        }
        
        for (int i = numLightsActive; i < lights.Length; i++)
        {
            lights[i].gameObject.SetActive(false);
        }
    }
}