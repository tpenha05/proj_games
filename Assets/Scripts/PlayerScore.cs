using UnityEngine;
using TMPro; // Adicione esta linha para usar TextMeshProUGUI

public class PlayerScore : MonoBehaviour
{
    private static int currentRunas = 0;
    private static PlayerScore instance;
    
    public TextMeshProUGUI runasText; // Agora o Unity reconhecerá este tipo
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        UpdateRunasDisplay();
    }
    
    public static void AddRunas(int amount)
    {
        currentRunas += amount;
        if (instance != null)
            instance.UpdateRunasDisplay();
    }
    
    public static int GetRunas()
    {
        return currentRunas;
    }
    
    public static void ResetRunas()
    {
        currentRunas = 0;
        if (instance != null)
            instance.UpdateRunasDisplay();
    }
    
    private void UpdateRunasDisplay()
    {
        if (runasText != null)
            runasText.text = currentRunas.ToString();
    }
}