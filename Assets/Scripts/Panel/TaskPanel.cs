using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskPanel : BasePanel
{
    public InputField taskNameInput;
    public InputField startTimeInput;
    public InputField endTimeInput;
    public Dropdown frequencyDropdown;
    public Button saveTaskButton;
    private Text reminderMessage;
    protected override void Awake()
    {
        base.Awake();
        InitUI();
    }

    private void InitUI()
    {
        saveTaskButton.onClick.AddListener(OnSaveTaskClicked);
        frequencyDropdown.ClearOptions();
        frequencyDropdown.AddOptions(new System.Collections.Generic.List<string> { "Once", "Daily" });
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

        // Show confirmation and return to main
        UIManager.Instance.OpenPanel(UIConst.MainPanel);

        // Schedule task
        GameManager.Instance.StartNewTask(startTime, endTime);
    }
}