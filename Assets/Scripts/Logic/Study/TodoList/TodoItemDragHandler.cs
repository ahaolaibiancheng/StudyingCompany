using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TodoBaseItem))]
public class TodoItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private TodoBaseItem baseItem;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Transform originalParent;
    private int originalSiblingIndex;
    private Vector2 dragOffset;
    private TodoBaseItem targetSwapItem;
    private Vector2? targetAnchoredPosition;
    private Vector2 originalAnchoredPosition;

    [SerializeField] private float dragScaleMultiplier = 1.05f;

    private bool isSwapping;
    private Coroutine swapCoroutine;
    private Canvas cachedCanvas;
    private bool originalOverrideSorting;
    private int originalSortingOrder;
    private Vector3 originalScale;

    private void Awake()
    {
        baseItem = GetComponent<TodoBaseItem>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
        originalAnchoredPosition = rectTransform.anchoredPosition;
        targetAnchoredPosition = null;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out var localPointerPosition))
        {
            dragOffset = localPointerPosition;
        }

        transform.SetAsLastSibling();

        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = false;

        originalScale = transform.localScale;
        float scaleMultiplier = Mathf.Max(0.01f, dragScaleMultiplier);
        transform.localScale = originalScale * scaleMultiplier;

        cachedCanvas = GetComponentInParent<Canvas>();
        if (cachedCanvas != null)
        {
            originalOverrideSorting = cachedCanvas.overrideSorting;
            originalSortingOrder = cachedCanvas.sortingOrder;

            cachedCanvas.overrideSorting = true;
            cachedCanvas.sortingOrder = 1000;
        }

        targetSwapItem = null;
        targetAnchoredPosition = null;
        targetAnchoredPosition = null;

        if (swapCoroutine != null)
        {
            StopCoroutine(swapCoroutine);
            swapCoroutine = null;
        }
        isSwapping = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (originalParent == null)
        {
            return;
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                originalParent as RectTransform, eventData.position, eventData.pressEventCamera, out var localPointerPosition))
        {
            Vector2 nextPosition = localPointerPosition - dragOffset;
            rectTransform.anchoredPosition = new Vector2(originalAnchoredPosition.x, nextPosition.y);
        }

        CheckForItemSwap(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (cachedCanvas != null)
        {
            cachedCanvas.overrideSorting = originalOverrideSorting;
            cachedCanvas.sortingOrder = originalSortingOrder;
        }
        cachedCanvas = null;

        transform.localScale = originalScale;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        bool isDroppedInOriginalContainer = IsDroppedInOriginalContainer(eventData);

        if (isDroppedInOriginalContainer)
        {
            UpdateSortOrder();
        }
        else
        {
            ReturnToOriginalPosition();
        }

        targetSwapItem = null;

        if (swapCoroutine != null)
        {
            StopCoroutine(swapCoroutine);
            swapCoroutine = null;
        }
        isSwapping = false;
    }

    private bool IsDroppedInOriginalContainer(PointerEventData eventData)
    {
        if (originalParent == null)
        {
            return false;
        }

        foreach (var hoveredObject in eventData.hovered)
        {
            if (hoveredObject.transform == originalParent || hoveredObject.transform.IsChildOf(originalParent))
            {
                return true;
            }
        }
        return false;
    }

    private void CheckForItemSwap(PointerEventData eventData)
    {
        if (isSwapping || originalParent == null)
        {
            return;
        }

        TodoBaseItem newTargetSwapItem = null;
        Vector2 mousePosition = eventData.position;

        for (int i = 0; i < originalParent.childCount; i++)
        {
            Transform child = originalParent.GetChild(i);
            if (child == transform)
            {
                continue;
            }

            TodoBaseItem otherItem = child.GetComponent<TodoBaseItem>();
            if (otherItem == null)
            {
                continue;
            }

            RectTransform otherRect = otherItem.GetComponent<RectTransform>();
            if (otherRect != null && RectTransformUtility.RectangleContainsScreenPoint(otherRect, mousePosition, eventData.pressEventCamera))
            {
                newTargetSwapItem = otherItem;
                break;
            }
        }

        if (newTargetSwapItem == targetSwapItem)
        {
            return;
        }

        if (swapCoroutine != null)
        {
            StopCoroutine(swapCoroutine);
            swapCoroutine = null;
        }

        if (newTargetSwapItem != null)
        {
            swapCoroutine = StartCoroutine(SmoothSwapWithItem(newTargetSwapItem));
        }

        targetSwapItem = newTargetSwapItem;
        targetAnchoredPosition = newTargetSwapItem != null
            ? newTargetSwapItem.GetComponent<RectTransform>()?.anchoredPosition
            : (Vector2?)null;
    }

    private void UpdateSortOrder()
    {
        if (baseItem.Data != null)
        {
            baseItem.Data.sortOrder = transform.GetSiblingIndex();
        }

        if (originalParent != null)
        {
            for (int i = 0; i < originalParent.childCount; i++)
            {
                TodoBaseItem item = originalParent.GetChild(i).GetComponent<TodoBaseItem>();
                if (item?.Data != null)
                {
                    item.Data.sortOrder = i;
                }
            }
        }

        TodoListController.Instance.SaveTodoList();
    }

    private void ReturnToOriginalPosition()
    {
        transform.SetSiblingIndex(originalSiblingIndex);
        rectTransform.anchoredPosition = originalAnchoredPosition;
        targetAnchoredPosition = null;
    }

    private IEnumerator SmoothSwapWithItem(TodoBaseItem targetItem)
    {
        isSwapping = true;

        RectTransform targetRect = targetItem.GetComponent<RectTransform>();
        if (targetRect == null)
        {
            isSwapping = false;
            yield break;
        }

        Vector2 thisStart = rectTransform.anchoredPosition;
        Vector2 thatStart = targetRect.anchoredPosition;

        float thisEndY = targetAnchoredPosition.HasValue ? targetAnchoredPosition.Value.y : thatStart.y;
        float thatEndY = thisStart.y;

        Vector2 thisEnd = new Vector2(thisStart.x, thisEndY);
        Vector2 thatEnd = new Vector2(thatStart.x, thatEndY);

        float duration = 0.25f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            float newThisY = Mathf.Lerp(thisStart.y, thisEnd.y, t);
            float newThatY = Mathf.Lerp(thatStart.y, thatEnd.y, t);

            rectTransform.anchoredPosition = new Vector2(thisStart.x, newThisY);
            targetRect.anchoredPosition = new Vector2(thatStart.x, newThatY);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = thisEnd;
        targetRect.anchoredPosition = thatEnd;

        int currentIndex = transform.GetSiblingIndex();
        int targetIndex = targetItem.transform.GetSiblingIndex();
        transform.SetSiblingIndex(targetIndex);
        targetItem.transform.SetSiblingIndex(currentIndex);

        isSwapping = false;
        targetAnchoredPosition = null;
        swapCoroutine = null;
    }
}
