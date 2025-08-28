using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskPanel : BasePanel
{
    public Dropdown startTimeDropdown;
    public Dropdown endTimeDropdown;
    public Dropdown frequencyDropdown;
    public InputField taskNameInput;
    public Button saveTaskButton;
    public Button closeButton;
    public Dropdown startDateDropdown;  // 新增开始日期下拉框
    public Dropdown endDateDropdown;    // 新增结束日期下拉框
    private Text reminderMessage;

    protected override void Awake()
    {
        base.Awake();
        InitUI();
    }

    private void InitUI()
    {
        frequencyDropdown.ClearOptions();
        frequencyDropdown.AddOptions(new System.Collections.Generic.List<string> { "Once", "Daily" });

        // 初始化日期下拉框
        InitDateDropdown(startDateDropdown);
        InitDateDropdown(endDateDropdown);

        // 初始化时间下拉框
        InitTimeDropdown(startTimeDropdown);
        InitTimeDropdown(endTimeDropdown);

        saveTaskButton.onClick.AddListener(OnSaveTaskClicked);
        closeButton.onClick.AddListener(OnCloseClicked);
    }

    private void InitDateDropdown(Dropdown dropdown)
    {
        dropdown.ClearOptions();
        List<string> dateOptions = new List<string>();

        // 添加未来7天的日期选项
        for (int i = 0; i < 7; i++)
        {
            DateTime date = DateTime.Today.AddDays(i);
            string dateText = date.ToString("yyyy-MM-dd");
            dateOptions.Add(dateText);
        }
        dropdown.AddOptions(dateOptions);
    }

    private void InitTimeDropdown(Dropdown dropdown)
    {
        dropdown.ClearOptions();
        List<string> timeOptions = new List<string>();
        for (int hour = 0; hour < 24; hour++)
        {
            for (int minute = 0; minute < 60; minute += 10) // 每10分钟一个选项
            {
                timeOptions.Add($"{hour:D2}:{minute:D2}");
            }
        }
        dropdown.AddOptions(timeOptions);
    }

    private void OnSaveTaskClicked()
    {
        // 获取日期值
        string startDateText = startDateDropdown.options[startDateDropdown.value].text;
        string endDateText = endDateDropdown.options[endDateDropdown.value].text;

        // 获取时间值
        string frequency = frequencyDropdown.options[frequencyDropdown.value].text;
        string startTimeText = startTimeDropdown.options[startTimeDropdown.value].text;
        string endTimeText = endTimeDropdown.options[endTimeDropdown.value].text;
        string description = taskNameInput.text;

        // 解析完整的日期时间
        if (DateTime.TryParseExact($"{startDateText} {startTimeText}", "yyyy-MM-dd HH:mm",
            null, System.Globalization.DateTimeStyles.None, out DateTime startTime) &&
            DateTime.TryParseExact($"{endDateText} {endTimeText}", "yyyy-MM-dd HH:mm",
            null, System.Globalization.DateTimeStyles.None, out DateTime endTime))
        {
            // 检查结束时间是否晚于开始时间
            if (endTime <= startTime || endTime < DateTime.Now)
            {
                UIManager.Instance.OpenPanel(UIConst.ReminderPanel);
                string text = "结束时间不能早于开始时间或早于当前时间";
                (UIManager.Instance.GetPanel(UIConst.ReminderPanel) as ReminderPanel)?.ShowReminderMessage(text); 
                return;
            }

            UIManager.Instance.ClosePanel(UIConst.TaskPanel);
            TaskManager.Instance.StartNewTask(startTime, endTime);

            StudyTask studyTask = TaskManager.Instance.CreateNewTask(frequency, startTime, endTime, description);
            if (studyTask == null)
            {
                Debug.LogError("Failed to create new task");
                return;
            }

            bool isValid = TaskManager.Instance.CheckNewTask(studyTask);
            if (!isValid)
            {
                // TODO: reminder提醒
                Debug.Log("任务时间存在冲突！");
                return;
            }

            TaskManager.Instance.SaveNewTask(studyTask);
        }
        else
        {
            Debug.LogError("日期时间格式错误");
            // TODO: 显示错误提示给用户
        }
    }

    private void OnCloseClicked()
    {
        UIManager.Instance.ClosePanel(UIConst.TaskPanel);
    }
}
