using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        SetGameState(GameState.Idle);
        remainingStudyTime = studyDuration * 60; // Convert to seconds
    }

    public void SetGameState(GameState newState)
    {
        Debug.Log($"SetGameState: {currentState} -> {newState}");
        if (currentState == newState) return;

        currentState = newState;
        OnGameStateChanged?.Invoke(newState);

        switch (newState)
        {
            case GameState.Idle:
                Debug.Log("进入空闲状态");
                // 待处理：这里需要添加弹窗
                break;
            case GameState.Studying:
                Debug.Log("开始学习会话");
                StartCoroutine(StudySession());
                break;
            case GameState.Resting:
                Debug.Log("开始休息会话");
                StartCoroutine(RestSession());
                break;
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
                UIManager.Instance.ShowReadyToTaskReminder();
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
            UIManager.Instance.ShowReadyToTaskReminder();
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
            UIManager.Instance.ShowTaskEndReminder();
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
}
