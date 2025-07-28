using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    //     private void OnEnable()
    //     {
    //         // Subscribe to scene loaded event
    //         if (GameManager.Instance != null)
    //         {
    //             GameManager.Instance.OnSceneLoaded += InitializeUI;
    //         }
    //     }

    //     private void OnDisable()
    //     {
    //         // Unsubscribe from scene loaded event
    //         if (GameManager.Instance != null)
    //         {
    //             GameManager.Instance.OnSceneLoaded -= InitializeUI;
    //         }
    //     }

    //     private void InitializeUI()
    //     {
    //         // Reinitialize UI elements when scene changes
    //         Debug.Log("Initializing UI for current scene");

    //         // Add your UI initialization code here
    //         // This will be called every time a new scene is loaded
    //     }

    // public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject taskPanel;
    public GameObject backpackPanel;
    public GameObject reminderPanel;

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

    // private void Awake()
    // {
    //     if (Instance == null)
    //     {
    //         Instance = this;
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    // }

    private void Start()
    {
        gameManager = GameManager.Instance;

        // Initialize UI
        ShowMainPanel();

        // Register events
        if (gameManager != null)
        {
            gameManager.OnGameStateChanged += HandleGameStateChange;
        }

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
                // Study Panel is now handled in the Studying scene
                SceneManager.LoadScene("Studying");
                break;
            case GameManager.GameState.Resting:
                // Resting state handled in Study scene
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

//-----------------------------------------------------------------------------------------

    private static UIManager _instance;
    private Transform _uiRoot;
    // 路径配置字典
    private Dictionary<string, string> pathDict;
    // 预制件缓存字典
    private Dictionary<string, GameObject> prefabDict;
    // 已打开界面的缓存字典
    public Dictionary<string, BasePanel> panelDict;
    
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIManager();
            }
            return _instance;
        }
    }

    public Transform UIRoot
    {
        get
        {
            if (_uiRoot == null)
            {
                if (GameObject.Find("Canvas"))
                {
                    _uiRoot = GameObject.Find("Canvas").transform;
                }
                else
                {
                    _uiRoot = new GameObject("Canvas").transform;
                }
            };
            return _uiRoot;
        }
    }

    private UIManager()
    {
        InitDicts();
    }

    private void InitDicts()
    {
        prefabDict = new Dictionary<string, GameObject>();
        panelDict = new Dictionary<string, BasePanel>();

        pathDict = new Dictionary<string, string>()
        {
            {UIConst.PackagePanel, "Package/PackagePanel"},
            // {UIConst.LotteryPanel, "Lottery/LotteryPanel"},
            // {UIConst.MainPanel, "MainPanel"},
        };
    }

    public BasePanel GetPanel(string name)
    {
        BasePanel panel = null;
        // 检查是否已打开
        if (panelDict.TryGetValue(name, out panel))
        {
            return panel;
        }
        return null;
    }

    public BasePanel OpenPanel(string name)
    {
        BasePanel panel = null;
        // 检查是否已打开
        if (panelDict.TryGetValue(name, out panel))
        {
            Debug.Log("界面已打开: " + name);
            return null;
        }

        // 检查路径是否配置
        string path = "";
        if (!pathDict.TryGetValue(name, out path))
        {
            Debug.Log("界面名称错误，或未配置路径: " + name);
            return null;
        }

        // 使用缓存预制件
        GameObject panelPrefab = null;
        if (!prefabDict.TryGetValue(name, out panelPrefab))
        {
            string realPath = "Prefab/Panel/" + path;

            panelPrefab = Resources.Load<GameObject>(realPath) as GameObject;
            prefabDict.Add(name, panelPrefab);
        }

        // 打开界面
        GameObject panelObject = GameObject.Instantiate(panelPrefab, UIRoot, false);
        panel = panelObject.GetComponent<BasePanel>();
        panelDict.Add(name, panel);
        panel.OpenPanel(name);
        return panel;
    }

    public bool ClosePanel(string name)
    {
        BasePanel panel = null;
        if (!panelDict.TryGetValue(name, out panel))
        {
            Debug.Log("界面未打开: " + name);
            return false;
        }

        panel.ClosePanel();
        // panelDict.Remove(name);
        return true;
    }

}

public class UIConst
{
    // menu panels

    public const string PackagePanel = "PackagePanel";
    // public const string LotteryPanel = "LotteryPanel";
    public const string MainPanel = "MainPanel";
}

