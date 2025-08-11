using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TaskData
{
    private static TaskData _instance;
    public List<StudyTask> studyTaskList;

    public static TaskData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TaskData();
            }
            return _instance;
        }
    }

    public void SaveTasks()
    {
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("TaskData", json);
        PlayerPrefs.Save();
    }

    public List<StudyTask> LoadTasks()
    {
        if (studyTaskList != null)
        {
            return studyTaskList;
        }
        if (PlayerPrefs.HasKey("TaskData"))
        {
            string json = PlayerPrefs.GetString("TaskData");
            TaskData taskData = JsonUtility.FromJson<TaskData>(json);
            studyTaskList = taskData.studyTaskList;
            return studyTaskList;
        }
        else
        {
            studyTaskList = new List<StudyTask>();
            return studyTaskList;
        }
    }
}

[System.Serializable]
public class StudyTask
{
    public int id;
    public string frequency;    // 一次、每天
    public DateTime startTime;
    public DateTime endTime;
    public string description;
    public bool isCompleted;
}