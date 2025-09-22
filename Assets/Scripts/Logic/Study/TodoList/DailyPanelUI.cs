using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyPanelUI : MonoBehaviour
{
    [Header("Type Category")]
    public Transform importantUrgentContainer;
    public Transform notImportantUrgentContainer;
    public Transform importantNotUrgentContainer;
    public Transform notImportantNotUrgentContainer;
    [Header("Daily Item Prefab")]
    public GameObject todoListItemPrefab;

    private TodoListController todoController;

    private void Awake()
    {
        todoController = GetComponentInParent<TodoListController>();
    }
    void OnEnable()
    {
        TodoListEventHandler.DailyItemCompleted += OnTaskCompleted;
    }

    void OnDisable()
    {
        TodoListEventHandler.DailyItemCompleted -= OnTaskCompleted;
    }
    private void Start()
    {
        RefreshDailyPanel();
    }

    private void RefreshDailyPanel()
    {
        RefreshContainer(todoController.todoListDataSO.importantUrgent, importantUrgentContainer);
        RefreshContainer(todoController.todoListDataSO.notImportantUrgent, notImportantUrgentContainer);
        RefreshContainer(todoController.todoListDataSO.importantNotUrgent, importantNotUrgentContainer);
        RefreshContainer(todoController.todoListDataSO.notImportantNotUrgent, notImportantNotUrgentContainer);
    }

    private void RefreshContainer(List<TodoItem> items, Transform container)
    {
        // step 1: clear
        int count = container.childCount;
        for (int i = 0; i < count; i++)
        {
            Destroy(container.GetChild(i).gameObject);
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
    }

    private void OnTaskMoved(TodoItem item, Transform newContainer)
    {

    }

    private void OnTaskDeleted(TodoItem item)
    {
    }
}
