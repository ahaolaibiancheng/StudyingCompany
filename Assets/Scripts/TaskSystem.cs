using UnityEngine;
using System;
using System.Collections.Generic;

public class TaskSystem : MonoBehaviour
{
    public static TaskSystem Instance { get; private set; }

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
    private GameManager gameManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadTasks();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
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
                gameManager.StartNewTask(task.startTime, task.endTime);
                
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

    private void LoadTasks()
    {
        if (PlayerPrefs.HasKey("StudyTasks"))
        {
            string json = PlayerPrefs.GetString("StudyTasks");
            TaskWrapper wrapper = JsonUtility.FromJson<TaskWrapper>(json);
            tasks = wrapper.tasks;
        }
    }

    // Wrapper class for JSON serialization
    [System.Serializable]
    private class TaskWrapper
    {
        public List<StudyTask> tasks;
    }
}
