using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement; // 添加场景管理命名空间
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Scene-loaded event for UI initialization
    public event Action OnSceneLoaded;

    // Game states
    public enum GameState { Idle, Studying, Resting }
    private GameState currentState = GameState.Idle;

    // Timing variables
    private DateTime taskStartTime;
    private DateTime taskEndTime;
    private float studyDuration = 45f; // Default 45 minutes
    private float remainingStudyTime;
    private float currentSessionTime; // 累计学习时间（跨会话）
    private bool isStudyPaused = false;
    public int timeBeforeReminder = 5;

    // Events
    public event Action<GameState> OnGameStateChanged;
    public event Action<float> OnStudyTimeUpdated;

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

    private bool isInitialized = false;

    private void InitializeGame()
    {
        if (isInitialized) return;

        Debug.Log("初始化游戏");
        SetGameState(GameState.Idle);
        remainingStudyTime = studyDuration * 60; // Convert to seconds

        isInitialized = true;
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
                        StartCoroutine(StudySession());
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

    private Coroutine reminderCoroutine; // 存储协程引用

    public void StartNewTask(DateTime startTime, DateTime endTime)
    {
        taskStartTime = startTime;
        taskEndTime = endTime;
        currentSessionTime = 0f; // 开始新任务时重置累计时间
        remainingStudyTime = studyDuration * 60; // 重置剩余学习时间

        // 取消现有计时器
        CancelReminderTimer();

        // 计算距离任务开始前5分钟的时间差（秒）
        TimeSpan timeToReminder = startTime - DateTime.Now - TimeSpan.FromMinutes(timeBeforeReminder);
        float delaySeconds = (float)timeToReminder.TotalSeconds;

        if (delaySeconds > 0)
        {
            // 启动协程定时器
            reminderCoroutine = StartCoroutine(ReminderCoroutine(delaySeconds));
        }
        else
        {
            Debug.Log("任务开始时间不足5分钟，立即提醒");
            if (UIManager.Instance != null)
            {
                // UIManager.Instance.ShowReadyToTaskReminder();
            }
            else
            {
                Debug.LogError("UIManager instance is null in GameManager.StartNewTask");
            }
        }
    }

    private IEnumerator ReminderCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (UIManager.Instance != null)
        {
            // UIManager.Instance.ShowReadyToTaskReminder();s
        }
        else
        {
            Debug.LogError("UIManager instance is null in GameManager.ReminderCoroutine");
        }
    }

    private void CancelReminderTimer()
    {
        if (reminderCoroutine != null)
        {
            StopCoroutine(reminderCoroutine);
            reminderCoroutine = null;
        }
    }

    private Coroutine startTaskCoroutine; // 用于存储开始任务的协程

    public void ConfirmTaskStart()
    {
        // 取消可能正在进行的等待
        CancelStartTaskWait();

        if (DateTime.Now >= taskEndTime)
        {
            Debug.LogWarning($"ConfirmTaskStart: 当前时间 {DateTime.Now} 晚于任务结束时间 {taskEndTime}");
            return;
        }

        if (DateTime.Now >= taskStartTime)
        {
            // 如果已经过了开始时间，立即开始
            SetGameState(GameState.Studying);
        }
        else
        {
            // 计算到任务开始时间还有多久
            TimeSpan timeToStart = taskStartTime - DateTime.Now;
            float waitSeconds = (float)timeToStart.TotalSeconds;

            Debug.Log($"等待任务开始: 还有 {timeToStart.TotalSeconds} 秒");
            startTaskCoroutine = StartCoroutine(WaitForTaskStart(waitSeconds));
        }
    }

    private IEnumerator WaitForTaskStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetGameState(GameState.Studying);
    }

    private void CancelStartTaskWait()
    {
        if (startTaskCoroutine != null)
        {
            StopCoroutine(startTaskCoroutine);
            startTaskCoroutine = null;
        }
    }

    public void CancelTask()
    {
        CancelStartTaskWait();
        // 其他取消逻辑...
    }

    private IEnumerator StudySession()
    {
        Debug.Log("StudySession 开始");
        isStudyPaused = false;

        // 不再重置 currentSessionTime，而是持续累加
        while (currentState == GameState.Studying)
        {
            if (!isStudyPaused)
            {
                currentSessionTime += Time.deltaTime;
                remainingStudyTime -= Time.deltaTime;

                // Debug.Log($"更新学习时间: session={currentSessionTime}, remaining={remainingStudyTime}");
                OnStudyTimeUpdated?.Invoke(remainingStudyTime);

                // 检查学习时间是否结束
                if (remainingStudyTime <= 0)
                {
                    Debug.Log("学习时间结束");
                    PetController.Instance.TriggerReminder();
                }

                // 检查任务时间是否结束
                if (DateTime.Now >= taskEndTime)
                {
                    Debug.Log("任务时间结束");
                    CompleteTask(); // 调用任务完成方法
                    yield break;
                }
            }
            yield return null;
        }
        Debug.Log("StudySession 结束 - 状态已改变");
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

    public void PauseStudy()
    {
        isStudyPaused = true;
    }

    public void ResumeStudy()
    {
        isStudyPaused = false;
    }

    public void CompleteTask()
    {
        // Reward player with random item
        // InventorySystem.Instance.AddRandomItem();
        SetGameState(GameState.Idle);

        // 完成任务时重置累计时间
        currentSessionTime = 0f;

        // 取消提醒计时器
        CancelReminderTimer();

        // 显示任务结束提醒
        if (UIManager.Instance != null)
        {
            // UIManager.Instance.ShowTaskEndReminder();
        }
        else
        {
            Debug.LogError("UIManager instance is null in GameManager.CompleteTask");
        }
    }

    // Public properties
    public GameState CurrentState => currentState;
    public float RemainingStudyTime => remainingStudyTime;
    public float CurrentSessionTime => currentSessionTime;



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
