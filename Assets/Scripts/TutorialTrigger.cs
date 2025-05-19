using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Tutorial Settings")]
    [TextArea(3, 10)]
    public string tutorialText = "Use as setas para se mover.";
    public float displayTime = 4f;
    public bool showOnlyOnce = true;
    
    [Header("UI References")]
    public TextMeshProUGUI tutorialTextUI; // Referência para o TextMeshPro
    
    [Header("Animation")]
    public float fadeInTime = 0.5f;
    public float fadeOutTime = 0.5f;
    
    private bool hasBeenTriggered = false;
    private CanvasGroup canvasGroup;
    private Coroutine displayCoroutine;
    
    void Start()
    {
        // Verificar se temos a referência necessária
        if (tutorialTextUI == null)
        {
            Debug.LogError("Tutorial Text não atribuído no objeto: " + gameObject.name);
            return;
        }
        
        // Obter ou adicionar o componente CanvasGroup ao objeto pai do texto
        GameObject textContainer = tutorialTextUI.gameObject;
        canvasGroup = textContainer.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = textContainer.AddComponent<CanvasGroup>();
        
        // Esconder o texto inicialmente
        canvasGroup.alpha = 0;
        tutorialTextUI.enabled = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificar se é o player que colidiu
        if (other.CompareTag("Player"))
        {
            // Verificar se já foi acionado (se configurado para mostrar apenas uma vez)
            if (showOnlyOnce && hasBeenTriggered)
                return;
                
            // Mostrar o tutorial
            ShowTutorial();
            hasBeenTriggered = true;
        }
    }
    
    public void ShowTutorial()
    {
        // Cancelar qualquer exibição em andamento
        if (displayCoroutine != null)
            StopCoroutine(displayCoroutine);
            
        // Iniciar nova rotina de exibição
        displayCoroutine = StartCoroutine(DisplayTutorialRoutine());
    }
    
    public void HideTutorial()
    {
        // Cancelar qualquer exibição em andamento
        if (displayCoroutine != null)
            StopCoroutine(displayCoroutine);
            
        // Iniciar rotina de ocultação
        StartCoroutine(HideTutorialRoutine());
    }
    
    private IEnumerator DisplayTutorialRoutine()
    {
        // Configurar o texto
        tutorialTextUI.text = tutorialText;
        
        // Ativar o texto
        tutorialTextUI.enabled = true;
        
        // Fade in
        float elapsed = 0;
        while (elapsed < fadeInTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeInTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
        
        // Esperar o tempo de exibição
        yield return new WaitForSeconds(displayTime);
        
        // Fade out
        elapsed = 0;
        while (elapsed < fadeOutTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeOutTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
        tutorialTextUI.enabled = false;
        
        displayCoroutine = null;
    }
    
    private IEnumerator HideTutorialRoutine()
    {
        // Fade out
        float elapsed = 0;
        while (elapsed < fadeOutTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeOutTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
        tutorialTextUI.enabled = false;
    }

    public void ResetTrigger()
    {
        hasBeenTriggered = false;
    }
}