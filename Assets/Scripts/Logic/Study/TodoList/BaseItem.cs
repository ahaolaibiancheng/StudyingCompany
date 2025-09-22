using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class BaseItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] protected TodoItem baseItem;
    [SerializeField] protected BaseItemType type;
    public Toggle completionToggle;
    public TextMeshProUGUI contentText;
    public TextMeshProUGUI deadlineTime;
    public TextMeshProUGUI totalreward;

    protected Action<TodoItem> onCompleted;
    protected Action<TodoItem, Transform> onMoved;
    protected Action<TodoItem> onDelete;

    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private int originalSiblingIndex;
    private Vector2 dragOffset;
    private bool isDragging = false;
    private BaseItem targetSwapItem = null;

    // 交换动画控制
    private bool isSwapping = false;
    private Coroutine swapCoroutine = null;

    protected void Awake()
    {
        completionToggle.onValueChanged.AddListener(OnToggleChanged);
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(TodoItem item)
    {
        baseItem = item;
        completionToggle.isOn = (type == BaseItemType.History) ? true : false;
        completionToggle.interactable = (type == BaseItemType.History) ? false : true;
        contentText.text = item.keywords;
        deadlineTime.text = ((type == BaseItemType.History) ? item.completionTime : item.deadlineTime).ToString("yyyy/MM/dd");
        deadlineTime.gameObject.SetActive(type != BaseItemType.Daily);
        totalreward.text = item.rewardTotal.ToString();

        transform.SetSiblingIndex(item.sortOrder);
    }

    private void OnToggleChanged(bool isCompleted)
    {
        // 处理Toggle状态变化
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TodoListController.Instance.detailPanel.SetupDetail(baseItem);
        TodoListController.Instance.detailPanel.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TodoListController.Instance.detailPanel.gameObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 记录原始位置和父对象
        originalParent = transform.parent;  // Content
        originalPosition = rectTransform.anchoredPosition;
        originalSiblingIndex = transform.GetSiblingIndex();

        // 讆算鼠标相对于项目中心的偏移
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition))
        {
            dragOffset = localPointerPosition;
        }

        // 保持在原始父级下，但置于顶层
        transform.SetAsLastSibling();

        // 启用拖动效果 - 保持完全可见
        canvasGroup.alpha = 1.0f;  // 确保完全不透明
        canvasGroup.blocksRaycasts = false;

        // 添加视觉反馈
        transform.localScale = Vector3.one * 1.05f;

        // 确保sortingOrder为最高
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            Canvas rootCanvas = canvas.rootCanvas;
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1000; // 设置为高优先级
        }

        isDragging = true;
        targetSwapItem = null;

        // 停止任何正在进行的交换动画
        if (swapCoroutine != null)
        {
            StopCoroutine(swapCoroutine);
            swapCoroutine = null;
        }
        isSwapping = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 更新位置
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            originalParent as RectTransform, eventData.position,
            eventData.pressEventCamera, out localPointerPosition))
        {
            rectTransform.anchoredPosition = localPointerPosition - dragOffset;
        }

        // 检查并处理项目交换
        CheckForItemSwap(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 恢复Canvas排序
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvas.overrideSorting = false;
            canvas.sortingOrder = 0;
        }

        // 恢复正常大小
        transform.localScale = Vector3.one;

        // 恢复拖动效果
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // 检查是否落在原容器内
        bool isDroppedInOriginalContainer = IsDroppedInOriginalContainer(eventData);

        if (isDroppedInOriginalContainer)
        {
            // 更新排序顺序
            UpdateSortOrder();
        }
        else
        {
            // 返回原位置
            ReturnToOriginalPosition();
        }

        isDragging = false;
        targetSwapItem = null;

        // 停止任何正在进行的交换动画
        if (swapCoroutine != null)
        {
            StopCoroutine(swapCoroutine);
            swapCoroutine = null;
        }
        isSwapping = false;
    }

    private bool IsDroppedInOriginalContainer(PointerEventData eventData)
    {
        // 检查所有悬停对象，寻找原容器
        foreach (var hoveredObject in eventData.hovered)
        {
            // 如果悬停对象是原容器或其子对象
            if (hoveredObject.transform == originalParent || hoveredObject.transform.IsChildOf(originalParent))
            {
                return true;
            }
        }
        return false;
    }

    private void CheckForItemSwap(PointerEventData eventData)
    {
        // 如果正在交换动画中，不处理新的交换
        if (isSwapping) return;

        if (originalParent == null) return;

        BaseItem newTargetSwapItem = null;
        Vector2 mousePosition = eventData.position;

        // 遍历同一容器中的所有项目
        for (int i = 0; i < originalParent.childCount; i++)
        {
            Transform child = originalParent.GetChild(i);
            if (child == transform) continue; // 跳过自己

            BaseItem otherItem = child.GetComponent<BaseItem>();
            if (otherItem != null)
            {
                RectTransform otherRect = otherItem.GetComponent<RectTransform>();
                if (otherRect != null)
                {
                    // 检查鼠标是否在该项目范围内
                    if (RectTransformUtility.RectangleContainsScreenPoint(otherRect, mousePosition, eventData.pressEventCamera))
                    {
                        newTargetSwapItem = otherItem;
                        break;
                    }
                }
            }
        }

        // 如果交换目标发生变化
        if (newTargetSwapItem != targetSwapItem)
        {
            // 停止之前的交换动画
            if (swapCoroutine != null)
            {
                StopCoroutine(swapCoroutine);
                swapCoroutine = null;
            }

            // 执行交换动画
            if (newTargetSwapItem != null)
            {
                swapCoroutine = StartCoroutine(SmoothSwapWithItem(newTargetSwapItem));
            }

            targetSwapItem = newTargetSwapItem;
        }
    }

    /// <summary>
    /// 快速交换：duration = 0.1f (100ms)
    /// 标准速度：duration = 0.2f (200ms)
    /// 慢速交换：duration = 0.3f (300ms)
    /// 超慢速：duration = 0.5f (500ms)

    /// 线性插值（匀速）
    /// t = elapsedTime / duration;
    /// 平滑插值（慢入慢出）
    /// t = Mathf.SmoothStep(0f, 1f, t);
    /// 弹性插值
    /// t = Mathf.SmoothDamp(0f, 1f, ref velocity, duration);
    /// </summary>
    /// <param name="targetItem"></param>
    /// <returns></returns>
    // private IEnumerator SmoothSwapWithItem(BaseItem targetItem)
    // {
    //     isSwapping = true;

    //     // 获取两个项目的RectTransform
    //     RectTransform targetRect = targetItem.rectTransform;

    //     // 记录原始位置
    //     Vector2 originalPos = rectTransform.anchoredPosition;
    //     Vector2 targetPos = targetRect.anchoredPosition;

    //     // 计算目标位置（交换后的位置）
    //     // 注意：由于是交换位置，所以目标是对方的原始位置
    //     Vector2 newThisPosition = targetPos;
    //     Vector2 newTargetPosition = originalPos;

    //     // 动画参数
    //     float duration = 0.2f; // 动画持续时间（可根据需要调整）
    //     float elapsedTime = 0f;

    //     // 执行平滑移动动画
    //     while (elapsedTime < duration)
    //     {
    //         float t = elapsedTime / duration;
    //         // 使用平滑插值
    //         t = Mathf.SmoothStep(0f, 1f, t);

    //         // 更新两个项目的位置
    //         rectTransform.anchoredPosition = Vector2.Lerp(originalPos, newThisPosition, t);
    //         targetRect.anchoredPosition = Vector2.Lerp(targetPos, newTargetPosition, t);

    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }

    //     // 确保最终位置准确
    //     rectTransform.anchoredPosition = newThisPosition;
    //     targetRect.anchoredPosition = newTargetPosition;

    //     // 交换在层级中的位置
    //     int currentIndex = transform.GetSiblingIndex();
    //     int targetIndex = targetItem.transform.GetSiblingIndex();
    //     transform.SetSiblingIndex(targetIndex);
    //     targetItem.transform.SetSiblingIndex(currentIndex);

    //     // 重置位置（因为SetSiblingIndex可能会影响位置）
    //     rectTransform.anchoredPosition = newThisPosition;
    //     targetRect.anchoredPosition = newTargetPosition;

    //     isSwapping = false;
    //     swapCoroutine = null;
    // }

    private void UpdateSortOrder()
    {
        // 更新当前项的排序顺序
        if (baseItem != null)
        {
            baseItem.sortOrder = transform.GetSiblingIndex();
        }

        // 更新同容器中所有项的排序顺序
        if (originalParent != null)
        {
            for (int i = 0; i < originalParent.childCount; i++)
            {
                BaseItem item = originalParent.GetChild(i).GetComponent<BaseItem>();
                if (item != null && item.baseItem != null)
                {
                    item.baseItem.sortOrder = i;
                }
            }
        }

        // 保存数据
        TodoListController.Instance.SaveTodoList();
    }

    private void ReturnToOriginalPosition()
    {
        transform.SetSiblingIndex(originalSiblingIndex);
    }

    private IEnumerator SmoothSwapWithItem(BaseItem targetItem)
    {
        isSwapping = true;
        RectTransform targetRect = targetItem.rectTransform;

        Vector2 originalPos = rectTransform.anchoredPosition;
        Vector2 targetPos = targetRect.anchoredPosition;
        Vector2 newThisPosition = targetPos;
        Vector2 newTargetPosition = originalPos;

        float duration = 0.25f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            // 添加弹性效果
            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            rectTransform.anchoredPosition = Vector2.Lerp(originalPos, newThisPosition, t);
            targetRect.anchoredPosition = Vector2.Lerp(targetPos, newTargetPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = newThisPosition;
        targetRect.anchoredPosition = newTargetPosition;

        // 交换层级位置
        int currentIndex = transform.GetSiblingIndex();
        int targetIndex = targetItem.transform.GetSiblingIndex();
        transform.SetSiblingIndex(targetIndex);
        targetItem.transform.SetSiblingIndex(currentIndex);

        isSwapping = false;
        swapCoroutine = null;
    }
}