using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseInfoControl : MonoBehaviour
{
    [Header("Player Info")]
    public string playerName = "修仙者";
    public int totalScore = 100; // 基础分100分

    private int baseScore = 100; // 基础分数
    private int qiRefiningLayers = 13; // 炼气期层数
    private int scorePerLayer = 10; // 每层所需分数

    // 境界分数范围
    private readonly Dictionary<string, (int min, int max)> realmRanges = new Dictionary<string, (int, int)>
    {
        {"炼气期", (100, 229)},
        {"筑基初期", (230, 299)},
        {"筑基中期", (300, 389)},
        {"筑基后期", (390, 499)},
        {"结丹初期", (500, 629)},
        {"结丹中期", (630, 779)},
        {"结丹后期", (780, 959)},
        {"结丹大圆满", (960, 999)},
        {"元婴初期", (1000, 1199)},
        {"元婴中期", (1200, 1499)},
        {"元婴后期", (1500, 1899)},
        {"元婴大圆满", (1900, 1999)},
        {"化神期", (2000, int.MaxValue)}
    };

    private ScoreManager scoreManager;

    void Awake()
    {
        // 获取TodoListController引用
        scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager == null)
        {
            Debug.LogError("TodoListController not found in scene!");
        }

        // 初始化时计算总分
        CalculateTotalScore();
    }

    // 计算总分（从TodoListController获取各类型分数并求和）
    public void CalculateTotalScore()
    {
        if (scoreManager == null) return;

        scoreManager.LoadScore();
        totalScore = baseScore +
                   scoreManager.scoreData.workStudyScore +
                   scoreManager.scoreData.personalHealthScore +
                   scoreManager.scoreData.familyLifeScore +
                   scoreManager.scoreData.socialRelationsScore +
                   scoreManager.scoreData.personalGrowthScore +
                   scoreManager.scoreData.financialManagementScore;
    }

    // 获取当前境界等级
    public string GetCurrentLevel()
    {
        foreach (var realm in realmRanges)
        {
            if (totalScore >= realm.Value.min && totalScore <= realm.Value.max)
            {
                if (realm.Key == "炼气期")
                {
                    return GetQiRefiningLayer();
                }
                return realm.Key;
            }
        }
        return "未知境界";
    }

    // 获取炼气期具体层数
    private string GetQiRefiningLayer()
    {
        if (totalScore < baseScore) return "炼气期0层";

        int layer = (totalScore - baseScore) / scorePerLayer + 1;
        layer = Mathf.Clamp(layer, 1, qiRefiningLayers);
        return $"炼气期{layer}层";
    }

    // 获取境界详情（当前分数/下一境界所需分数）
    public string GetLevelDetails()
    {
        string currentLevel = GetCurrentLevel();
        (int min, int max) = GetCurrentRealmRange();

        if (currentLevel.Contains("炼气期"))
        {
            int currentLayer = GetCurrentQiRefiningLayer();
            if (currentLayer < qiRefiningLayers)
            {
                int nextLayerScore = baseScore + currentLayer * scorePerLayer;
                int currentLayerScore = baseScore + (currentLayer - 1) * scorePerLayer;
                return $"当前: {currentLayerScore}-{nextLayerScore - 1}分\n下一层: {nextLayerScore}分";
            }
            else
            {
                return $"当前: {min}-{max}分\n下一境界: 筑基初期(230分)";
            }
        }

        // 查找下一境界
        string nextRealm = GetNextRealm();
        if (nextRealm != null)
        {
            (int nextMin, int nextMax) = realmRanges[nextRealm];
            return $"当前: {min}-{max}分\n下一境界: {nextRealm}({nextMin}分)";
        }

        return $"当前: {min}+分\n已达最高境界";
    }

    // 获取当前境界的分数范围
    private (int min, int max) GetCurrentRealmRange()
    {
        string currentLevel = GetCurrentLevel();
        foreach (var realm in realmRanges)
        {
            if (currentLevel.Contains(realm.Key) || realm.Key.Contains(currentLevel))
            {
                return realm.Value;
            }
        }
        return (0, 0);
    }

    // 获取下一境界
    private string GetNextRealm()
    {
        string currentLevel = GetCurrentLevel();
        bool foundCurrent = false;

        foreach (var realm in realmRanges)
        {
            if (foundCurrent)
            {
                return realm.Key;
            }
            if (currentLevel.Contains(realm.Key) || realm.Key.Contains(currentLevel))
            {
                foundCurrent = true;
            }
        }
        return null; // 已是最高境界
    }

    // 获取炼气期当前层数
    private int GetCurrentQiRefiningLayer()
    {
        if (totalScore < baseScore) return 0;
        int layer = (totalScore - baseScore) / scorePerLayer + 1;
        return Mathf.Clamp(layer, 1, qiRefiningLayers);
    }

    // 获取境界进度百分比
    public float GetLevelProgressPercentage()
    {
        string currentLevel = GetCurrentLevel();
        (int min, int max) = GetCurrentRealmRange();

        if (currentLevel.Contains("炼气期"))
        {
            int currentLayer = GetCurrentQiRefiningLayer();
            if (currentLayer < qiRefiningLayers)
            {
                int currentLayerMin = baseScore + (currentLayer - 1) * scorePerLayer;
                int currentLayerMax = baseScore + currentLayer * scorePerLayer - 1;
                float progress = (float)(totalScore - currentLayerMin) / (currentLayerMax - currentLayerMin);
                return Mathf.Clamp01(progress);
            }
        }

        if (max == int.MaxValue) return 1.0f; // 化神期已达最高

        float realmProgress = (float)(totalScore - min) / (max - min);
        return Mathf.Clamp01(realmProgress);
    }

    // 获取各类型分数详情
    public Dictionary<string, int> GetScoreDetails()
    {
        if (scoreManager == null)
        {
            return new Dictionary<string, int>
            {
                {"工作/学业", 0},
                {"个人健康", 0},
                {"家庭生活", 0},
                {"社交人际", 0},
                {"个人成长/兴趣", 0},
                {"财务管理", 0}
            };
        }

        scoreManager.LoadScore();
        return new Dictionary<string, int>
        {
            {"工作/学业", scoreManager.scoreData.workStudyScore},
            {"个人健康", scoreManager.scoreData.personalHealthScore},
            {"家庭生活", scoreManager.scoreData.familyLifeScore},
            {"社交人际", scoreManager.scoreData.socialRelationsScore},
            {"个人成长/兴趣", scoreManager.scoreData.personalGrowthScore},
            {"财务管理", scoreManager.scoreData.financialManagementScore}
        };
    }

    // 设置玩家名称
    public void SetPlayerName(string newName)
    {
        if (!string.IsNullOrEmpty(newName))
        {
            playerName = newName;
            // 这里可以添加保存到PlayerPrefs或文件的逻辑
            Debug.Log($"玩家名称已更新为: {playerName}");
        }
    }

    // 更新总分（当TodoListController分数变化时调用）
    public void UpdateTotalScore()
    {
        CalculateTotalScore();
        Debug.Log($"总分已更新: {totalScore}, 当前境界: {GetCurrentLevel()}");
    }

    // 提供给外部调用的刷新方法
    public void RefreshData()
    {
        CalculateTotalScore();
    }
}
