using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReminderPanel : BasePanel
{
    public Button confirmStartButton;
    public Button delayButton;
    public Button cancelButton;
    public Text reminderMessage;

    private TaskSystem taskSystem;
    protected override void Awake()
    {
        base.Awake();
        InitUI();
    }

    private void InitUI()
    {
        confirmStartButton.onClick.AddListener(OnConfirmStartClicked);
        delayButton.onClick.AddListener(OnDelayClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);

        taskSystem = TaskSystem.Instance;
    }

    public void ShowReadyToTaskReminder()
    {
        reminderMessage.text = "任务即将开始，请做好准备！";
        // UIManager.Instance.OpenPanel(UIConst.ReminderPanel);
    }

    public void ShowTaskEndReminder()
    {
        reminderMessage.text = "任务已完成！";
        UIManager.Instance.OpenPanel(UIConst.ReminderPanel);

        // 设置确认按钮文本和事件
        // 待处理：confirmStartButton位置未处理
        confirmStartButton.onClick.RemoveAllListeners();
        confirmStartButton.onClick.AddListener(OnTaskEndConfirmed);

        // 隐藏其他不需要的按钮
        delayButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
    }

    private void OnConfirmStartClicked()
    {
        UIManager.Instance.ClosePanel(UIConst.ReminderPanel);
        taskSystem.ConfirmTaskStart(); // 用户确认开始任务
    }

    private void OnDelayClicked()
    {
        UIManager.Instance.ClosePanel(UIConst.ReminderPanel);
        // Schedule reminder again in 5 minutes
        Invoke("ShowReadyToTaskReminder", 300f);
    }

    private void OnCancelClicked()
    {
        UIManager.Instance.ClosePanel(UIConst.ReminderPanel);
        UIManager.Instance.OpenPanel(UIConst.MainPanel);
        // 待处理：任务取消，后台会有操作
    }
    
    private void OnTaskEndConfirmed()
    {
        UIManager.Instance.ClosePanel(UIConst.ReminderPanel);
        UIManager.Instance.OpenPanel(UIConst.MainPanel);
    }
}