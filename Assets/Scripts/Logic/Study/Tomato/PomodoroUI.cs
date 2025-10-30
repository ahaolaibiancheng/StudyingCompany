using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PomodoroUI : MonoBehaviour
{
    [Header("模式切换")]
    public Button pomodoroButton;
    public Button stopwatchButton;
    public Button countdownButton;

    [Header("计时显示")]
    public TextMeshProUGUI timerText;
    public Slider progressBar;

    [Header("控制按钮")]
    public Button startButton;
    public Button resetButton;

    [Header("番茄钟参数输入")]
    public TMP_InputField workInput;
    public TMP_InputField workInputCD;  // 倒计时长（分钟）
    public TMP_InputField shortBreakInput;
    public TMP_InputField longBreakInput;
    public TMP_InputField cycleInput;

    [Header("状态显示")]
    public TextMeshProUGUI phaseText;
    public TextMeshProUGUI cycleText;
    [Header("各个组件")]
    public GameObject state;
    public GameObject slider;
    public GameObject config;
    public GameObject configCD;

    private PomodoroController controller;

    void Start()
    {
        controller = GetComponent<PomodoroController>();
        if (controller == null)
        {
            Debug.LogError("PomodoroController 未绑定到同一对象上！");
            return;
        }

        // 绑定按钮事件
        pomodoroButton.onClick.AddListener(OnpomodoroButtonClicked);
        stopwatchButton.onClick.AddListener(OnstopwatchButtonClicked);
        countdownButton.onClick.AddListener(OnCountdownButtonClicked);

        startButton.onClick.AddListener(() => controller.ToggleRun());
        resetButton.onClick.AddListener(() => controller.ResetTimer());

        // 输入框变化绑定
        workInput.onEndEdit.AddListener(v => controller.SetWorkDuration(float.Parse(v) * 60));
        workInputCD.onEndEdit.AddListener(v => controller.SetWorkDuration(float.Parse(v) * 60));
        shortBreakInput.onEndEdit.AddListener(v => controller.SetShortBreakDuration(float.Parse(v) * 60));
        longBreakInput.onEndEdit.AddListener(v => controller.SetLongBreakDuration(float.Parse(v) * 60));
        cycleInput.onEndEdit.AddListener(v => controller.SetCycleCount(int.Parse(v)));
    }

    void Update()
    {
        if (controller == null) return;

        timerText.text = controller.GetFormattedTime();
        progressBar.value = controller.GetProgress();
        phaseText.text = $"阶段：{controller.GetPhaseName()}";
        cycleText.text = $"剩余周期：{controller.Cycles}";
    }

    private void OnpomodoroButtonClicked()
    {
        state.SetActive(true);
        slider.SetActive(true);
        config.SetActive(true);
        configCD.SetActive(false);
        controller.SetAppMode(AppMode.Pomodoro);
    }

    private void OnstopwatchButtonClicked()
    {
        state.SetActive(false);
        slider.SetActive(false);
        config.SetActive(false);
        configCD.SetActive(false);
        controller.SetAppMode(AppMode.Stopwatch);
    }

    private void OnCountdownButtonClicked()
    {
        state.SetActive(false);
        slider.SetActive(false);
        config.SetActive(false);
        configCD.SetActive(true);
        controller.SetAppMode(AppMode.Countdown);
    }

}