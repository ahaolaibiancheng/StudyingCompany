using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Unity.VisualScripting;

public class TodoListController : Singleton<TodoListController>
{
    public TodoListDataSO todoListDataSO;
    public DetailPanelUI detailPanel;
    private ScoreManager scoreManager;
    public RewardTable rewardTable;
    public event Action<FragmentType> OnTaskComplete;

    protected override void Awake()
    {
        base.Awake();
        scoreManager = ScoreManager.Instance;
    }

    private void Start()
    {
        LoadTodoList();
    }

    public void SaveTodoList()
    {
        SaveManager.Instance.Save(this.todoListDataSO, "TodoList");
    }

    public void LoadTodoList()
    {
        SaveManager.Instance.Load(this.todoListDataSO, "TodoList");
    }

    /// <summary>
    /// 新增或者重新编辑待办事项
    /// </summary>
    /// <param name="item"></param>
    public void AddTodoItem(TodoItem item)
    {
        todoListDataSO.AddTodoItem(item);
        SaveTodoList();
    }

    public void RemoveTodoItem(TodoItem item)
    {
        if (todoListDataSO == null || item == null)
        {
            return;
        }

        bool removed = todoListDataSO.RemoveTodoItem(item);
        if (removed)
        {
            SaveTodoList();
        }
    }

    /// <summary>
    /// 当任务完成时，移动到对应的类型列表
    /// </summary>
    /// <param name="item"></param>
    public void CompleteTodoItem(TodoItem item)
    {
        // 根据类型添加到相应列表
        todoListDataSO.CompleteTodoItem(item);
        // 计算所有类型分数
        RecalculateTypeScores();

        SaveTodoList();
    }

    public void CompleteDailyItem(TodoItem item)
    {
        // 更新该任务的总奖励
        todoListDataSO.CompleteDailyItem(item);

        // 通过概率表根据任务类型生成碎片类型
        FragmentType fragType = rewardTable.GetRandomFragment(item.taskType);

        // 广播事件
        OnTaskComplete?.Invoke(fragType);

        SaveTodoList();
    }

    // 遍历所有类型列表成员，重新计算分数
    private void RecalculateTypeScores()
    {
        // 重置所有分数
        scoreManager.scoreData.workStudyScore = 0;
        scoreManager.scoreData.personalHealthScore = 0;
        scoreManager.scoreData.familyLifeScore = 0;
        scoreManager.scoreData.socialRelationsScore = 0;
        scoreManager.scoreData.personalGrowthScore = 0;
        scoreManager.scoreData.financialManagementScore = 0;

        // 遍历工作/学业列表
        foreach (var item in todoListDataSO.workStudyList)
        {
            scoreManager.scoreData.workStudyScore += item.GetScore();
        }

        // 遍历个人健康列表
        foreach (var item in todoListDataSO.personalHealthList)
        {
            scoreManager.scoreData.personalHealthScore += item.GetScore();
        }

        // 遍历家庭生活列表
        foreach (var item in todoListDataSO.familyLifeList)
        {
            scoreManager.scoreData.familyLifeScore += item.GetScore();
        }

        // 遍历社交人际列表
        foreach (var item in todoListDataSO.socialRelationsList)
        {
            scoreManager.scoreData.socialRelationsScore += item.GetScore();
        }

        // 遍历个人成长/兴趣列表
        foreach (var item in todoListDataSO.personalGrowthList)
        {
            scoreManager.scoreData.personalGrowthScore += item.GetScore();
        }

        // 遍历财务管理列表
        foreach (var item in todoListDataSO.financialManagementList)
        {
            scoreManager.scoreData.financialManagementScore += item.GetScore();
        }
        scoreManager.SaveScore();
    }

    // 公开方法用于重新计算类型分数（供UI调用）
    public void UpdateTypeScores()
    {
        RecalculateTypeScores();
        SaveTodoList();
    }
}
