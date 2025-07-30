using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // 添加场景管理命名空间

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Scene-loaded event for UI initialization
    public event Action OnSceneLoaded;

    // Game states
    public enum GameState { Idle, Studying, Resting }

    // Events
    public event Action<GameState> OnGameStateChanged;
    public GameState currentState = GameState.Idle;

    private TaskPanel taskPanel;
    private TaskSystem taskSystem;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();

            // Subscribe to scene change events
            SceneManager.sceneLoaded += OnSceneLoadedHandler;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoadedHandler(Scene scene, LoadSceneMode mode)
    {
        // Notify subscribers that a new scene is loaded
        OnSceneLoaded?.Invoke();
    }

    private void OnDestroy()
    {
        // Unsubscribe from scene change events
        SceneManager.sceneLoaded -= OnSceneLoadedHandler;
    }

    private void Start()
    {
        // todo
        UIManager.Instance.OpenPanel(UIConst.MainPanel);
        // print(GetPackageLocalData().Count);
        // print(GetPackageTable().DataList.Count);
    }

    // private bool isInitialized = false;

    private void InitializeGame()
    {
        // if (isInitialized) return;

        Debug.Log("初始化游戏");
        // 初始化 taskSystem
        taskSystem = FindObjectOfType<TaskSystem>();
        SetGameState(GameState.Idle);

        // isInitialized = true;
    }

    public void SetGameState(GameState newState)
    {
        Debug.Log($"SetGameState: {currentState} -> {newState}");
        if (currentState == newState) return;

        // 处理场景切换
        if (newState == GameState.Studying && currentState != GameState.Studying)
        {
            // 进入学习状态时切换到Studying场景
            SceneManager.LoadScene("Studying");
        }
        else if (newState == GameState.Idle && currentState == GameState.Studying)
        {
            // 退出学习状态时返回Home场景
            SceneManager.LoadScene("Home");
        }

        // 保存旧状态
        GameState previousState = currentState;
        currentState = newState;

        // 只在状态真正改变时触发事件
        if (previousState != newState)
        {
            OnGameStateChanged?.Invoke(newState);
        }

        // 避免在Idle状态重复触发
        if (newState != GameState.Idle)
        {
            switch (newState)
            {
                case GameState.Studying:
                    // 确保不在同一个状态下重复启动协程
                    if (previousState != GameState.Studying)
                    {
                        Debug.Log("开始学习会话");
                        StartCoroutine(taskSystem.StudySession());
                    }
                    break;
                case GameState.Resting:
                    // 确保不在同一个状态下重复启动协程
                    if (previousState != GameState.Resting)
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

        while (currentState == GameState.Resting && restTime < maxRestTime)
        {
            restTime += Time.deltaTime;
            yield return null;
        }

        // Return to studying after rest
        if (currentState == GameState.Resting)
        {
            SetGameState(GameState.Studying);
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
