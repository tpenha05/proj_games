using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    private static int currentRunas = 0;
    private static PlayerScore instance;

    public TextMeshProUGUI runasText;

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

    private void OnEnable()
    {
        // Assina o callback para quando uma nova cena terminar de carregar
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Na primeira cena, já mostra
        UpdateRunasDisplay();
    }

    // Chamado sempre que uma cena é carregada
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Procura na cena o objeto com o TMP que você queira usar
        // Troque "RunasText" pelo nome ou tag do seu objeto na cena
        var go = GameObject.FindWithTag("RunasText");
        if (go != null)
        {
            runasText = go.GetComponent<TextMeshProUGUI>();
            UpdateRunasDisplay();
        }
    }

    public static void AddRunas(int amount)
    {
        currentRunas += amount;
        if (instance != null)
            instance.UpdateRunasDisplay();
    }

    public static int GetRunas() => currentRunas;

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