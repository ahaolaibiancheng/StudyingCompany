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
    public string dueDateString; // 使用字符串存储日期
    public bool isCompleted;
    public int sortOrder; // 用于任务排序
    public string type; // 任务类型：工作/学业、个人健康、家庭生活、社交人际、个人成长/兴趣、财务管理
    public string creationTimeString; // 创建时间字符串
    public string completionTimeString; // 完成时间字符串

    // 属性用于访问DateTime值
    public DateTime dueDate
    {
        get
        {
            if (DateTime.TryParse(dueDateString, out DateTime result))
                return result;
            return DateTime.Now.AddDays(7); // 默认7天后
        }
        set
        {
            dueDateString = value.ToString("yyyy-MM-dd HH:mm:ss");
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
        if (string.IsNullOrEmpty(completionTimeString) || string.IsNullOrEmpty(creationTimeString))
            return 0;
        
        TimeSpan difference = completionTime - creationTime;
        return (int)Math.Ceiling(difference.TotalDays);
    }
}

public class TodoListController : MonoBehaviour
{
    public List<TodoItem> importantUrgent = new List<TodoItem>();
    public List<TodoItem> notImportantUrgent = new List<TodoItem>();
    public List<TodoItem> importantNotUrgent = new List<TodoItem>();
    public List<TodoItem> notImportantNotUrgent = new List<TodoItem>();

    // 类型特定的列表
    public List<TodoItem> workStudyList = new List<TodoItem>();
    public List<TodoItem> personalHealthList = new List<TodoItem>();
    public List<TodoItem> familyLifeList = new List<TodoItem>();
    public List<TodoItem> socialRelationsList = new List<TodoItem>();
    public List<TodoItem> personalGrowthList = new List<TodoItem>();
    public List<TodoItem> financialManagementList = new List<TodoItem>();

    // 类型分数
    public int workStudyScore = 0;
    public int personalHealthScore = 0;
    public int familyLifeScore = 0;
    public int socialRelationsScore = 0;
    public int personalGrowthScore = 0;
    public int financialManagementScore = 0;

    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "todoList.json");
        LoadTodoList();
    }

    public void AddTodoItem(TodoItem item)
    {
        // 设置创建时间（如果未设置）
        if (string.IsNullOrEmpty(item.creationTimeString))
        {
            item.creationTime = DateTime.Now;
        }

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

    // 当任务完成时调用，移动到类型列表
    public void CompleteTodoItem(TodoItem item)
    {
        // 从原列表移除
        importantUrgent.Remove(item);
        notImportantUrgent.Remove(item);
        importantNotUrgent.Remove(item);
        notImportantNotUrgent.Remove(item);

        // 设置完成时间
        item.completionTime = DateTime.Now;
        item.isCompleted = true;

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

        // 计算所有类型分数
        RecalculateTypeScores();

        SaveTodoList();
    }

    public void SaveTodoList()
    {
        TodoListData data = new TodoListData
        {
            importantUrgent = this.importantUrgent,
            notImportantUrgent = this.notImportantUrgent,
            importantNotUrgent = this.importantNotUrgent,
            notImportantNotUrgent = this.notImportantNotUrgent,
            workStudyList = this.workStudyList,
            personalHealthList = this.personalHealthList,
            familyLifeList = this.familyLifeList,
            socialRelationsList = this.socialRelationsList,
            personalGrowthList = this.personalGrowthList,
            financialManagementList = this.financialManagementList,
            workStudyScore = this.workStudyScore,
            personalHealthScore = this.personalHealthScore,
            familyLifeScore = this.familyLifeScore,
            socialRelationsScore = this.socialRelationsScore,
            personalGrowthScore = this.personalGrowthScore,
            financialManagementScore = this.financialManagementScore
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
            
            // 加载类型特定的列表
            workStudyList = data.workStudyList ?? new List<TodoItem>();
            personalHealthList = data.personalHealthList ?? new List<TodoItem>();
            familyLifeList = data.familyLifeList ?? new List<TodoItem>();
            socialRelationsList = data.socialRelationsList ?? new List<TodoItem>();
            personalGrowthList = data.personalGrowthList ?? new List<TodoItem>();
            financialManagementList = data.financialManagementList ?? new List<TodoItem>();
            
            // 加载类型分数
            workStudyScore = data.workStudyScore;
            personalHealthScore = data.personalHealthScore;
            familyLifeScore = data.familyLifeScore;
            socialRelationsScore = data.socialRelationsScore;
            personalGrowthScore = data.personalGrowthScore;
            financialManagementScore = data.financialManagementScore;
        }

        // 重新计算所有类型分数
        RecalculateTypeScores();
    }

    // 遍历所有类型列表成员，重新计算分数
    private void RecalculateTypeScores()
    {
        // 重置所有分数
        workStudyScore = 0;
        personalHealthScore = 0;
        familyLifeScore = 0;
        socialRelationsScore = 0;
        personalGrowthScore = 0;
        financialManagementScore = 0;

        // 遍历工作/学业列表
        foreach (var item in workStudyList)
        {
            workStudyScore += item.GetScore();
        }

        // 遍历个人健康列表
        foreach (var item in personalHealthList)
        {
            personalHealthScore += item.GetScore();
        }

        // 遍历家庭生活列表
        foreach (var item in familyLifeList)
        {
            familyLifeScore += item.GetScore();
        }

        // 遍历社交人际列表
        foreach (var item in socialRelationsList)
        {
            socialRelationsScore += item.GetScore();
        }

        // 遍历个人成长/兴趣列表
        foreach (var item in personalGrowthList)
        {
            personalGrowthScore += item.GetScore();
        }

        // 遍历财务管理列表
        foreach (var item in financialManagementList)
        {
            financialManagementScore += item.GetScore();
        }
    }

    // 公开方法用于重新计算类型分数（供UI调用）
    public void UpdateTypeScores()
    {
        RecalculateTypeScores();
        SaveTodoList();
    }

    [System.Serializable]
    private class TodoListData
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
        
        // 类型分数
        public int workStudyScore;
        public int personalHealthScore;
        public int familyLifeScore;
        public int socialRelationsScore;
        public int personalGrowthScore;
        public int financialManagementScore;
    }
}
