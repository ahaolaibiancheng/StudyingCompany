using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TodoListUI : MonoBehaviour
{
    public static TodoListUI Instance;
    
    [Header("Tasks Container")]
    public Transform tasksContainer;

    [Header("Add Task UI")]
    public GameObject addTaskPanel;
    public GameObject addUpPanel;
    public GameObject hisUpPanel;
    public TMP_InputField taskContentInput;
    public TMP_InputField dueDateInput;
    public Toggle importantToggle;
    public Toggle urgentToggle;
    public Dropdown typeDropdown; // 任务类型下拉菜单
    public Button submitButton;
    public Button cancelButton;

    [Header("Task Item Prefab")]
    public GameObject taskItemPrefab;

    [Header("Category Buttons")]
    public Button importantUrgentButton;
    public Button notImportantUrgentButton;
    public Button importantNotUrgentButton;
    public Button notImportantNotUrgentButton;

    [Header("Type Category Buttons")]
    public Button workStudyButton;
    public Button personalHealthButton;
    public Button familyLifeButton;
    public Button socialRelationsButton;
    public Button personalGrowthButton;
    public Button financialManagementButton;
    
    [Header("Active Category Text")]
    public Text activeCategoryText1;
    public Text activeCategoryText2;

    [Header("Score Display")]
    public Text scoreText;
    
    [Header("Add Task Button")]
    public Button addTaskButton;
    public Button taskButton;
    public Button historyButton;
    public Button closeButton;

    [Header("Delete Area")]
    public RectTransform deleteArea;
    public Image deleteAreaImage;
    public Text deleteAreaText;

    private TodoListController todoController;
    private ScoreManager scoreManager;

    // 当前显示的状态
    private string currentCategoryName;
    private List<TodoItem> currentList;
    private bool currentIsCompletedList;
    private int currentScore;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        todoController = GetComponent<TodoListController>();
        if (todoController == null)
            todoController = gameObject.AddComponent<TodoListController>();
        scoreManager = GetComponent<ScoreManager>();
        if (scoreManager == null)
            scoreManager = gameObject.AddComponent<ScoreManager>();
        
        submitButton.onClick.AddListener(AddNewTask);
        cancelButton.onClick.AddListener(CancelNewTask);
        addTaskButton.onClick.AddListener(ShowAddTaskPanel);
        taskButton.onClick.AddListener(ShowTaskPanel);
        historyButton.onClick.AddListener(ShowHistoryPanel);
        closeButton.onClick.AddListener(CloseAddTaskPanel);
        
        // 绑定分类按钮事件 - 使用共享容器
        importantUrgentButton.onClick.AddListener(() => ShowList("重要且紧急", todoController.importantUrgent, false));
        notImportantUrgentButton.onClick.AddListener(() => ShowList("不重要且紧急", todoController.notImportantUrgent, false));
        importantNotUrgentButton.onClick.AddListener(() => ShowList("重要且不紧急", todoController.importantNotUrgent, false));
        notImportantNotUrgentButton.onClick.AddListener(() => ShowList("不重要且不紧急", todoController.notImportantNotUrgent, false));
        
        // 绑定类型分类按钮事件
        if (workStudyButton != null) workStudyButton.onClick.AddListener(() => ShowList("工作/学业", todoController.workStudyList, true, scoreManager.workStudyScore));
        if (personalHealthButton != null) personalHealthButton.onClick.AddListener(() => ShowList("个人健康", todoController.personalHealthList, true, scoreManager.personalHealthScore));
        if (familyLifeButton != null) familyLifeButton.onClick.AddListener(() => ShowList("家庭生活", todoController.familyLifeList, true, scoreManager.familyLifeScore));
        if (socialRelationsButton != null) socialRelationsButton.onClick.AddListener(() => ShowList("社交人际", todoController.socialRelationsList, true, scoreManager.socialRelationsScore));
        if (personalGrowthButton != null) personalGrowthButton.onClick.AddListener(() => ShowList("个人成长/兴趣", todoController.personalGrowthList, true, scoreManager.personalGrowthScore));
        if (financialManagementButton != null) financialManagementButton.onClick.AddListener(() => ShowList("财务管理", todoController.financialManagementList, true, scoreManager.financialManagementScore));
        
        // 默认显示重要且紧急分类
        ShowList("重要且紧急", todoController.importantUrgent, false);
        addTaskPanel.SetActive(false);

        // 初始化删除区域 - 默认隐藏
        if (deleteArea != null)
        {
            deleteArea.gameObject.SetActive(false);
            if (deleteAreaImage != null)
            {
                deleteAreaImage.color = new Color(1f, 0.5f, 0.5f, 0.3f); // 半透明红色背景
            }
            if (deleteAreaText != null)
            {
                deleteAreaText.text = "拖拽至此删除";
            }
        }

        // 初始化任务类型下拉菜单
        if (typeDropdown != null)
        {
            typeDropdown.ClearOptions();
            List<string> options = new List<string>
            {
                "工作/学业",
                "个人健康",
                "家庭生活",
                "社交人际",
                "个人成长/兴趣",
                "财务管理"
            };
            typeDropdown.AddOptions(options);
            typeDropdown.value = 0; // 默认选择第一个选项
        }

        LoadTodoItems();
    }

    public void ShowTaskPanel()
    {
        taskButton.gameObject.SetActive(false);
        historyButton.gameObject.SetActive(true);
        addUpPanel.SetActive(true);
        hisUpPanel.SetActive(false);
        ShowList("重要且紧急", todoController.importantUrgent, false);
    }
    public void ShowHistoryPanel()
    {
        taskButton.gameObject.SetActive(true);
        historyButton.gameObject.SetActive(false);
        addUpPanel.SetActive(false);
        hisUpPanel.SetActive(true);
        ShowList("工作/学业", todoController.workStudyList, true, scoreManager.workStudyScore);
    }

    public void ShowAddTaskPanel()
    {
        addTaskPanel.SetActive(true);
    }

    public void CloseAddTaskPanel()
    {
        // 需要同时删除3个panel
        Transform grandpa = closeButton.transform.parent.parent;
        grandpa.gameObject.SetActive(false);
    }

    private void AddNewTask()
    {
        if (string.IsNullOrEmpty(taskContentInput.text))
            return;

        // 获取选中的任务类型
        string selectedType = "工作/学业"; // 默认值
        if (typeDropdown != null && typeDropdown.options.Count > 0)
        {
            selectedType = typeDropdown.options[typeDropdown.value].text;
        }

        TodoItem newItem = new TodoItem
        {
            content = taskContentInput.text,
            isImportant = importantToggle.isOn,
            isUrgent = urgentToggle.isOn,
            dueDate = ParseDueDate(dueDateInput.text),
            isCompleted = false,
            type = selectedType
        };

        todoController.AddTodoItem(newItem);
        AddTaskToUI(newItem);

        // 重置表单
        taskContentInput.text = "";
        importantToggle.isOn = false;
        urgentToggle.isOn = false;
        dueDateInput.text = "";
        if (typeDropdown != null)
        {
            typeDropdown.value = 0; // 重置为默认选项
        }
        addTaskPanel.SetActive(false);
    }

    private void CancelNewTask()
    {
        addTaskPanel.SetActive(false);
    }

    private DateTime ParseDueDate(string dateString)
    {
        if (DateTime.TryParse(dateString, out DateTime result))
            return result;
        return DateTime.Now.AddDays(7); // 默认7天后
    }

    private void LoadTodoItems()
    {
        // 清空共享容器
        ClearContainer(tasksContainer);
        
        // 添加当前列表的任务到共享容器
        if (currentList != null)
        {
            AddItemsToContainer(currentList, tasksContainer);
        }
    }

    private void AddItemsToContainer(List<TodoItem> items, Transform container)
    {
        foreach (TodoItem item in items)
        {
            AddTaskToUI(item, container);
        }
    }

    private void AddTaskToUI(TodoItem item, Transform parent = null)
    {
        // Always use the shared tasks container
        if (parent == null)
        {
            parent = tasksContainer;
        }

        GameObject taskItem = Instantiate(taskItemPrefab, parent);
        TaskItemUI itemUI = taskItem.GetComponent<TaskItemUI>();
        itemUI.Initialize(item, OnTaskCompleted, OnTaskMoved, OnTaskDeleted, deleteArea);
    }


    private void ClearContainer(Transform container)
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);
    }
    
    private void ShowList(string categoryName, List<TodoItem> items, bool isCompletedList, int score = 0)
    {
        // 保存当前显示的状态
        currentCategoryName = categoryName;
        currentList = items;
        currentIsCompletedList = isCompletedList;
        currentScore = score;

        // 清空共享容器
        ClearContainer(tasksContainer);
        
        // 添加任务到共享容器
        AddItemsToContainer(items, tasksContainer);
        
        // 更新当前激活分类文本
        activeCategoryText1.text = $"{categoryName}";
        activeCategoryText2.text = $"{categoryName}";
        
        // 显示或隐藏分数
        if (scoreText != null)
        {
            if (isCompletedList)
            {
                scoreText.gameObject.SetActive(true);
                scoreText.text = $"分数: {score}";
            }
            else
            {
                scoreText.gameObject.SetActive(false);
            }
        }
    }

    private void OnTaskCompleted(TodoItem item)
    {
        // 调用CompleteTodoItem来移动任务到类型列表并计算分数
        todoController.CompleteTodoItem(item);
        // 重新显示当前列表以反映变化
        ShowList(currentCategoryName, currentList, currentIsCompletedList, currentScore);
    }

    private void OnTaskMoved(TodoItem item, Transform newContainer)
    {
        todoController.SaveTodoList();
        
        // 重新加载UI保持顺序
        LoadTodoItems();
    }

    private void OnTaskDeleted(TodoItem item)
    {
        // 从控制器中移除任务
        RemoveItemFromController(item);
        
        // 重新计算所有类型分数（处理从类型列表中删除的情况）
        todoController.UpdateTypeScores();
        
        // 如果当前显示的是已完成列表，更新当前分数
        if (currentIsCompletedList)
        {
            // 根据当前类别名称获取最新的分数
            currentScore = GetScoreForCategory(currentCategoryName);
        }
        
        // 重新显示当前列表以反映变化
        ShowList(currentCategoryName, currentList, currentIsCompletedList, currentScore);
    }

    // 根据类别名称获取对应的分数
    private int GetScoreForCategory(string categoryName)
    {
        switch (categoryName)
        {
            case "工作/学业":
                return scoreManager.workStudyScore;
            case "个人健康":
                return scoreManager.personalHealthScore;
            case "家庭生活":
                return scoreManager.familyLifeScore;
            case "社交人际":
                return scoreManager.socialRelationsScore;
            case "个人成长/兴趣":
                return scoreManager.personalGrowthScore;
            case "财务管理":
                return scoreManager.financialManagementScore;
            default:
                return 0;
        }
    }

    private void RemoveItemFromController(TodoItem item)
    {
        // 从所有10个列表中移除任务
        todoController.importantUrgent.Remove(item);
        todoController.notImportantUrgent.Remove(item);
        todoController.importantNotUrgent.Remove(item);
        todoController.notImportantNotUrgent.Remove(item);
        
        todoController.workStudyList.Remove(item);
        todoController.personalHealthList.Remove(item);
        todoController.familyLifeList.Remove(item);
        todoController.socialRelationsList.Remove(item);
        todoController.personalGrowthList.Remove(item);
        todoController.financialManagementList.Remove(item);
    }

    public void ShowDeleteArea()
    {
        deleteArea.gameObject.SetActive(true);
        deleteAreaText.text = "拖拽至此删除";
    }

    public void HideDeleteArea()
    {
        deleteArea.gameObject.SetActive(false);
    }
}
