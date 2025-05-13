// CheckpointManager.cs
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager I;
    public CheckpointController activeCheckpoint;

    void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    public void SetActive(CheckpointController cp)
    {
        if (activeCheckpoint != null && activeCheckpoint != cp)
            activeCheckpoint.Deactivate();
        activeCheckpoint = cp;
        cp.Activate(); 
    }
}
