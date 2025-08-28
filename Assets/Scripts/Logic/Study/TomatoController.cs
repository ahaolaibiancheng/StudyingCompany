using UnityEngine;
using System;

public class TomatoController : MonoBehaviour
{
    public static TomatoController Instance;
    // 用户配置参数
    public float focusDuration = 25f * 60f;  // 专注时长（秒）
    public float breakDuration = 5f * 60f;   // 休息时长（秒）
    public int targetCycles = 4;             // 目标周期数
    
    // 运行时状态
    private float countdown;
    private int completedCycles;
    private bool isFocusPhase;
    public bool isActive;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        ResetTimer();
    }
    
    public void ResetTimer()
    {
        isActive = false;
        isFocusPhase = true;
        countdown = focusDuration;
        completedCycles = 0;
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
            countdown = breakDuration;
            ToolEventHandler.CallToolTimerCancelEvent();
        }
        else
        {
            // 休息结束，完成一个周期
            isFocusPhase = true;
            countdown = focusDuration;
            completedCycles++;
            ToolEventHandler.CallToolCycleCompleteEvent();

            // 开始下一个专注周期
            ToolEventHandler.CallToolFocusStartEvent();
        }
    }
    
    void Update()
    {
        if (!isActive) return;
        
        countdown -= Time.deltaTime;
        
        if (countdown <= 0)
        {
            // 时间结束，自动切换到下一阶段
            if (isFocusPhase)
            {
                // 专注->休息
                isFocusPhase = false;
                countdown = breakDuration;
                ToolEventHandler.CallToolBreakStartEvent(); // 通知UI休息开始
            }
            else
            {
                // 休息时间结束，完成一个周期
                isFocusPhase = true;
                countdown = focusDuration;
                completedCycles++;
                ToolEventHandler.CallToolCycleCompleteEvent();

                if (completedCycles < targetCycles)
                {
                    // 开始下一个专注周期
                    ToolEventHandler.CallToolFocusStartEvent();
                    // 保持计时器活跃状态，继续专注计时
                }
                else
                {
                    // 所有周期完成，停止计时
                    isActive = false;
                }
            }
        }
    }
    
    // 获取当前进度百分比 (0-1)
    public float GetProgressBarValue()
    {
        float total = isFocusPhase ? focusDuration : breakDuration;
        return 1 - (countdown / total);
    }
    
    // 获取当前阶段剩余时间（格式化）
    public string GetCountdownTime()
    {
        TimeSpan time = TimeSpan.FromSeconds(countdown);
        return $"{time.Minutes:00}:{time.Seconds:00}";
    }
    
    public int GetCompletedCycles() => completedCycles;
    public bool IsFocusPhase() => isFocusPhase;
}
