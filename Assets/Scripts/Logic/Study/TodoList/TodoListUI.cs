using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TodoListUI : MonoBehaviour
{
    // public static TodoListUI Instance;

    // [Header("Tasks Container")]
    // public Transform tasksContainer;

    // [Header("Add Task UI")]
    // public GameObject addTaskPanel;
    // public GameObject addUpPanel;
    // public GameObject hisUpPanel;


    // [Header("Active Category Text")]
    // public Text activeCategoryText1;
    // public Text activeCategoryText2;

    // [Header("Score Display")]
    // public Text scoreText;

    // [Header("Add Task Button")]

    // public Button taskButton;

    // public Button closeButton;

    // [Header("Delete Area")]
    // public RectTransform deleteArea;
    // public Image deleteAreaImage;
    // public Text deleteAreaText;







    // void Awake()
    // {
    //     Instance = this;
    // }

    // void Start()
    // {
    //     todoController = GetComponent<TodoListController>();
    //     if (todoController == null)
    //         todoController = gameObject.AddComponent<TodoListController>();
    //     scoreManager = GetComponent<ScoreManager>();
    //     if (scoreManager == null)
    //         scoreManager = gameObject.AddComponent<ScoreManager>();

    //     submitButton.onClick.AddListener(AddNewTask);
    //     cancelButton.onClick.AddListener(CancelNewTask);
    //     addTaskButton.onClick.AddListener(ShowAddTaskPanel);
    //     taskButton.onClick.AddListener(ShowTaskPanel);
    //     historyButton.onClick.AddListener(ShowHistoryPanel);
    //     closeButton.onClick.AddListener(CloseAddTaskPanel);



    //     // 默认显示重要且紧急分类
    //     ShowList("重要且紧急", todoController.todoListDataSO.importantUrgent, false);
    //     addTaskPanel.SetActive(false);

    //     // 初始化删除区域 - 默认隐藏
    //     if (deleteArea != null)
    //     {
    //         deleteArea.gameObject.SetActive(false);
    //         if (deleteAreaImage != null)
    //         {
    //             deleteAreaImage.color = new Color(1f, 0.5f, 0.5f, 0.3f); // 半透明红色背景
    //         }
    //         if (deleteAreaText != null)
    //         {
    //             deleteAreaText.text = "拖拽至此删除";
    //         }
    //     }



    //     LoadTodoItems();
    // }



    // public void CloseAddTaskPanel()
    // {
    //     // 需要同时删除3个panel
    //     Transform grandpa = closeButton.transform.parent.parent;
    //     grandpa.gameObject.SetActive(false);
    // }



    // private DateTime ParseDueDate(string dateString)
    // {
    //     if (DateTime.TryParse(dateString, out DateTime result))
    //         return result;
    //     return DateTime.Now.AddDays(7); // 默认7天后
    // }

    // private void LoadTodoItems()
    // {
    //     // 清空共享容器
    //     ClearContainer(tasksContainer);

    //     // 添加当前列表的任务到共享容器
    //     if (currentList != null)
    //     {
    //         AddItemsToContainer(currentList, tasksContainer);
    //     }
    // }


    // // 根据类别名称获取对应的分数
    // private int GetScoreForCategory(string categoryName)
    // {
    //     switch (categoryName)
    //     {
    //         case "工作/学业":
    //             return scoreManager.scoreData.workStudyScore;
    //         case "个人健康":
    //             return scoreManager.scoreData.personalHealthScore;
    //         case "家庭生活":
    //             return scoreManager.scoreData.familyLifeScore;
    //         case "社交人际":
    //             return scoreManager.scoreData.socialRelationsScore;
    //         case "个人成长/兴趣":
    //             return scoreManager.scoreData.personalGrowthScore;
    //         case "财务管理":
    //             return scoreManager.scoreData.financialManagementScore;
    //         default:
    //             return 0;
    //     }
    // }

    // private void RemoveItemFromController(TodoItem item)
    // {
    //     // 从所有10个列表中移除任务
    //     todoController.todoListDataSO.importantUrgent.Remove(item);
    //     todoController.todoListDataSO.notImportantUrgent.Remove(item);
    //     todoController.todoListDataSO.importantNotUrgent.Remove(item);
    //     todoController.todoListDataSO.notImportantNotUrgent.Remove(item);

    //     todoController.todoListDataSO.workStudyList.Remove(item);
    //     todoController.todoListDataSO.personalHealthList.Remove(item);
    //     todoController.todoListDataSO.familyLifeList.Remove(item);
    //     todoController.todoListDataSO.socialRelationsList.Remove(item);
    //     todoController.todoListDataSO.personalGrowthList.Remove(item);
    //     todoController.todoListDataSO.financialManagementList.Remove(item);
    // }

    // public void ShowDeleteArea()
    // {
    //     deleteArea.gameObject.SetActive(true);
    //     deleteAreaText.text = "拖拽至此删除";
    // }

    // public void HideDeleteArea()
    // {
    //     deleteArea.gameObject.SetActive(false);
    // }
}
