using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DocUI : MonoBehaviour
{
    public static DocUI Instance;

    [Header("UI Elements")]
    public GameObject promptPanel;
    public GameObject docPanel;
    public TextMeshProUGUI contentText;
    public TextMeshProUGUI contentText_2;
    public Image docImage;
    public TextMeshProUGUI closeHintText;
    public Button closeButton; 

    [Header("Document Content")]
    [TextArea(5, 10)] public string documentText;
    public Sprite documentSprite;

    private GameObject player;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");


        docPanel.SetActive(true);
        contentText.ForceMeshUpdate();
        contentText_2.ForceMeshUpdate();
        docPanel.SetActive(false);

        closeButton.onClick.AddListener(() => HideDocument());
    }

    void Update()
    {
        if (docPanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
        {
            HideDocument();
        }
    }

    public void ShowPrompt(bool show)
    {
        if (promptPanel != null)
        {
            promptPanel.SetActive(show);
        }
    }

    public void ShowDocument()
    {
        docPanel.SetActive(true);

    }

    void HideDocument()
    {
        docPanel.SetActive(false);
    }
}
