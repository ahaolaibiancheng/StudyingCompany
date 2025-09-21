using System;
using UnityEngine;

public static class TodoListEventHandler
{
    /// <summary>
    /// 触发任务完成事件
    /// </summary>
    /// <param name="todoItem"></param>
    public static event Action<TodoItem> TodoItemCompleted;
    public static void CallTodoItemCompleted(TodoItem todoItem)
    {
        TodoItemCompleted?.Invoke(todoItem);
    }
}