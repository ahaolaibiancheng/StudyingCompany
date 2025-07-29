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

    private GameManager gameManager;
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
    }

    public void ShowReadyToTaskReminder()
    {
        UIManager.Instance.OpenPanel(UIConst.ReminderPanel);
        reminderMessage.text = "It's time to start your study!";
    }

    public void ShowTaskEndReminder()
    {
        UIManager.Instance.OpenPanel(UIConst.ReminderPanel);
        reminderMessage.text = "任务已完成！";

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
        gameManager.ConfirmTaskStart(); // 用户确认开始任务
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
        // 待处理：任务取消，后台会有操作
    }
    
    private void OnTaskEndConfirmed()
    {
        UIManager.Instance.ClosePanel(UIConst.ReminderPanel);
        UIManager.Instance.OpenPanel(UIConst.MainPanel);
    }
}