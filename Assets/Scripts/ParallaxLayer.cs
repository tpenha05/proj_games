using UnityEngine;

[RequireComponent(typeof(Transform))]
public class ParallaxLayer : MonoBehaviour
{
    [Range(0f,1f)]
    public float parallaxFactor = 0.5f;

    public float stopParallaxY = 5f;
    public Transform player;

    private Transform cam;
    private Vector3 layerStartPos;
    private Vector3 camStartPos;
    private float lastAllowedCamX;

    void Start()
    {
        cam = Camera.main.transform;
        layerStartPos = transform.position;
        camStartPos   = cam.position;
        lastAllowedCamX = 0f;

        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;
    }

    void LateUpdate()
    {
        // deslocamento total da câmera desde o início, só X
        float totalCamDeltaX = cam.position.x - camStartPos.x;

        // se ultrapassar a altura, congela no último X permitido
        if (player != null && player.position.y > stopParallaxY)
            totalCamDeltaX = lastAllowedCamX;
        else
            lastAllowedCamX = totalCamDeltaX;

        // posiciona só no X, mantém Y/Z originais
        transform.position = new Vector3(
            layerStartPos.x + totalCamDeltaX * parallaxFactor,
            layerStartPos.y,
            layerStartPos.z
        );
    }
}
