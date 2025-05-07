using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;

    public float minX;
    public float maxX;
    public float minY = -9999f;
    public float maxY = 9999f;

    void LateUpdate()
    {
        if (target == null) return;

        // Calcula a largura visível com base no zoom atual
        float camHalfHeight = Camera.main.orthographicSize;
        float camHalfWidth = camHalfHeight * Camera.main.aspect;

        // Deseja seguir o alvo no centro da câmera
        Vector3 desiredPosition = target.position;

        // Limita posição horizontal e vertical com base na largura visível da câmera
        float minLimitX = minX + camHalfWidth;
        float maxLimitX = maxX - camHalfWidth;
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minLimitX, maxLimitX);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);

        // Suaviza a movimentação da câmera até o ponto desejado
        Vector3 smoothed = Vector3.Lerp(
            transform.position,
            new Vector3(desiredPosition.x, desiredPosition.y, transform.position.z),
            smoothSpeed
        );

        transform.position = smoothed;
    }
}
