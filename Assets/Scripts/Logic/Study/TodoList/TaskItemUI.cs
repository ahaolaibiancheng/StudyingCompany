using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class TaskItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] public TMP_Text contentText;
    [SerializeField] public TMP_Text dueDateText;
    [SerializeField] public Toggle completionToggle;
    [SerializeField] public Image background;

    private TodoItem todoItem;
    private Action<TodoItem> onCompleted;
    private Action<TodoItem, Transform> onMoved;
    private Action<TodoItem> onDelete;

    // Drag and drop variables
    private RectTransform parentRect;
    private int startSiblingIndex;
    private GameObject placeholder;
    // private bool isDragging = false;
    private bool isOverDeleteArea = false;
    private RectTransform deleteAreaRect;

    public void Initialize(TodoItem item, Action<TodoItem> completedCallback, Action<TodoItem, Transform> movedCallback, Action<TodoItem> deleteCallback = null, RectTransform deleteArea = null)
    {
        todoItem = item;
        onCompleted = completedCallback;
        onMoved = movedCallback;
        onDelete = deleteCallback;
        deleteAreaRect = deleteArea;

        contentText.text = item.keywords;
        dueDateText.text = item.deadlineTime.ToString("yyyy-MM-dd");
        completionToggle.isOn = item.isCompleted;
        completionToggle.onValueChanged.AddListener(OnToggleChanged);

        UpdateAppearance();

        // 添加拖拽排序功能
        transform.SetSiblingIndex(item.sortOrder);
    }

    private void UpdateAppearance()
    {
        // 确保文本始终可见
        Color textColor = todoItem.isCompleted ?
            new Color(0.3f, 0.3f, 0.3f) :  // 完成时深灰色
            new Color(0.1f, 0.1f, 0.1f);   // 未完成时黑色

        contentText.color = textColor;
        dueDateText.color = textColor;

        background.color = todoItem.isCompleted ?
            new Color(0.9f, 0.9f, 0.9f) :  // 完成时浅灰背景
            new Color(1f, 1f, 1f);          // 未完成时白色背景       
    }

    private void OnToggleChanged(bool isCompleted)
    {
        todoItem.isCompleted = isCompleted;
        UpdateAppearance();
        onCompleted?.Invoke(todoItem);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //     parentRect = transform.parent.GetComponent<RectTransform>();
        //     startSiblingIndex = transform.GetSiblingIndex();

        //     // Create placeholder
        //     placeholder = new GameObject("Placeholder");
        //     placeholder.transform.SetParent(transform.parent);
        //     placeholder.transform.SetSiblingIndex(startSiblingIndex);
        //     // Add LayoutElement to placeholder to have same size as item
        //     LayoutElement le = placeholder.AddComponent<LayoutElement>();
        //     RectTransform rect = GetComponent<RectTransform>();
        //     le.preferredHeight = rect.rect.height;
        //     le.preferredWidth = rect.rect.width;

        //     background.color = new Color(0.9f, 0.9f, 1f);
        //     // Set as last sibling to draw on top
        //     transform.SetAsLastSibling();

        //     // Show delete area when dragging starts
        //     if (TodoListUI.Instance != null)
        //     {
        //         TodoListUI.Instance.ShowDeleteArea();
        //     }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //     transform.position = eventData.position;

        //     // Check if over delete area
        //     if (deleteAreaRect != null)
        //     {
        //         bool wasOverDeleteArea = isOverDeleteArea;
        //         isOverDeleteArea = RectTransformUtility.RectangleContainsScreenPoint(deleteAreaRect, eventData.position, null);

        //         if (isOverDeleteArea != wasOverDeleteArea)
        //         {
        //             // Update appearance to indicate delete state
        //             background.color = isOverDeleteArea ?
        //                 new Color(1f, 0.5f, 0.5f) : // Red tint for delete
        //                 new Color(0.9f, 0.9f, 1f);   // Normal drag color

        //             // If just entered delete area, update delete area text
        //             if (isOverDeleteArea && !wasOverDeleteArea && TodoListUI.Instance != null && TodoListUI.Instance.deleteAreaText != null)
        //             {
        //                 TodoListUI.Instance.deleteAreaText.text = "松开删除";
        //             }
        //             else if (!isOverDeleteArea && wasOverDeleteArea && TodoListUI.Instance != null && TodoListUI.Instance.deleteAreaText != null)
        //             {
        //                 TodoListUI.Instance.deleteAreaText.text = "拖拽至此删除";
        //             }
        //         }
        //     }

        //     // Update placeholder position for sorting
        //     if (placeholder != null && !isOverDeleteArea)
        //     {
        //         int newSiblingIndex = transform.parent.childCount;

        //         // Find the correct position for the placeholder
        //         for (int i = 0; i < transform.parent.childCount; i++)
        //         {
        //             if (transform.parent.GetChild(i) == placeholder.transform)
        //                 continue;

        //             if (transform.position.y > transform.parent.GetChild(i).position.y)
        //             {
        //                 newSiblingIndex = i;
        //                 if (newSiblingIndex > placeholder.transform.GetSiblingIndex())
        //                     newSiblingIndex--;
        //                 break;
        //             }
        //         }

        //         placeholder.transform.SetSiblingIndex(newSiblingIndex);
        //     }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //     // If the item was deleted during drag, we don't need to do anything here
        //     if (this == null) return;

        //     // Check if over delete area and delete if true
        //     if (isOverDeleteArea && onDelete != null)
        //     {
        //         onDelete?.Invoke(todoItem);
        //         // Clean up placeholder
        //         if (placeholder != null)
        //         {
        //             Destroy(placeholder);
        //             placeholder = null;
        //         }
        //         Destroy(gameObject);

        //         // Hide delete area after deletion
        //         if (TodoListUI.Instance != null)
        //         {
        //             TodoListUI.Instance.HideDeleteArea();
        //         }
        //         return; // Stop further processing
        //     }

        //     // Clean up placeholder
        //     if (placeholder != null)
        //     {
        //         Destroy(placeholder);
        //         placeholder = null;
        //     }

        //     // 更新排序顺序
        //     transform.SetSiblingIndex(startSiblingIndex);
        //     todoItem.sortOrder = transform.GetSiblingIndex();

        //     background.color = todoItem.isCompleted ?
        //         new Color(0.8f, 0.8f, 0.8f) :
        //         new Color(1f, 1f, 1f);

        //     // Reset delete area state
        //     isOverDeleteArea = false;

        //     // Hide delete area when dragging ends
        //     if (TodoListUI.Instance != null)
        //     {
        //         TodoListUI.Instance.HideDeleteArea();
        //     }
    }
}
