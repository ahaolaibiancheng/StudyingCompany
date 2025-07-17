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

    void Update()
    {
        // Debug.Log($"GameManager State: {currentState}");
        // Debug.Log($"RemainingStudyTime: {remainingStudyTime}");
        // Debug.Log($"CurrentSessionTime: {currentSessionTime}");
        // Debug.Log($"isStudyPaused: {isStudyPaused}");
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

    public void StartNewTask(DateTime startTime, DateTime endTime)
    {
        taskStartTime = startTime;
        taskEndTime = endTime;
        currentSessionTime = 0f; // 开始新任务时重置累计时间
        remainingStudyTime = studyDuration * 60; // 重置剩余学习时间
        UIManager.Instance.ShowTaskReminder();
    }

    public void ConfirmTaskStart()
    {
        if (DateTime.Now >= taskStartTime && DateTime.Now < taskEndTime)
        {
            Debug.Log("ConfirmTaskStart: 满足条件，开始学习");
            SetGameState(GameState.Studying);
        }
        else
        {
            Debug.LogWarning($"ConfirmTaskStart: 当前时间 {DateTime.Now} 晚于任务结束时间 {taskEndTime}");
        }
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
                
                Debug.Log($"更新学习时间: session={currentSessionTime}, remaining={remainingStudyTime}");
                OnStudyTimeUpdated?.Invoke(remainingStudyTime);

                // Check if study time is completed
                if (remainingStudyTime <= 0)
                {
                    Debug.Log("学习时间结束");
                    PetController.Instance.TriggerReminder();
                }

                // Check if task time has ended
                if (DateTime.Now >= taskEndTime)
                {
                    Debug.Log("任务时间结束");
                    SetGameState(GameState.Idle);
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
        InventorySystem.Instance.AddRandomItem();
        SetGameState(GameState.Idle);
        
        // 完成任务时重置累计时间
        currentSessionTime = 0f;
    }

    // Public properties
    public GameState CurrentState => currentState;
    public float RemainingStudyTime => remainingStudyTime;
    public float CurrentSessionTime => currentSessionTime;
}
