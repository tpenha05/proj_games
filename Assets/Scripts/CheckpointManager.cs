// CheckpointManager.cs
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager I;
    [HideInInspector] public CheckpointController activeCheckpoint;
    [HideInInspector] public CheckpointController spawnCheckpoint;

    void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    // Chamado pelo spawn no Start()
    public void RegisterSpawn(CheckpointController cp)
    {
        spawnCheckpoint = cp;
        // Deixe o spawn “aceso” por padrão
        cp.ForceOn();
    }

    // Ativa um novo checkpoint
    public void SetActive(CheckpointController cp)
    {
        if (activeCheckpoint != null && activeCheckpoint != cp)
        {
            activeCheckpoint.ForceOff();
            activeCheckpoint.isActiveCheckpoint = false;
        }

        activeCheckpoint = cp;
        cp.isActiveCheckpoint = true;
        cp.PlayActivateAnimation();
    }

}
