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
    // private void OnEnable()
    // {
    //     // Subscribe to scene loaded event
    //     if (GameManager.Instance != null)
    //     {
    //         GameManager.Instance.OnSceneLoaded += InitializeUI;
    //     }
    // }

    // private void OnDisable()
    // {
    //     // Unsubscribe from scene loaded event
    //     if (GameManager.Instance != null)
    //     {
    //         GameManager.Instance.OnSceneLoaded -= InitializeUI;
    //     }
    // }

    // private void InitializeUI()
    // {
    //     // Reinitialize UI elements when scene changes
    //     Debug.Log("Initializing UI for current scene");

    //     // Add your UI initialization code here
    //     // This will be called every time a new scene is loaded
    // }
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
    private static UIManager _instance;
    private Transform _uiRoot;
    // 路径配置字典
    private Dictionary<string, string> pathDict;
    // 预制件缓存字典
    private Dictionary<string, GameObject> prefabDict;
    // 已打开界面的缓存字典
    public Dictionary<string, BasePanel> panelDict;
    private GameManager gameManager;
    
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
                
                if (_instance == null)
                {
                    GameObject uiManagerObject = new GameObject("UIManager");
                    _instance = uiManagerObject.AddComponent<UIManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            InitDicts();
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // InitDicts();    // 不能在Awake里初始化

        gameManager = GameManager.Instance;

        // Register events
        if (gameManager != null)
        {
            gameManager.OnGameStateChanged += HandleGameStateChange;
        }

    }

    public Transform UIRoot
    {
        get
        {
            if (_uiRoot == null)
            {
                if (GameObject.Find("PanelCanvas"))
                {
                    _uiRoot = GameObject.Find("PanelCanvas").transform;
                }
                else
                {
                    _uiRoot = new GameObject("PanelCanvas").transform;
                }
            }
            ;
            return _uiRoot;
        }
    }

    private void InitDicts()
    {
        prefabDict = new Dictionary<string, GameObject>();
        panelDict = new Dictionary<string, BasePanel>();

        pathDict = new Dictionary<string, string>()
        {
            {UIConst.MainPanel, "Main/MainPanel"},
            {UIConst.MenuPanel, "MenuPanel"},
            {UIConst.TaskPanel, "TaskPanel"},
            {UIConst.ReminderPanel, "ReminderPanel"},
            {UIConst.PackagePanel, "PackagePanel"},
            // {UIConst.LotteryPanel, "Lottery/LotteryPanel"},
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
            if (panelPrefab == null)
            {
                Debug.LogError($"无法加载预制体: {realPath}，请检查路径是否正确");
                return null;
            }
            prefabDict.Add(name, panelPrefab);
        }

        // 检查UIRoot是否存在
        if (UIRoot == null)
        {
            Debug.LogError("UIRoot为空，无法找到PanelCanvas对象");
            return null;
        }

        // 打开界面，挂载在UIRoot下
        GameObject panelObject = GameObject.Instantiate(panelPrefab, UIRoot, false);
        if (panelObject == null)
        {
            Debug.LogError($"实例化预制体失败: {name}");
            return null;
        }
        
        panel = panelObject.GetComponent<BasePanel>();
        if (panel == null)
        {
            Debug.LogError($"预制体 {name} 缺少 BasePanel 组件");
            Destroy(panelObject); // 销毁已创建的对象
            return null;
        }
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
        return true;
    }

}

public class UIConst
{
    public const string MainPanel = "MainPanel";
    public const string MenuPanel = "MenuPanel";
    public const string TaskPanel = "TaskPanel";
    public const string ReminderPanel = "ReminderPanel";
    public const string PackagePanel = "PackagePanel";
    // public const string LotteryPanel = "LotteryPanel";
}

