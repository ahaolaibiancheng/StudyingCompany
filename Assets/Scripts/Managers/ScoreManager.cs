using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Unity.VisualScripting;

public class ScoreManager : Singleton<ScoreManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    [SerializeField] public ScoreData scoreData;

    private void Start()
    {
        LoadScore();
    }

    public void SaveScore()
    {
        SaveManager.Instance.Save(this.scoreData, "ScoreData");
    }

    public void LoadScore()
    {
        SaveManager.Instance.Load(this.scoreData, "ScoreData");
    }
    [System.Serializable]
    public class ScoreData
    {
        public int workStudyScore;
        public int personalHealthScore;
        public int familyLifeScore;
        public int socialRelationsScore;
        public int personalGrowthScore;
        public int financialManagementScore;
    }
}
