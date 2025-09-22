using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ShowPanelUI : MonoBehaviour
{
    [Header("Category Buttons")]
    public UnityEngine.UI.Button importantUrgentButton;
    public UnityEngine.UI.Button notImportantUrgentButton;
    public UnityEngine.UI.Button importantNotUrgentButton;
    public UnityEngine.UI.Button notImportantNotUrgentButton;
    [Header("Type Category Buttons")]
    public UnityEngine.UI.Button workStudyButton;
    public UnityEngine.UI.Button personalHealthButton;
    public UnityEngine.UI.Button familyLifeButton;
    public UnityEngine.UI.Button socialRelationsButton;
    public UnityEngine.UI.Button personalGrowthButton;
    public UnityEngine.UI.Button financialManagementButton;
    [Header("Other UI")]
    public TextMeshProUGUI type;
    public Transform tasksContainer;
    public UnityEngine.UI.Button addTaskButton;
    public UnityEngine.UI.Button doingButton;
    public UnityEngine.UI.Button historyButton;

    [Header("Show Item Prefab")]
    public GameObject todoListItemPrefab;
    [Header("Panel UI")]
    public GameObject homeTypePanel;
    public GameObject hisTypePanel;
    public GameObject editPanel;

    // 当前显示的状态    
    private string currentCategoryName;
    private List<TodoItem> currentList;
    private bool currentIsCompletedList;
    private int currentScore;

    private TodoListController todoController;
    private ScoreManager scoreManager;

    private void Awake()
    {
        todoController = GetComponentInParent<TodoListController>();
        scoreManager = ScoreManager.Instance;

        // 绑定分类按钮事件 - 使用共享容器
        importantUrgentButton.onClick.AddListener(() => ShowList("重要且紧急", todoController.todoListDataSO.importantUrgent, false));
        notImportantUrgentButton.onClick.AddListener(() => ShowList("不重要且紧急", todoController.todoListDataSO.notImportantUrgent, false));
        importantNotUrgentButton.onClick.AddListener(() => ShowList("重要且不紧急", todoController.todoListDataSO.importantNotUrgent, false));
        notImportantNotUrgentButton.onClick.AddListener(() => ShowList("不重要且不紧急", todoController.todoListDataSO.notImportantNotUrgent, false));

        // 绑定类型分类按钮事件
        workStudyButton.onClick.AddListener(() => ShowList("工作/学业", todoController.todoListDataSO.workStudyList, true, scoreManager.scoreData.workStudyScore));
        personalHealthButton.onClick.AddListener(() => ShowList("个人健康", todoController.todoListDataSO.personalHealthList, true, scoreManager.scoreData.personalHealthScore));
        familyLifeButton.onClick.AddListener(() => ShowList("家庭生活", todoController.todoListDataSO.familyLifeList, true, scoreManager.scoreData.familyLifeScore));
        socialRelationsButton.onClick.AddListener(() => ShowList("社交人际", todoController.todoListDataSO.socialRelationsList, true, scoreManager.scoreData.socialRelationsScore));
        personalGrowthButton.onClick.AddListener(() => ShowList("个人成长/兴趣", todoController.todoListDataSO.personalGrowthList, true, scoreManager.scoreData.personalGrowthScore));
        financialManagementButton.onClick.AddListener(() => ShowList("财务管理", todoController.todoListDataSO.financialManagementList, true, scoreManager.scoreData.financialManagementScore));

        doingButton.onClick.AddListener(ShowTaskPanel);
        historyButton.onClick.AddListener(ShowHistoryPanel);
        addTaskButton.onClick.AddListener(ShowAddTaskPanel);
    }

    void OnEnable()
    {
        TodoListEventHandler.TodoItemCompleted += OnTaskCompleted;
    }

    void OnDisable()
    {
        TodoListEventHandler.TodoItemCompleted -= OnTaskCompleted;
    }

    public void ShowTaskPanel()
    {
        doingButton.gameObject.SetActive(false);
        historyButton.gameObject.SetActive(true);
        homeTypePanel.SetActive(true);
        hisTypePanel.SetActive(false);
        ShowList("重要且紧急", todoController.todoListDataSO.importantUrgent, false);
    }
    public void ShowHistoryPanel()
    {
        doingButton.gameObject.SetActive(true);
        historyButton.gameObject.SetActive(false);
        homeTypePanel.SetActive(false);
        hisTypePanel.SetActive(true);
        ShowList("工作/学业", todoController.todoListDataSO.workStudyList, true, scoreManager.scoreData.workStudyScore);
    }

    public void ShowAddTaskPanel()
    {
        editPanel.SetActive(true);
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
        // 刷新共享容器
        FlushContainer(items, tasksContainer);

        // 更新当前激活分类文本
        type.text = $"{categoryName}";
    }

    private void ClearContainer(Transform container)
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);
    }

    private void FlushContainer(List<TodoItem> items, Transform container)
    {
        foreach (TodoItem item in items)
        {
            AddTaskToUI(item, container);
        }
    }

    private void AddTaskToUI(TodoItem item, Transform parent)
    {
        parent = tasksContainer;

        GameObject taskItem = Instantiate(todoListItemPrefab, parent);
        BaseItem itemUI = taskItem.GetComponent<ShowItemUI>();
        itemUI.Initialize(item);
    }

    private void OnTaskCompleted(TodoItem item)
    {
        // 调用CompleteTodoItem来移动任务到类型列表并计算分数
        todoController.CompleteTodoItem(item);
        // 更新UI
        ShowList(currentCategoryName, currentList, currentIsCompletedList, currentScore);
    }

    private void OnTaskMoved(TodoItem item, Transform newContainer)
    {
        //     todoController.SaveTodoList();

        //     // 重新加载UI保持顺序
        //     LoadTodoItems();
    }

    private void OnTaskDeleted(TodoItem item)
    {
        //     // 从控制器中移除任务
        //     RemoveItemFromController(item);

        //     // 重新计算所有类型分数（处理从类型列表中删除的情况）
        //     todoController.UpdateTypeScores();

        //     // 如果当前显示的是已完成列表，更新当前分数
        //     if (currentIsCompletedList)
        //     {
        //         // 根据当前类别名称获取最新的分数
        //         currentScore = GetScoreForCategory(currentCategoryName);
        //     }

        //     // 重新显示当前列表以反映变化
        //     ShowList(currentCategoryName, currentList, currentIsCompletedList, currentScore);
    }
}
