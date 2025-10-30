using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DailyPanelUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    [Header("UI Container")]
    public Transform container;
    [Header("Category Buttons")]
    public Button importantUrgentButton;
    public Button notImportantUrgentButton;
    public Button importantNotUrgentButton;
    public Button notImportantNotUrgentButton;
    [Header("Actions")]
    public Button addButton;
    [Header("Daily Item Prefab")]
    public GameObject todoListItemPrefab;

    private TodoListController todoController;
    private readonly Dictionary<Button, UnityAction> buttonListeners = new Dictionary<Button, UnityAction>();

    private enum TodoCategory
    {
        ImportantUrgent,
        NotImportantUrgent,
        ImportantNotUrgent,
        NotImportantNotUrgent
    }

    private TodoCategory currentCategory = TodoCategory.ImportantUrgent;

    private void Awake()
    {
        todoController = GetComponentInParent<TodoListController>();
        RegisterCategoryButton(importantUrgentButton, TodoCategory.ImportantUrgent);
        RegisterCategoryButton(notImportantUrgentButton, TodoCategory.NotImportantUrgent);
        RegisterCategoryButton(importantNotUrgentButton, TodoCategory.ImportantNotUrgent);
        RegisterCategoryButton(notImportantNotUrgentButton, TodoCategory.NotImportantNotUrgent);
        if (addButton != null)
        {
            addButton.onClick.AddListener(OnAddButtonClicked);
        }
    }

    void OnEnable()
    {
        TodoListEventHandler.DailyItemCompleted += OnTaskCompleted;
        RefreshCurrentCategory();
    }

    void OnDisable()
    {
        TodoListEventHandler.DailyItemCompleted -= OnTaskCompleted;
    }

    private void OnDestroy()
    {
        foreach (KeyValuePair<Button, UnityAction> entry in buttonListeners)
        {
            if (entry.Key != null)
            {
                entry.Key.onClick.RemoveListener(entry.Value);
            }
        }
        buttonListeners.Clear();
        if (addButton != null)
        {
            addButton.onClick.RemoveListener(OnAddButtonClicked);
        }
    }

    private void Start()
    {
        RefreshCurrentCategory();
    }

    private void RefreshCurrentCategory()
    {
        ShowCategory(currentCategory);
    }

    private void ShowCategory(TodoCategory category)
    {
        currentCategory = category;
        RefreshTitleText(category);
        RefreshContainer(GetCategoryItems(category));
    }

    private void RefreshTitleText(TodoCategory category)
    {
        switch (category)
        {
            case TodoCategory.ImportantUrgent:
                titleText.text = "今日待办（紧急重要）";
                break;
            case TodoCategory.NotImportantUrgent:
                titleText.text = "今日待办（非紧急重要）";
                break;
            case TodoCategory.ImportantNotUrgent:
                titleText.text = "今日待办（紧急非重要）";
                break;
            case TodoCategory.NotImportantNotUrgent:
                titleText.text = "清单（非紧急非重要）";
                break;
            default:
                titleText.text = "今日待办（紧急重要）";
                break;
        }
    }


    private List<TodoItem> GetCategoryItems(TodoCategory category)
    {
        if (todoController == null || todoController.todoListDataSO == null)
        {
            return null;
        }

        switch (category)
        {
            case TodoCategory.ImportantUrgent:
                return todoController.todoListDataSO.importantUrgent;
            case TodoCategory.NotImportantUrgent:
                return todoController.todoListDataSO.notImportantUrgent;
            case TodoCategory.ImportantNotUrgent:
                return todoController.todoListDataSO.importantNotUrgent;
            case TodoCategory.NotImportantNotUrgent:
                return todoController.todoListDataSO.notImportantNotUrgent;
            default:
                return null;
        }
    }

    private void RefreshContainer(List<TodoItem> items)
    {
        if (container == null)
        {
            return;
        }

        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        if (items == null)
        {
            return;
        }

        foreach (TodoItem item in items)
        {
            if (item.deadlineTime.Date <= DateTime.Now.Date)
            {
                AddTaskToUI(item, container);
            }
        }
    }

    private void AddTaskToUI(TodoItem item, Transform parent)
    {
        GameObject DailyItem = Instantiate(todoListItemPrefab, parent);
        DailyItemUI itemUI = DailyItem.GetComponent<DailyItemUI>();
        itemUI.Initialize(item);
    }

    private void OnTaskCompleted(TodoItem item)
    {
        todoController.CompleteDailyItem(item);
        RefreshCurrentCategory();
    }

    private void OnTaskMoved(TodoItem item, Transform newContainer)
    {

    }

    private void OnTaskDeleted(TodoItem item)
    {
    }

    private void OnCategoryButtonClicked(TodoCategory category)
    {
        ShowCategory(category);
    }

    private void OnAddButtonClicked()
    {
        if (todoController == null || todoController.todoListDataSO == null)
        {
            return;
        }

        TodoItem newItem = CreateDefaultTodoItem(currentCategory);
        todoController.AddTodoItem(newItem);
        RefreshCurrentCategory();
    }

    private void RegisterCategoryButton(Button button, TodoCategory category)
    {
        if (button == null)
        {
            return;
        }

        UnityAction clickAction = () => OnCategoryButtonClicked(category);

        if (buttonListeners.ContainsKey(button))
        {
            button.onClick.RemoveListener(buttonListeners[button]);
            buttonListeners[button] = clickAction;
        }
        else
        {
            buttonListeners.Add(button, clickAction);
        }

        button.onClick.AddListener(clickAction);
    }

    private TodoItem CreateDefaultTodoItem(TodoCategory category)
    {
        List<TodoItem> currentList = GetCategoryItems(category);
        int nextSortOrder = currentList != null ? currentList.Count : 0;

        bool isImportant = category == TodoCategory.ImportantUrgent || category == TodoCategory.ImportantNotUrgent;
        bool isUrgent = category == TodoCategory.ImportantUrgent || category == TodoCategory.NotImportantUrgent;

        TodoItem item = new TodoItem
        {
            guid = Guid.NewGuid().ToString(),
            keywords = "新日常任务",
            isImportant = isImportant,
            isUrgent = isUrgent,
            isCompleted = false,
            type = "工作/学业",
            rewardDaily = 1,
            rewardTotal = 0,
            completedTimes = 0,
            period = new bool[7] { true, true, true, true, true, true, true },
            description = string.Empty,
            sortOrder = nextSortOrder,
            creationTime = DateTime.Now,
            completionTime = DateTime.MaxValue
        };

        item.deadlineTime = DateTime.Now;
        return item;
    }
}
