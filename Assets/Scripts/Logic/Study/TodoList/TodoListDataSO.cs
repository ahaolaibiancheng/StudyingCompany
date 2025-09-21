
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataTables/TodoListDataSO", fileName = "New TodoListDataSO")]
public class TodoListDataSO : ScriptableObject
{
    public List<TodoItem> importantUrgent;
    public List<TodoItem> notImportantUrgent;
    public List<TodoItem> importantNotUrgent;
    public List<TodoItem> notImportantNotUrgent;

    // 类型特定的列表
    public List<TodoItem> workStudyList;
    public List<TodoItem> personalHealthList;
    public List<TodoItem> familyLifeList;
    public List<TodoItem> socialRelationsList;
    public List<TodoItem> personalGrowthList;
    public List<TodoItem> financialManagementList;

    public void AddTodoItem(TodoItem item)
    {
        if (item.isImportant && item.isUrgent)
            this.importantUrgent.Add(item);
        else if (!item.isImportant && item.isUrgent)
            this.notImportantUrgent.Add(item);
        else if (item.isImportant && !item.isUrgent)
            this.importantNotUrgent.Add(item);
        else
            this.notImportantNotUrgent.Add(item);
    }

    public void CompleteTodoItem(TodoItem item)
    {
        // 从原列表移除
        this.importantUrgent.Remove(item);
        this.notImportantUrgent.Remove(item);
        this.importantNotUrgent.Remove(item);
        this.notImportantNotUrgent.Remove(item);

        // 设置完成时间
        item.isCompleted = true;
        item.completionTime = DateTime.Now;

        // 根据类型添加到相应列表
        switch (item.type)
        {
            case "工作/学业":
                workStudyList.Add(item);
                break;
            case "个人健康":
                personalHealthList.Add(item);
                break;
            case "家庭生活":
                familyLifeList.Add(item);
                break;
            case "社交人际":
                socialRelationsList.Add(item);
                break;
            case "个人成长/兴趣":
                personalGrowthList.Add(item);
                break;
            case "财务管理":
                financialManagementList.Add(item);
                break;
            default:
                // 如果没有类型，默认添加到工作/学业
                workStudyList.Add(item);
                break;
        }
    }
}

[System.Serializable]
public class TodoItem
{
    public string guid;    // 目标的唯一标识
    public string keywords;  // 简述
    public bool isImportant;
    public bool isUrgent;
    public bool isCompleted;
    public string type; // 任务类型：工作/学业、个人健康、家庭生活、社交人际、个人成长/兴趣、财务管理
    public int rewardDaily; // 每日奖励/奖励系数
    public int rewardTotal; // 已累计奖励
    public int completedTimes;   // 已完成次数
    public bool[] period;   // 周期
    public string description;  // 详细描述
    public int sortOrder; // 用于任务排序

    // 序列化用的字符串字段
    public string creationTimeString;
    public string deadlineTimeString;
    public string completionTimeString;

    // 截止时间属性
    public DateTime deadlineTime
    {
        get
        {
            if (DateTime.TryParse(deadlineTimeString, out DateTime result))
                return result;
            return DateTime.Now.AddDays(7); // 默认7天后
        }
        set
        {
            deadlineTimeString = value.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    // 创建时间属性
    public DateTime creationTime
    {
        get
        {
            if (DateTime.TryParse(creationTimeString, out DateTime result))
                return result;
            return DateTime.Now; // 默认当前时间
        }
        set
        {
            creationTimeString = value.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    // 完成时间属性
    public DateTime completionTime
    {
        get
        {
            if (DateTime.TryParse(completionTimeString, out DateTime result))
                return result;
            return DateTime.Now; // 默认当前时间
        }
        set
        {
            completionTimeString = value.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    // 计算分数（完成时间 - 创建时间的差值，以天为单位，向上取整）
    public int GetScore()
    {
        TimeSpan difference = completionTime - creationTime;
        return (int)Math.Ceiling(difference.TotalDays);
    }
}
