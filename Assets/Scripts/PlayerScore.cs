using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public static int runas = 0;

    public static void AddRunas(int amount)
    {
        runas += amount;
    }

    public static void ResetRunas()
    {
        runas = 0;
    }
}
