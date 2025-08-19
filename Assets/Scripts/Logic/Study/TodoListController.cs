using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public class TodoItem
{
    public string content;
    public bool isImportant;
    public bool isUrgent;
    public DateTime dueDate;
    public bool isCompleted;
    public int sortOrder; // 用于任务排序
}

public class TodoListController : MonoBehaviour
{
    public List<TodoItem> importantUrgent = new List<TodoItem>();
    public List<TodoItem> notImportantUrgent = new List<TodoItem>();
    public List<TodoItem> importantNotUrgent = new List<TodoItem>();
    public List<TodoItem> notImportantNotUrgent = new List<TodoItem>();

    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "todoList.json");
        LoadTodoList();
    }

    public void AddTodoItem(TodoItem item)
    {
        if (item.isImportant && item.isUrgent)
            importantUrgent.Add(item);
        else if (!item.isImportant && item.isUrgent)
            notImportantUrgent.Add(item);
        else if (item.isImportant && !item.isUrgent)
            importantNotUrgent.Add(item);
        else
            notImportantNotUrgent.Add(item);
        
        SaveTodoList();
    }

    public void SaveTodoList()
    {
        TodoListData data = new TodoListData
        {
            importantUrgent = this.importantUrgent,
            notImportantUrgent = this.notImportantUrgent,
            importantNotUrgent = this.importantNotUrgent,
            notImportantNotUrgent = this.notImportantNotUrgent
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadTodoList()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            TodoListData data = JsonUtility.FromJson<TodoListData>(json);

            importantUrgent = data.importantUrgent ?? new List<TodoItem>();
            notImportantUrgent = data.notImportantUrgent ?? new List<TodoItem>();
            importantNotUrgent = data.importantNotUrgent ?? new List<TodoItem>();
            notImportantNotUrgent = data.notImportantNotUrgent ?? new List<TodoItem>();
        }
    }

    [System.Serializable]
    private class TodoListData
    {
        public List<TodoItem> importantUrgent;
        public List<TodoItem> notImportantUrgent;
        public List<TodoItem> importantNotUrgent;
        public List<TodoItem> notImportantNotUrgent;
    }
}
