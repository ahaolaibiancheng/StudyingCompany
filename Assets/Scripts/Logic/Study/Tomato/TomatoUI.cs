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
    public Text timeText;   // 倒计时
    public Text cycleText;  // 处于哪个循环中
    public Text phaseText;  // 处于什么阶段
    public Button continueButton;
    public Button stopButton;
    public Button cancelButton;

    private TomatoController timerController;

    private void OnEnable()
    {
        ToolEventHandler.ToolFocusStartEvent += () => UpdatePhaseDisplay("专注中");
        ToolEventHandler.ToolBreakStartEvent += () => UpdatePhaseDisplay("休息中");
        ToolEventHandler.ToolCycleCompleteEvent += UpdateCycleTextDisplay;
        ToolEventHandler.ToolTimerCancelEvent += OnToolTimerCancelEvent;
    }
    private void OnDisable()
    {
        ToolEventHandler.ToolFocusStartEvent -= () => UpdatePhaseDisplay("专注中");
        ToolEventHandler.ToolBreakStartEvent -= () => UpdatePhaseDisplay("休息中");
        ToolEventHandler.ToolCycleCompleteEvent -= UpdateCycleTextDisplay;
        ToolEventHandler.ToolTimerCancelEvent -= OnToolTimerCancelEvent;
    }

    void Start()
    {
        timerController = GetComponent<TomatoController>();
        if (timerController == null)
        {
            timerController = gameObject.AddComponent<TomatoController>();
        }

        // 配置界面按钮绑定事件
        startButton.onClick.AddListener(OnStartButtonClicked);
        returnButton.onClick.AddListener(OnReturnButtonClicked);
        // 计时界面按钮绑定事件
        continueButton.onClick.AddListener(OnContinueButtonClicked);
        stopButton.onClick.AddListener(OnStopButtonClicked);
        cancelButton.onClick.AddListener(OnCancelButtonClicked);

        // 初始UI状态
        focusInput.text = "25";
        breakInput.text = "5";
        cyclesInput.text = "4";
        configPanel.SetActive(false);
        timerPanel.SetActive(false);
    }

    void Update()
    {
        if (timerController.isActive == false) return;
        
        progressBar.value = timerController.GetProgressBarValue();
        timeText.text = timerController.GetCountdownTime();
    }

    private void OnStartButtonClicked()
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
        continueButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(true);

        UpdateCycleTextDisplay();
    }

    private void OnReturnButtonClicked()
    {
        configPanel.SetActive(false);
    }

    private void OnContinueButtonClicked()
    {
        timerController.isActive = true;
        continueButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(true);
    }

    private void OnStopButtonClicked()
    {
        timerController.isActive = false;
        continueButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(false);
    }

    private void OnCancelButtonClicked()
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

    private void UpdateCycleTextDisplay()
    {
        cycleText.text = $"{timerController.GetCompletedCycles()}/{timerController.targetCycles}";
    }
}
