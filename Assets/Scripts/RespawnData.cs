using UnityEngine;

public class RespawnData : MonoBehaviour
{
    public static RespawnData I;

    public Vector3? checkpointPosition = null;

    private void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
