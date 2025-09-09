using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToggleHoverHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image labelBackground;
    [SerializeField] private Color hoverColor = new Color(1f, 1f, 0.8f, 1f);
    private Color originalColor;

    private void Start()
    {
        if (labelBackground != null)
            originalColor = labelBackground.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (labelBackground != null)
            labelBackground.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (labelBackground != null)
            labelBackground.color = originalColor;
    }
}
