using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TodoBaseItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected TodoItem todoBaseItem;
    [SerializeField] protected TodoBaseItemType type;
    public Button taskDoingBtn;
    public Button taskCompletedBtn;
    public Button deleteButton;
    public TMP_InputField contentText;
    // 以下暂未使用
    public TextMeshProUGUI deadlineTime;
    public TextMeshProUGUI totalreward;

    public TodoItem Data => todoBaseItem;

    protected virtual void Awake()
    {
        RegisterButtonListeners();
    }

    public void Initialize(TodoItem item)
    {
        todoBaseItem = item;
        contentText.text = item.keywords;
        // deadlineTime.text = ((type == TodoBaseItemType.History) ? item.completionTime : item.deadlineTime).ToString("yyyy/MM/dd");
        // deadlineTime.gameObject.SetActive(type != TodoBaseItemType.Daily);
        // totalreward.text = item.rewardTotal.ToString();

        RegisterButtonListeners();
        ApplyCompletionVisual(item.isCompleted);
        transform.SetSiblingIndex(item.sortOrder);
    }

    protected virtual void OnToggleChanged(bool isCompleted)
    {
        // Override in derived classes to handle completion state changes.
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //     TodoListController.Instance.detailPanel.SetupDetail(todoBaseItem);
        //     TodoListController.Instance.detailPanel.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // TodoListController.Instance.detailPanel.gameObject.SetActive(false);
    }

    private void RegisterButtonListeners()
    {
        taskDoingBtn?.onClick.AddListener(OnDoingButtonClicked);
        taskCompletedBtn?.onClick.AddListener(OnCompletedButtonClicked);
        deleteButton?.onClick.AddListener(OnDeleteButtonClicked);
    }

    private void OnDoingButtonClicked()
    {
        SetCompletionState(true);
    }

    private void OnCompletedButtonClicked()
    {
        SetCompletionState(false);
    }

    private void SetCompletionState(bool isCompleted)
    {
        if (todoBaseItem == null) return;

        if (todoBaseItem.isCompleted == isCompleted)
        {
            ApplyCompletionVisual(isCompleted);
            return;
        }

        todoBaseItem.isCompleted = isCompleted;
        ApplyCompletionVisual(isCompleted);
        OnToggleChanged(isCompleted);
    }

    private void OnDeleteButtonClicked()
    {
        DeleteCurrentItem();
    }

    private void DeleteCurrentItem()
    {
        if (todoBaseItem == null)
        {
            return;
        }

        TodoListController.Instance.RemoveTodoItem(todoBaseItem);

        OnItemDeleted();
    }

    private void ApplyCompletionVisual(bool isCompleted)
    {
        if (taskDoingBtn != null)
        {
            taskDoingBtn.gameObject.SetActive(!isCompleted);
        }

        if (taskCompletedBtn != null)
        {
            taskCompletedBtn.gameObject.SetActive(isCompleted);
        }

        if (contentText != null)
        {
            if (isCompleted)
            {
                contentText.textComponent.fontStyle |= FontStyles.Strikethrough;
            }
            else
            {
                contentText.textComponent.fontStyle &= ~FontStyles.Strikethrough;
            }
        }
    }

    protected virtual void OnItemDeleted()
    {
        Destroy(gameObject);
    }
}
