using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BaseInfoUI : BasePanel
{
    [Header("UI Components")]
    public Text playerNameText;
    public Text levelText;
    public Text scoreText;
    public Text levelDetailsText;
    private BaseInfoControl baseInfoControl;
    
    [Serializable]
    public class ScoreDetailEntry
    {
        public string type;
        public Text textComponent;
    }
    
    public List<ScoreDetailEntry> scoreDetailEntries = new List<ScoreDetailEntry>();
    
    protected override void Awake()
    {
        base.Awake();
        name = "BaseInfoPanel";
        
        // 获取BaseInfoControl引用
        baseInfoControl = FindObjectOfType<BaseInfoControl>();
        if (baseInfoControl == null)
        {
            Debug.LogError("BaseInfoControl not found in scene!");
            return;
        }
        
        // 初始化分数详情UI
        InitializeScoreDetails();
    }
    
    void Start()
    {
        UpdateUI();
    }
     
    // 初始化分数详情UI
    private void InitializeScoreDetails()
    {
        // 创建6个类型的分数显示
        string[] scoreTypes = new string[]
        {
            "工作/学业",
            "个人健康", 
            "家庭生活",
            "社交人际",
            "个人成长/兴趣",
            "财务管理"
        };

        foreach (string type in scoreTypes)
        {
            ScoreDetailEntry entry = scoreDetailEntries.Find(e => e.type == type);
            if (entry != null)
            {
                entry.textComponent.text = "0";
            }
        }
    }
    
    // 更新UI显示
    private void UpdateUI()
    {
        if (baseInfoControl == null) return;
        
        // 更新基本信息
        playerNameText.text = baseInfoControl.playerName;
        levelText.text = baseInfoControl.GetCurrentLevel();
        scoreText.text = $"总分: {baseInfoControl.totalScore}";
        levelDetailsText.text = baseInfoControl.GetLevelDetails();
        
        // 更新分数详情
        UpdateScoreDetails();
    }
    
    // 更新分数详情显示
    private void UpdateScoreDetails()
    {
        var scoreDetails = baseInfoControl.GetScoreDetails();
        
        foreach (var entry in scoreDetailEntries)
        {
            if (scoreDetails.TryGetValue(entry.type, out int value))
            {
                entry.textComponent.text = $"{value}";
            }
        }
    }
    
    // 打开面板
    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);
        UpdateUI(); // 每次打开时刷新数据
    }
    
    // 关闭面板
    public override void ClosePanel()
    {
        base.ClosePanel();
    }
    
    // 提供给外部调用的刷新方法
    public void RefreshUI()
    {
        UpdateUI();
    }
}