using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskPanel : BasePanel
{
    public InputField startTimeInput;
    public InputField endTimeInput;
    public Dropdown frequencyDropdown;
    public InputField taskNameInput;
    public Button saveTaskButton;
    public Button closeButton;

    private Text reminderMessage;
    private GameManager gameManager;
    private TaskSystem taskSystem;

    protected override void Awake()
    {
        base.Awake();
        InitUI();
    }

    private void InitUI()
    {
        gameManager = GameManager.Instance;
        taskSystem = TaskSystem.Instance;

        frequencyDropdown.ClearOptions();
        frequencyDropdown.AddOptions(new System.Collections.Generic.List<string> { "Once", "Daily" });
        saveTaskButton.onClick.AddListener(OnSaveTaskClicked);
        closeButton.onClick.AddListener(OnCloseClicked);
    }

    private void OnSaveTaskClicked()
    {
        // Parse task details with exact format validation
        DateTime startTime, endTime;
        string format = "HH:mm"; // 24-hour format like "14:30"
        bool isValidStart = DateTime.TryParseExact(startTimeInput.text, format, null, System.Globalization.DateTimeStyles.None, out startTime);
        bool isValidEnd = DateTime.TryParseExact(endTimeInput.text, format, null, System.Globalization.DateTimeStyles.None, out endTime);

        if (!isValidStart || !isValidEnd)
        {
            // Show detailed error message
            reminderMessage.text = "Invalid time format! Please use 24-hour format (e.g. 14:30).";
            UIManager.Instance.OpenPanel(UIConst.ReminderPanel);
            return;
        }

        bool isDaily = frequencyDropdown.value == 1; // 0=Once, 1=Daily

        taskSystem.StartNewTask(startTime, endTime);
    }

    private void OnCloseClicked()
    {
        UIManager.Instance.ClosePanel(UIConst.TaskPanel);
        UIManager.Instance.OpenPanel(UIConst.MainPanel);
    }
}