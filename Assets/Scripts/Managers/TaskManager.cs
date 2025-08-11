using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;
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
    }

    private void InitializeTask()
    {   
        // 加载任务
        TaskData.Instance.LoadTasks();
        // 初始化
        remainingStudyTime = studyDuration * 60; // Convert to seconds
    }

    private void Start()
    {
        // InvokeRepeating("CheckTasks", 0f, 60f); // Check every minute
    }

    public StudyTask CreateNewTask(string frequency, DateTime startTime, DateTime endTime, string description)
    {
        List<StudyTask> studyTaskList = TaskData.Instance.LoadTasks();
        if (studyTaskList == null)
        {
            Debug.LogError("Failed to load TaskData");
            return null;
        }

        return new StudyTask
        {
            id = studyTaskList.Count,
            frequency = frequency,
            startTime = startTime,
            endTime = endTime,
            description = description,
            isCompleted = false
        };
    }

    public bool CheckNewTask(StudyTask studyTask)
    {
        List<StudyTask> studyTaskList = TaskData.Instance.LoadTasks();
        if (studyTaskList == null)
        {
            Debug.LogError("Failed to load TaskData");
            return false;
        }

        DateTime newTaskStart = studyTask.startTime;
        DateTime newTaskEnd = studyTask.endTime;

        foreach (var existingTask in studyTaskList)
        {
            // 跳过已完成的任务
            if (existingTask.isCompleted)
                continue;

            DateTime existingTaskStart = existingTask.startTime;
            DateTime existingTaskEnd = existingTask.endTime;

            // 检查时间是否重叠
            // 两个时间段重叠的条件是：新任务开始时间早于现有任务结束时间，
            // 且新任务结束时间晚于现有任务开始时间
            if (newTaskStart < existingTaskEnd && newTaskEnd > existingTaskStart)
            {
                return false; // 存在时间冲突
            }

            // 特殊情况：如果新任务是日常任务，还需要检查未来日期的冲突
            if (studyTask.frequency == "Daily")
            {
                // 检查未来7天内是否有冲突（可根据需要调整天数）
                for (int i = 1; i <= 7; i++)
                {
                    DateTime futureNewTaskStart = newTaskStart.AddDays(i);
                    DateTime futureNewTaskEnd = newTaskEnd.AddDays(i);
                    
                    if (futureNewTaskStart < existingTaskEnd && futureNewTaskEnd > existingTaskStart)
                    {
                        return false; // 存在时间冲突
                    }
                }
            }
            
            // 如果现有任务是日常任务，也需要检查未来日期
            if (existingTask.frequency == "Daily")
            {
                // 检查现有任务未来7天内是否与新任务冲突
                for (int i = 1; i <= 7; i++)
                {
                    DateTime futureExistingTaskStart = existingTaskStart.AddDays(i);
                    DateTime futureExistingTaskEnd = existingTaskEnd.AddDays(i);
                    
                    if (newTaskStart < futureExistingTaskEnd && newTaskEnd > futureExistingTaskStart)
                    {
                        return false; // 存在时间冲突
                    }
                }
            }
        }

        return true; // 无时间冲突
    }

    public void SaveNewTask(StudyTask studyTask)
    {
        List<StudyTask> studyTaskList = TaskData.Instance.LoadTasks();
        if (studyTaskList == null)
        {
            Debug.LogError("Failed to load TaskData");
            return;
        }

        studyTaskList.Add(studyTask);
        // TODO: 按照startTime进行排序 TaskManager.cs
        TaskData.Instance.SaveTasks();
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

        // 计算距离任务开始前5分钟的时间差（秒）
        TimeSpan timeToReminder = startTime - DateTime.Now - TimeSpan.FromMinutes(timeBeforeReminder);
        float delaySeconds = (float)timeToReminder.TotalSeconds;

        if (delaySeconds > 5 * 60)  // 大于5min
        {
            // 启动协程定时器
            reminderCoroutine = StartCoroutine(ReminderCoroutine(delaySeconds));
        }
        else
        {
            UIManager.Instance.OpenPanel(UIConst.ReminderPanel);
            string text = "任务开始时间不足5分钟，立即提醒";
            (UIManager.Instance.GetPanel(UIConst.ReminderPanel) as ReminderPanel)?.ShowReadyToTaskReminder(text);            
        }
    }
    private IEnumerator ReminderCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
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
