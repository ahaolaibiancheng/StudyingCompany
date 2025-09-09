using UnityEngine;
using UnityEngine.EventSystems;

public class MouseWheelBlocker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static bool IsBlocking = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsBlocking = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsBlocking = false;
    }
}
