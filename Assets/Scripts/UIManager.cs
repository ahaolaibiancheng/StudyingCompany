using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject taskPanel;
    public GameObject backpackPanel;
    public GameObject studyPanel;
    public GameObject reminderPanel;

    [Header("Study Panel Elements")]
    public Text sessionTimeText;
    public Button pauseButton;
    public Button continueButton;

    [Header("Reminder Panel Elements")]
    public Text reminderMessage;
    public Button confirmStartButton;
    public Button delayButton;
    public Button cancelButton;

    [Header("Task Panel Elements")]
    public InputField taskNameInput;
    public InputField startTimeInput;
    public InputField endTimeInput;
    public Dropdown frequencyDropdown;
    public Button saveTaskButton;
    
    // Volume control elements
    public Slider volumeSlider;
    public Text volumeText;

    private GameManager gameManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        
        // Initialize UI
        ShowMainPanel();
        
        // Register events
        if (gameManager != null)
        {
            gameManager.OnGameStateChanged += HandleGameStateChange;
            gameManager.OnStudyTimeUpdated += UpdateTimerDisplay;
        }

        // Set up button listeners
        // StudyPanel界面
        pauseButton.onClick.AddListener(OnPauseButtonClicked);
        continueButton.onClick.AddListener(OnContinueButtonClicked);
        // ReminderPanel界面
        confirmStartButton.onClick.AddListener(OnConfirmStartClicked);
        delayButton.onClick.AddListener(OnDelayClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);

        saveTaskButton.onClick.AddListener(OnSaveTaskClicked);
        
        // Initialize dropdown options
        frequencyDropdown.ClearOptions();
        frequencyDropdown.AddOptions(new System.Collections.Generic.List<string> { "Once", "Daily" });
        
        // Initialize volume settings
        LoadVolumeSettings();
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            UpdateVolumeText(volumeSlider.value);
        }
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnGameStateChanged -= HandleGameStateChange;
            gameManager.OnStudyTimeUpdated -= UpdateTimerDisplay;
        }
    }
    
    private void Update()
    {
        // 实时更新会话时间显示
        if (studyPanel.activeSelf)
        {
            UpdateTimerDisplay(gameManager.RemainingStudyTime);
        }
    }

    private void HandleGameStateChange(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.Idle:
                ShowMainPanel();
                break;
            case GameManager.GameState.Studying:
                ShowStudyPanel();
                break;
            case GameManager.GameState.Resting:
                // Keep study panel visible during rest
                break;
        }
    }

    // Panel management
    public void ShowMainPanel()
    {
        HideAllPanels();
        mainPanel.SetActive(true);
    }

    public void ShowSettingsPanel()
    {
        HideAllPanels();
        settingsPanel.SetActive(true);
        
        // 确保音量控件可见
        if (volumeSlider != null) volumeSlider.gameObject.SetActive(true);
        if (volumeText != null) volumeText.gameObject.SetActive(true);
    }

    public void ShowTaskPanel()
    {
        HideAllPanels();
        taskPanel.SetActive(true);
    }

    public void ShowBackpackPanel()
    {
        HideAllPanels();
        backpackPanel.SetActive(true);
    }

    public void ShowStudyPanel()
    {
        HideAllPanels();
        studyPanel.SetActive(true);
        UpdateTimerDisplay(gameManager.RemainingStudyTime); // 确保面板显示时立即更新
    }

    public void ShowReadyToTaskReminder()
    {
        reminderPanel.SetActive(true);
        reminderMessage.text = "It's time to start your study!";
    }
    
    public void ShowTaskEndReminder()
    {
        reminderPanel.SetActive(true);
        reminderMessage.text = "任务已完成！";
        
        // 设置确认按钮文本和事件
        // 待处理：confirmStartButton位置未处理
        confirmStartButton.onClick.RemoveAllListeners();
        confirmStartButton.onClick.AddListener(OnTaskEndConfirmed);
        
        // 隐藏其他不需要的按钮
        delayButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
    }
    
    private void HideAllPanels()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(false);
        taskPanel.SetActive(false);
        backpackPanel.SetActive(false);
        studyPanel.SetActive(false);
        reminderPanel.SetActive(false);

        // 隐藏音量控件
        if (volumeSlider != null) volumeSlider.gameObject.SetActive(false);
        if (volumeText != null) volumeText.gameObject.SetActive(false);
    }

    // Button click handlers
    public void OnSettingsButtonClicked()
    {
        ShowSettingsPanel();
    }

    public void OnTaskButtonClicked()
    {
        ShowTaskPanel();
    }

    public void OnBackpackButtonClicked()
    {
        ShowBackpackPanel();
    }

    public void OnPauseButtonClicked()
    {
        if (CharacterController.Instance != null)
        {
            CharacterController.Instance.OnPauseButton();
            pauseButton.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("CharacterController instance is null in OnPauseButtonClicked");
        }
    }

    public void OnContinueButtonClicked()
    {
        CharacterController.Instance.OnContinueButton();
        continueButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
    }

    private void OnConfirmStartClicked()
    {
        reminderPanel.SetActive(false);
        gameManager.ConfirmTaskStart(); // 用户确认开始任务
    }
    
    private void OnTaskEndConfirmed()
    {
        reminderPanel.SetActive(false);
        ShowMainPanel();
    }

    private void OnDelayClicked()
    {
        reminderPanel.SetActive(false);
        // Schedule reminder again in 5 minutes
        Invoke("ShowReadyToTaskReminder", 300f);
    }

    private void OnCancelClicked()
    {
        reminderPanel.SetActive(false);
        // 待处理：任务取消，后台会有操作
    }
    
    // Volume control methods
    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        UpdateVolumeText(value);
        SaveVolumeSettings();
    }
    
    private void UpdateVolumeText(float value)
    {
        if (volumeText != null)
        {
            volumeText.text = $"Volume: {Mathf.RoundToInt(value * 100)}%";
        }
    }
    
    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("SystemVolume", volumeSlider.value);
        PlayerPrefs.Save();
    }
    
    private void LoadVolumeSettings()
    {
        if (PlayerPrefs.HasKey("SystemVolume"))
        {
            float volume = PlayerPrefs.GetFloat("SystemVolume");
            AudioListener.volume = volume;
            if (volumeSlider != null)
            {
                volumeSlider.value = volume;
            }
        }
        else
        {
            // Default volume
            AudioListener.volume = 0.8f;
            if (volumeSlider != null)
            {
                volumeSlider.value = 0.8f;
            }
        }
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
            reminderPanel.SetActive(true);
            reminderMessage.text = "Invalid time format! Please use 24-hour format (e.g. 14:30).";
            return;
        }
        
        bool isDaily = frequencyDropdown.value == 1; // 0=Once, 1=Daily
        
        // Show confirmation and return to main
        ShowMainPanel();
        
        // Schedule task
        gameManager.StartNewTask(startTime, endTime);
    }

    // Update display methods
    private void UpdateTimerDisplay(float remainingTime)
    {
        if (studyPanel.activeSelf)
        {
            // 计算总分钟数
            int totalMinutes = (int)gameManager.CurrentSessionTime / 60;
            // Debug.Log($"CurrentSessionTime second: {gameManager.CurrentSessionTime}");
            sessionTimeText.text = $"Session: {totalMinutes} min";
        }
    }
}
