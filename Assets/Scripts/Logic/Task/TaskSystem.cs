using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TaskSystem : MonoBehaviour
{
    public static TaskSystem Instance;
    [Header("任务时间")]
    public int timeBeforeReminder = 5;  // 提前通知时长
    public float studyDuration = 45f; // 学习时间片时长
    public float currentSessionTime; // 累计学习时长
    public float remainingStudyTime;    // 学习时间片剩余时长
    
    private Coroutine startTaskCoroutine; // 用于存储开始任务的协程
    private Coroutine reminderCoroutine; // 存储协程引用
    private DateTime taskStartTime;
    private DateTime taskEndTime;
    private bool isStudyPaused = false;

    [System.Serializable]
    public class StudyTask
    {
        public string taskName;
        public DateTime startTime;
        public DateTime endTime;
        public bool isDaily;
        public bool isCompleted;
    }

    private List<StudyTask> tasks = new List<StudyTask>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        InitializeTask();
        LoadTasks();

        taskStartTime = DateTime.Now - TimeSpan.FromMinutes(5);
        taskEndTime = DateTime.Now + TimeSpan.FromSeconds(10);
    }

    private void InitializeTask()
    {
        remainingStudyTime = studyDuration * 60; // Convert to seconds
    }

    private void LoadTasks()
    {
        if (PlayerPrefs.HasKey("StudyTasks"))
        {
            string json = PlayerPrefs.GetString("StudyTasks");
            TaskWrapper wrapper = JsonUtility.FromJson<TaskWrapper>(json);
            tasks = wrapper.tasks;
        }
    }

    private void Start()
    {
        InvokeRepeating("CheckTasks", 0f, 60f); // Check every minute
    }

    public void CreateTask(string name, DateTime start, DateTime end, bool daily)
    {
        StudyTask newTask = new StudyTask
        {
            taskName = name,
            startTime = start,
            endTime = end,
            isDaily = daily,
            isCompleted = false
        };

        tasks.Add(newTask);
        SaveTasks();
    }

    private void CheckTasks()
    {
        DateTime now = DateTime.Now;

        foreach (var task in tasks)
        {
            if (!task.isCompleted && now >= task.startTime && now < task.endTime)
            {
                // Trigger task reminder
                StartNewTask(task.startTime, task.endTime);

                // Mark as completed if it's a one-time task
                if (!task.isDaily)
                {
                    task.isCompleted = true;
                }
            }
            else if (task.isDaily && now.Date > task.startTime.Date)
            {
                // Reset daily task for next day
                if (now.Hour == 0 && now.Minute == 0)
                {
                    task.startTime = task.startTime.AddDays(1);
                    task.endTime = task.endTime.AddDays(1);
                    task.isCompleted = false;
                }
            }
        }

        SaveTasks();
    }

    public List<StudyTask> GetTasks()
    {
        return tasks;
    }

    private void SaveTasks()
    {
        string json = JsonUtility.ToJson(new TaskWrapper { tasks = tasks });
        PlayerPrefs.SetString("StudyTasks", json);
        PlayerPrefs.Save();
    }

    // Wrapper class for JSON serialization
    [System.Serializable]
    private class TaskWrapper
    {
        public List<StudyTask> tasks;
    }

    private IEnumerator WaitForTaskStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.SetCharacterState(CharacterState.Studying);
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
            GameManager.Instance.SetCharacterState(CharacterState.Studying);
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
                UIManager.Instance.OpenPanel(UIConst.ReminderPanel);
                (UIManager.Instance.GetPanel(UIConst.ReminderPanel) as ReminderPanel)?.ShowReadyToTaskReminder();
            }
            else
            {
                Debug.LogError("UIManager instance is null in TaskPanel.StartNewTask");
            }
        }
    }

    private IEnumerator ReminderCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OpenPanel(UIConst.ReminderPanel);
            (UIManager.Instance.GetPanel(UIConst.ReminderPanel) as ReminderPanel)?.ShowReadyToTaskReminder();
        }
        else
        {
            Debug.LogError("UIManager instance is null in TaskPanel.ReminderCoroutine");
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

    public IEnumerator StudySession()
    {
        Debug.Log("StudySession 开始");
        isStudyPaused = false;

        // 不再重置 currentSessionTime，而是持续累加
        while (GameManager.Instance.currentState == CharacterState.Studying)
        {
            if (!isStudyPaused)
            {
                currentSessionTime += Time.deltaTime;
                remainingStudyTime -= Time.deltaTime;

                // Debug.Log($"更新学习时间: session={currentSessionTime}, remaining={remainingStudyTime}");
                EventHandler.CallStudyTimeUpdatedEvent(remainingStudyTime);

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

    public void CompleteTask()
    {
        // InventorySystem.Instance.AddRandomItem();
        GameManager.Instance.SetCharacterState(CharacterState.Idle);

        // 完成任务时重置累计时间
        currentSessionTime = 0f;

        // 取消提醒计时器
        CancelReminderTimer();

        // 显示任务结束提醒
        // UIManager.Instance.ShowTaskEndReminder();

        // Reward player with random item
        UIManager.Instance.OpenPanel(UIConst.RewardPanel);
        EventHandler.CallStudyEndEvent();
    }

    public void PauseStudy()
    {
        isStudyPaused = true;
    }

    public void ResumeStudy()
    {
        isStudyPaused = false;
    }

}
