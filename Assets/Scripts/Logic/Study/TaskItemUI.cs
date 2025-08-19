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
    
    // Drag and drop variables
    private RectTransform parentRect;
    private int startSiblingIndex;
    private GameObject placeholder;
    private bool isDragging = false;

    public void Initialize(TodoItem item, Action<TodoItem> completedCallback, Action<TodoItem, Transform> movedCallback)
    {
        todoItem = item;
        onCompleted = completedCallback;
        onMoved = movedCallback;
        
        contentText.text = item.content;
        dueDateText.text = item.dueDate.ToString("yyyy-MM-dd");
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
            
        // 设置文本删除线效果
        contentText.fontStyle = todoItem.isCompleted ? 
            FontStyles.Strikethrough : FontStyles.Normal;
        dueDateText.fontStyle = todoItem.isCompleted ? 
            FontStyles.Strikethrough : FontStyles.Normal;   
    }

    private void OnToggleChanged(bool isCompleted)
    {
        todoItem.isCompleted = isCompleted;
        UpdateAppearance();
        onCompleted?.Invoke(todoItem);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentRect = transform.parent.GetComponent<RectTransform>();
        startSiblingIndex = transform.GetSiblingIndex();
        
        // Create placeholder
        placeholder = new GameObject("Placeholder");
        placeholder.transform.SetParent(transform.parent);
        placeholder.transform.SetSiblingIndex(startSiblingIndex);
        // Add LayoutElement to placeholder to have same size as item
        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        RectTransform rect = GetComponent<RectTransform>();
        le.preferredHeight = rect.rect.height;
        le.preferredWidth = rect.rect.width;
        
        background.color = new Color(0.9f, 0.9f, 1f);
        // Set as last sibling to draw on top
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        
        // Update placeholder position for sorting
        if (placeholder != null)
        {
            int newSiblingIndex = transform.parent.childCount;
            
            // Find the correct position for the placeholder
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                if (transform.parent.GetChild(i) == placeholder.transform)
                    continue;
                    
                if (transform.position.y > transform.parent.GetChild(i).position.y)
                {
                    newSiblingIndex = i;
                    if (newSiblingIndex > placeholder.transform.GetSiblingIndex())
                        newSiblingIndex--;
                    break;
                }
            }
            
            placeholder.transform.SetSiblingIndex(newSiblingIndex);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Clean up placeholder
        if (placeholder != null)
        {
            Destroy(placeholder);
            placeholder = null;
        }
        
        // 查找最近的分类容器
        Transform closestContainer = FindClosestContainer();
        if (closestContainer != null && closestContainer != transform.parent)
        {
            transform.SetParent(closestContainer);
            onMoved?.Invoke(todoItem, closestContainer);
        }
        else
        {
            // 更新排序顺序
            transform.SetSiblingIndex(placeholder != null ? placeholder.transform.GetSiblingIndex() : startSiblingIndex);
            todoItem.sortOrder = transform.GetSiblingIndex();
            onCompleted?.Invoke(todoItem);
        }
        
        background.color = todoItem.isCompleted ? 
            new Color(0.8f, 0.8f, 0.8f) : 
            new Color(1f, 1f, 1f);
    }

    private Transform FindClosestContainer()
    {
        Transform[] containers = {
            transform.parent, // 当前容器
            TodoListUI.Instance.importantUrgentContainer,
            TodoListUI.Instance.notImportantUrgentContainer,
            TodoListUI.Instance.importantNotUrgentContainer,
            TodoListUI.Instance.notImportantNotUrgentContainer
        };

        Transform closest = transform.parent;
        float minDistance = float.MaxValue;

        foreach (Transform container in containers)
        {
            float distance = Vector3.Distance(transform.position, container.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = container;
            }
        }

        return closest;
    }
}
