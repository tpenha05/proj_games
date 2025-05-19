using UnityEngine;

public class PlayerRespawnHandler : MonoBehaviour
{
    void Start()
    {
        if (RespawnData.I != null && RespawnData.I.checkpointPosition.HasValue)
        {
            transform.position = RespawnData.I.checkpointPosition.Value;
            RespawnData.I.checkpointPosition = null; // limpa para não usar de novo sem querer
        }
    }
}
