using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject hoverIndicator;
    
    private void Start()
    {
        // Verifica se o indicador foi configurado
        if (hoverIndicator == null)
        {
            Debug.LogWarning("Hover Indicator não configurado para o botão: " + gameObject.name);
        }
        else
        {
            // Desativa o indicador inicialmente
            hoverIndicator.SetActive(false);
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverIndicator != null)
        {
            // Simplesmente ativa o indicador
            hoverIndicator.SetActive(true);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverIndicator != null)
        {
            // Desativa o indicador
            hoverIndicator.SetActive(false);
        }
    }
}