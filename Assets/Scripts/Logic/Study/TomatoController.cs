using UnityEngine;
using System;

public class TomatoController : MonoBehaviour
{
    // 用户配置参数
    public float focusDuration = 25f * 60f;  // 专注时长（秒）
    public float breakDuration = 5f * 60f;   // 休息时长（秒）
    public int targetCycles = 4;             // 目标周期数
    
    // 运行时状态
    private float currentTime;
    private int completedCycles;
    private bool isFocusPhase;
    private bool isActive;
    
    void Start()
    {
        ResetTimer();
    }
    
    public void ResetTimer()
    {
        currentTime = focusDuration;
        completedCycles = 0;
        isFocusPhase = true;
        isActive = false;
    }
    
    public void StartTimer()
    {
        isActive = true;
        if (isFocusPhase) ToolEventHandler.CallToolFocusStartEvent();
        else ToolEventHandler.CallToolBreakStartEvent();
    }

    public void CancelTimer()
    {
        isActive = false;
        ToolEventHandler.CallToolTimerCancelEvent();
    }
    
    public void ConfirmPhaseCompletion()
    {
        if (!isActive) return;

        if (isFocusPhase)
        {
            // 专注结束，开始休息
            isFocusPhase = false;
            currentTime = breakDuration;
            ToolEventHandler.CallToolTimerCancelEvent();
        }
        else
        {
            // 休息结束，完成一个周期
            isFocusPhase = true;
            currentTime = focusDuration;
            completedCycles++;
            ToolEventHandler.CallToolCycleCompleteEvent();

            // 开始下一个专注周期
            ToolEventHandler.CallToolFocusStartEvent();
        }
    }
    
    void Update()
    {
        if (!isActive) return;
        
        currentTime -= Time.deltaTime;
        
        if (currentTime <= 0)
        {
            // 当前阶段时间结束
            isActive = false;
        }
    }
    
    // 获取当前进度百分比 (0-1)
    public float GetProgress()
    {
        float total = isFocusPhase ? focusDuration : breakDuration;
        return 1 - (currentTime / total);
    }
    
    // 获取当前阶段剩余时间（格式化）
    public string GetCurrentTimeFormatted()
    {
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        return $"{time.Minutes:00}:{time.Seconds:00}";
    }
    
    public int GetCompletedCycles() => completedCycles;
    public bool IsFocusPhase() => isFocusPhase;
    public bool IsActive() => isActive;
}
