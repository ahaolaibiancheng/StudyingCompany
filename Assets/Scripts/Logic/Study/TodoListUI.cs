using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TodoListUI : MonoBehaviour
{
    public static TodoListUI Instance;
    
    [Header("Category Containers")]
    public Transform importantUrgentContainer;
    public Transform notImportantUrgentContainer;
    public Transform importantNotUrgentContainer;
    public Transform notImportantNotUrgentContainer;

    [Header("Add Task UI")]
    public GameObject addTaskPanel;
    public TMP_InputField taskContentInput;
    public Toggle importantToggle;
    public Toggle urgentToggle;
    public TMP_InputField dueDateInput;
    public Button submitButton;
    public Button cancelButton;

    [Header("Task Item Prefab")]
    public GameObject taskItemPrefab;

    [Header("Category Buttons")]
    public Button importantUrgentButton;
    public Button notImportantUrgentButton;
    public Button importantNotUrgentButton;
    public Button notImportantNotUrgentButton;
    
    [Header("Active Category Text")]
    public Text activeCategoryText;
    
    [Header("Add Task Button")]
    public Button addTaskButton;
    public Button closeButton;

    [Header("Delete Area")]
    public RectTransform deleteArea;
    public Image deleteAreaImage;
    public Text deleteAreaText;

    private TodoListController todoController;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        todoController = GetComponent<TodoListController>();
        if (todoController == null)
            todoController = gameObject.AddComponent<TodoListController>();
        
        submitButton.onClick.AddListener(AddNewTask);
        cancelButton.onClick.AddListener(CancelNewTask);
        addTaskButton.onClick.AddListener(ShowAddTaskPanel);
        closeButton.onClick.AddListener(CloseTaskPanel);
        
        // 绑定分类按钮事件
        importantUrgentButton.onClick.AddListener(() => ShowCategory(importantUrgentContainer, "重要且紧急"));
        notImportantUrgentButton.onClick.AddListener(() => ShowCategory(notImportantUrgentContainer, "不重要且紧急"));
        importantNotUrgentButton.onClick.AddListener(() => ShowCategory(importantNotUrgentContainer, "重要且不紧急"));
        notImportantNotUrgentButton.onClick.AddListener(() => ShowCategory(notImportantNotUrgentContainer, "不重要且不紧急"));
        
        // 默认显示重要且紧急分类
        ShowCategory(importantUrgentContainer, "重要且紧急");
        addTaskPanel.SetActive(false);

        // 初始化删除区域 - 默认隐藏
        if (deleteArea != null)
        {
            deleteArea.gameObject.SetActive(false);
            if (deleteAreaImage != null)
            {
                deleteAreaImage.color = new Color(1f, 0.5f, 0.5f, 0.3f); // 半透明红色背景
            }
            if (deleteAreaText != null)
            {
                deleteAreaText.text = "拖拽至此删除";
            }
        }

        LoadTodoItems();
    }

    public void ShowAddTaskPanel()
    {
        addTaskPanel.SetActive(true);
    }

    public void CloseTaskPanel()
    {
        Transform grandpa = closeButton.transform.parent.parent;
        grandpa.gameObject.SetActive(false);
    }

    private void AddNewTask()
    {
        if (string.IsNullOrEmpty(taskContentInput.text))
            return;

        TodoItem newItem = new TodoItem
        {
            content = taskContentInput.text,
            isImportant = importantToggle.isOn,
            isUrgent = urgentToggle.isOn,
            dueDate = ParseDueDate(dueDateInput.text),
            isCompleted = false
        };

        todoController.AddTodoItem(newItem);
        AddTaskToUI(newItem);

        // 重置表单
        taskContentInput.text = "";
        importantToggle.isOn = false;
        urgentToggle.isOn = false;
        dueDateInput.text = "";
        addTaskPanel.SetActive(false);
    }

    private void CancelNewTask()
    {
        addTaskPanel.SetActive(false);
    }

    private DateTime ParseDueDate(string dateString)
    {
        if (DateTime.TryParse(dateString, out DateTime result))
            return result;
        return DateTime.Now.AddDays(7); // 默认7天后
    }

    private void LoadTodoItems()
    {
        ClearAllContainers();
        
        AddItemsToContainer(todoController.importantUrgent, importantUrgentContainer);
        AddItemsToContainer(todoController.notImportantUrgent, notImportantUrgentContainer);
        AddItemsToContainer(todoController.importantNotUrgent, importantNotUrgentContainer);
        AddItemsToContainer(todoController.notImportantNotUrgent, notImportantNotUrgentContainer);
    }

    private void AddItemsToContainer(List<TodoItem> items, Transform container)
    {
        foreach (TodoItem item in items)
        {
            AddTaskToUI(item, container);
        }
    }

    private void AddTaskToUI(TodoItem item, Transform parent = null)
    {
        if (parent == null)
        {
            if (item.isImportant && item.isUrgent)
                parent = importantUrgentContainer;
            else if (!item.isImportant && item.isUrgent)
                parent = notImportantUrgentContainer;
            else if (item.isImportant && !item.isUrgent)
                parent = importantNotUrgentContainer;
            else
                parent = notImportantNotUrgentContainer;
        }

        GameObject taskItem = Instantiate(taskItemPrefab, parent);
        TaskItemUI itemUI = taskItem.GetComponent<TaskItemUI>();
        itemUI.Initialize(item, OnTaskCompleted, OnTaskMoved, OnTaskDeleted, deleteArea);
    }

    private void ClearAllContainers()
    {
        ClearContainer(importantUrgentContainer);
        ClearContainer(notImportantUrgentContainer);
        ClearContainer(importantNotUrgentContainer);
        ClearContainer(notImportantNotUrgentContainer);
    }

    private void ClearContainer(Transform container)
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);
    }
    
    private void ShowCategory(Transform activeContainer, string categoryName)
    {
        // 隐藏所有容器
        importantUrgentContainer.gameObject.SetActive(false);
        notImportantUrgentContainer.gameObject.SetActive(false);
        importantNotUrgentContainer.gameObject.SetActive(false);
        notImportantNotUrgentContainer.gameObject.SetActive(false);
        
        // 激活选中的容器
        activeContainer.gameObject.SetActive(true);
        
        // 更新当前激活分类文本
        activeCategoryText.text = $"{categoryName}";
    }

    private void OnTaskCompleted(TodoItem item)
    {
        todoController.SaveTodoList();
    }

    private void OnTaskMoved(TodoItem item, Transform newContainer)
    {
        // 更新任务分类
        item.isImportant = newContainer == importantUrgentContainer || newContainer == importantNotUrgentContainer;
        item.isUrgent = newContainer == importantUrgentContainer || newContainer == notImportantUrgentContainer;
        
        // 从原列表移除
        RemoveItemFromController(item);
        
        // 添加到新分类
        todoController.AddTodoItem(item);
        todoController.SaveTodoList();
        
        // 重新加载UI保持顺序
        LoadTodoItems();
    }

    private void OnTaskDeleted(TodoItem item)
    {
        // 从控制器中移除任务
        RemoveItemFromController(item);
        todoController.SaveTodoList();
    }

    private void RemoveItemFromController(TodoItem item)
    {
        todoController.importantUrgent.Remove(item);
        todoController.notImportantUrgent.Remove(item);
        todoController.importantNotUrgent.Remove(item);
        todoController.notImportantNotUrgent.Remove(item);
    }

    public void ShowDeleteArea()
    {
        deleteArea.gameObject.SetActive(true);
        deleteAreaText.text = "拖拽至此删除";
    }

    public void HideDeleteArea()
    {
        deleteArea.gameObject.SetActive(false);
    }
}
