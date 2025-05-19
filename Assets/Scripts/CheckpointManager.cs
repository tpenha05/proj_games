// CheckpointManager.cs
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager I;

    public CheckpointController spawnCheckpoint;
    public CheckpointController activeCheckpoint;

    private void Awake()
    {
        if (I == null)
            I = this;
        else
            Destroy(gameObject);
    }

    public void SetActive(CheckpointController checkpoint)
    {
        if (activeCheckpoint != null)
            activeCheckpoint.ForceOff();

        activeCheckpoint = checkpoint;
        activeCheckpoint.PlayActivateAnimation();
    }

    public void RegisterSpawn(CheckpointController checkpoint)
    {
        spawnCheckpoint = checkpoint;
    }
}

