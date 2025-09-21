using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class ShowItemUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    public Toggle completionToggle;
    public TextMeshProUGUI contentText;
    public TextMeshProUGUI deadlineTime;
    public TextMeshProUGUI totalreward;

    private TodoItem todoItem;
    private Action<TodoItem> onCompleted;
    private Action<TodoItem, Transform> onMoved;
    private Action<TodoItem> onDelete;

    // Drag and drop variables
    private RectTransform parentRect;
    private int startSiblingIndex;
    private GameObject placeholder;
    // private bool isDragging = false;
    // private bool isOverDeleteArea = false;
    private RectTransform deleteAreaRect;

    public void Initialize(TodoItem item, Action<TodoItem, Transform> movedCallback,
        Action<TodoItem> deleteCallback = null, RectTransform deleteArea = null)
    {
        todoItem = item;
        onMoved = movedCallback;
        onDelete = deleteCallback;
        deleteAreaRect = deleteArea;

        completionToggle.isOn = item.isCompleted;
        completionToggle.transform.GetChild(0).gameObject.SetActive(!(item.isCompleted));
        contentText.text = item.keywords;
        // FIXME:应该重设UI与ShowItemUI区分开
        deadlineTime.text = ((item.isCompleted) ? item.completionTime : item.deadlineTime).ToString("yyyy/MM/dd");
        totalreward.text = item.rewardTotal.ToString();

        completionToggle.onValueChanged.AddListener(OnToggleChanged);

        // 添加拖拽排序功能
        transform.SetSiblingIndex(item.sortOrder);
    }

    private void OnToggleChanged(bool isCompleted)
    {
        todoItem.isCompleted = isCompleted;
        TodoListEventHandler.CallTodoItemCompleted(todoItem);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
