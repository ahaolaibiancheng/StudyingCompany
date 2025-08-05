using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // 添加场景管理命名空间

public class GameManager : Singleton<GameManager>
{
    public CharacterState currentState;
    protected override void Awake()
    {
        base.Awake();
        InitializeGame();
    }
    private void Start()
    {
        currentState = CharacterState.Idle;
        Debug.Log("Screen.width/height: " + Screen.width + "/" + Screen.height);
    }

    private void InitializeGame()
    {
        Debug.Log("游戏开始");
        SetCharacterState(CharacterState.Idle);
    }

    public void SetCharacterState(CharacterState newState)
    {
        Debug.Log($"SetCharacterState: {currentState} -> {newState}");
        if (currentState == newState) return;

        // 处理场景切换
        if (newState == CharacterState.Studying && currentState != CharacterState.Studying)
        {
            // 进入学习状态时切换到Studying场景
            SceneManager.LoadScene("Studying");
        }
        else if (newState == CharacterState.Idle && currentState == CharacterState.Studying)
        {
            // 退出学习状态时返回Home场景
            SceneManager.LoadScene("Home");
        }

        // 保存旧状态
        CharacterState previousState = currentState;
        currentState = newState;

        // 只在状态真正改变时触发事件
        if (previousState != newState)
        {
            EventHandler.CallCharacterStateChangedEvent(newState);
        }

        // 避免在Idle状态重复触发
        if (newState != CharacterState.Idle)
        {
            switch (newState)
            {
                case CharacterState.Studying:
                    // 确保不在同一个状态下重复启动协程
                    if (previousState != CharacterState.Studying)
                    {
                        Debug.Log("开始学习会话");
                        StartCoroutine(TaskSystem.Instance.StudySession());
                    }
                    break;
                case CharacterState.Resting:
                    // 确保不在同一个状态下重复启动协程
                    if (previousState != CharacterState.Resting)
                    {
                        Debug.Log("开始休息会话");
                        StartCoroutine(RestSession());
                    }
                    break;
            }
        }
    }

    private IEnumerator RestSession()
    {
        Debug.Log("RestSession 开始");
        float restTime = 0f;
        float maxRestTime = 5 * 60; // 5 minutes in seconds

        while (currentState == CharacterState.Resting && restTime < maxRestTime)
        {
            restTime += Time.deltaTime;
            yield return null;
        }

        // Return to studying after rest
        if (currentState == CharacterState.Resting)
        {
            SetCharacterState(CharacterState.Studying);
        }
        Debug.Log("RestSession 结束");
    }

    private PackageTable packageTable;

    public PackageTable GetPackageTable()
    {
        if (packageTable == null)
        {
            packageTable = Resources.Load<PackageTable>("TableData/PackageTable");
        }
        return packageTable;
    }

    public List<PackageLocalItem> GetPackageLocalData()
    {
        return PackageLocalData.Instance.LoadPackage();
    }

    public PackageTableItem GetPackageItemById(int id)
    {
        List<PackageTableItem> packageDataList = GetPackageTable().DataList;
        foreach (PackageTableItem item in packageDataList)
        {
            if (item.id == id)
            {
                return item;
            }
        }
        return null;
    }

    public PackageLocalItem GetPackageLocalItemByUId(string uid)
    {
        List<PackageLocalItem> packageDataList = GetPackageLocalData();
        foreach (PackageLocalItem item in packageDataList)
        {
            if (item.uid == uid)
            {
                return item;
            }
        }
        return null;
    }

    public List<PackageLocalItem> GetSortPackageLocalData()
    {
        List<PackageLocalItem> localItems = PackageLocalData.Instance.LoadPackage();
        localItems.Sort(new PackageItemComparer());
        return localItems;
    }
}

public class PackageItemComparer : IComparer<PackageLocalItem>
{
    public int Compare(PackageLocalItem a, PackageLocalItem b)
    {
        PackageTableItem x = GameManager.Instance.GetPackageItemById(a.id);
        PackageTableItem y = GameManager.Instance.GetPackageItemById(b.id);
        // 首先按star从大到小排序
        int starComparison = y.star.CompareTo(x.star);

        // 如果star相同，则按id从大到小排序
        if (starComparison == 0)
        {
            int idComparison = y.id.CompareTo(x.id);
            if (idComparison == 0)
            {
                return b.level.CompareTo(a.level);
            }
            return idComparison;
        }

        return starComparison;
    }
}
