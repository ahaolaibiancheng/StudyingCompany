using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TomatoUI : MonoBehaviour
{
    [Header("配置界面")]
    public GameObject configPanel;
    public InputField focusInput;
    public InputField breakInput;
    public InputField cyclesInput;
    public Button startButton;
    public Button returnButton;

    [Header("计时界面")]
    public GameObject timerPanel;
    public Slider progressBar;
    public Text timeText;
    public Text cycleText;
    public Text phaseText;
    public Button confirmButton;
    public Button cancelButton;

    private TomatoController timerController;

    private void OnEnable()
    {
        ToolEventHandler.ToolFocusStartEvent += () => UpdatePhaseDisplay("专注中");
        ToolEventHandler.ToolBreakStartEvent += () => UpdatePhaseDisplay("休息中");
        ToolEventHandler.ToolCycleCompleteEvent += UpdateCycleDisplay;
        ToolEventHandler.ToolTimerCancelEvent += OnToolTimerCancelEvent;
    }
    private void OnDisable()
    {
        ToolEventHandler.ToolFocusStartEvent -= () => UpdatePhaseDisplay("专注中");
        ToolEventHandler.ToolBreakStartEvent -= () => UpdatePhaseDisplay("休息中");
        ToolEventHandler.ToolCycleCompleteEvent -= UpdateCycleDisplay;
        ToolEventHandler.ToolTimerCancelEvent -= OnToolTimerCancelEvent;
    }

    void Start()
    {
        timerController = GetComponent<TomatoController>();
        if (timerController == null)
        {
            timerController = gameObject.AddComponent<TomatoController>();
        }

        // 初始设置
        focusInput.text = "25";
        breakInput.text = "5";
        cyclesInput.text = "4";

        // 绑定事件
        startButton.onClick.AddListener(StartTimer);
        returnButton.onClick.AddListener(EndTimer);
        confirmButton.onClick.AddListener(ConfirmPhase);
        cancelButton.onClick.AddListener(CancelTimer);

        // 初始UI状态
        configPanel.SetActive(false);
        timerPanel.SetActive(false);
    }

    void Update()
    {
        if (timerController.IsActive())
        {
            progressBar.value = timerController.GetProgress();
            timeText.text = timerController.GetCurrentTimeFormatted();

            // 自动检测时间结束
            if (!timerController.IsActive())
            {
                confirmButton.gameObject.SetActive(true);
            }
        }
    }

    private void StartTimer()
    {
        // 应用用户配置
        timerController.focusDuration = float.Parse(focusInput.text) * 60;
        timerController.breakDuration = float.Parse(breakInput.text) * 60;
        timerController.targetCycles = int.Parse(cyclesInput.text);

        timerController.ResetTimer();
        timerController.StartTimer();

        // 切换UI
        configPanel.SetActive(false);
        timerPanel.SetActive(true);
        confirmButton.gameObject.SetActive(false);

        UpdateCycleDisplay();
    }

    private void EndTimer()
    {
        configPanel.SetActive(false);
    }

    private void ConfirmPhase()
    {
        timerController.ConfirmPhaseCompletion();
        confirmButton.gameObject.SetActive(false);

        // 如果计时器激活（进入新阶段），继续计时
        if (timerController.IsActive())
        {
            timerController.StartTimer();
        }
    }

    private void CancelTimer()
    {
        timerController.CancelTimer();
    }

    public void OnToolTimerCancelEvent()
    {
        configPanel.SetActive(true);
        timerPanel.SetActive(false);
    }

    private void UpdatePhaseDisplay(string phase)
    {
        phaseText.text = phase;
    }

    private void UpdateCycleDisplay()
    {
        cycleText.text = $"{timerController.GetCompletedCycles()}/{timerController.targetCycles}";
    }
}
