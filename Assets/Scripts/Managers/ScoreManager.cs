using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ScoreManager : MonoBehaviour
{
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
        savePath = Path.Combine(Application.persistentDataPath, "score.json");
    }

    public void SaveScore()
    {
        ScoreData data = new ScoreData
        {
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

    public void LoadScore()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            ScoreData data = JsonUtility.FromJson<ScoreData>(json);

            // 加载类型分数
            workStudyScore = data.workStudyScore;
            personalHealthScore = data.personalHealthScore;
            familyLifeScore = data.familyLifeScore;
            socialRelationsScore = data.socialRelationsScore;
            personalGrowthScore = data.personalGrowthScore;
            financialManagementScore = data.financialManagementScore;
        }
    }

    [System.Serializable]
    private class ScoreData
    {
        // 类型分数
        public int workStudyScore;
        public int personalHealthScore;
        public int familyLifeScore;
        public int socialRelationsScore;
        public int personalGrowthScore;
        public int financialManagementScore;
    }
}
