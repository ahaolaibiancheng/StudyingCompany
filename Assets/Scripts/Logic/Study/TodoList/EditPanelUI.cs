using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Spine;
using Unity.Mathematics;

public class EditPanelUI : MonoBehaviour
{
    public TextMeshProUGUI type;
    public TMP_InputField keywordInput;
    public TMP_InputField descriptionInput;
    public TMP_Dropdown deadlineYearDd;
    public TMP_Dropdown deadlineMouDd;
    public TMP_Dropdown deadlineDayDd;
    public TMP_Dropdown timeConsumeDd;
    public TMP_Dropdown typeDd;

    public Toggle urgentToggle;
    public Toggle importantToggle;
    public Transform weekdayToggles;
    public Button submitButton;
    public Button cancelButton;

    private TodoListController todoController;

    private void Awake()
    {
        todoController = GetComponentInParent<TodoListController>();

        submitButton.onClick.AddListener(AddNewTask);
        cancelButton.onClick.AddListener(CancelNewTask);
    }

    private void Start()
    {
        // TODO: 重置表单

        DeadlineDdInit();
        TimeConsumeDdInit();
        TypeDdInit();
    }

    public void DeadlineDdInit()
    {
        // 初始化年份下拉框
        deadlineYearDd.ClearOptions();
        List<string> years = new List<string>();
        int currentYear = System.DateTime.Now.Year;
        for (int i = currentYear; i <= currentYear + 10; i++)
        {
            years.Add(i.ToString());
        }
        deadlineYearDd.AddOptions(years);
        deadlineYearDd.value = 0;

        // 初始化月份下拉框
        deadlineMouDd.ClearOptions();
        List<string> months = new List<string>();
        for (int i = 1; i <= 12; i++)
        {
            months.Add(i.ToString());
        }
        deadlineMouDd.AddOptions(months);
        deadlineMouDd.value = 0;

        // 初始化日期下拉框
        deadlineDayDd.ClearOptions();
        List<string> days = new List<string>();
        for (int i = 1; i <= 31; i++)
        {
            days.Add(i.ToString());
        }
        deadlineDayDd.AddOptions(days);
        deadlineDayDd.value = 0;
    }
    private void TimeConsumeDdInit()
    {
        timeConsumeDd.ClearOptions();
        List<string> options = new List<string>
            {
                "小于一小时",   // 难度1，每日基础分为1
                "小于二小时",   // 难度2，每日基础分为2
                "小于四小时",   // 难度3，每日基础分为4
                "小于八小时",   // 难度4，每日基础分为8
            };
        timeConsumeDd.AddOptions(options);
        timeConsumeDd.value = 0; // 默认选择第一个选项
    }
    private void TypeDdInit()
    {
        typeDd.ClearOptions();
        List<string> options = new List<string>
            {
                "工作/学业",
                "个人健康",
                "家庭生活",
                "社交人际",
                "个人成长/兴趣",
                "财务管理"
            };
        typeDd.AddOptions(options);
        typeDd.value = 0; // 默认选择第一个选项
    }

    private void AddNewTask()
    {
        if (!CheckNewTask())
        {
            Debug.LogError("请填写完整任务信息");
            return;
        }

        TodoItem newItem = new TodoItem
        {
            guid = Guid.NewGuid().ToString(),
            keywords = keywordInput.text,
            // isImportant = importantToggle.isOn,
            // isUrgent = urgentToggle.isOn,
            isCompleted = false,
            type = typeDd.options[typeDd.value].text,
            rewardDaily = GetRewardDaily(),
            rewardTotal = 0,
            completedTimes = 0,
            period = GetPeriod(),
            description = descriptionInput.text,
            sortOrder = 0,
            creationTime = DateTime.Now,
            // deadlineTime
            completionTime = DateTime.MaxValue,
        };

        // 拼接年月日下拉框的值组成完整日期
        string year = deadlineYearDd.options[deadlineYearDd.value].text;
        string month = deadlineMouDd.options[deadlineMouDd.value].text;
        string day = deadlineDayDd.options[deadlineDayDd.value].text;

        // 组合成完整日期字符串 (格式: YYYY-MM-DD)
        string deadlineDate = $"{year}/{month.PadLeft(2, '0')}/{day.PadLeft(2, '0')}";
        newItem.deadlineTime = DateTime.ParseExact(deadlineDate, "yyyy/MM/dd", null);

        // 保存数据
        todoController.AddTodoItem(newItem);

        CancelNewTask();
    }

    private void CancelNewTask()
    {
        gameObject.SetActive(false);
    }

    private bool CheckNewTask()
    {
        if (string.IsNullOrEmpty(keywordInput.text) || string.IsNullOrEmpty(descriptionInput.text))
        {
            Debug.Log("请填写完整任务信息: TMP_InputField");
            return false;
        }

        if (deadlineYearDd == null || deadlineMouDd == null || deadlineDayDd == null ||
            timeConsumeDd == null || typeDd == null)
        {
            Debug.Log("请填写完整任务信息: TMP_Dropdown");
            return false;
        }

        if (!CheckPeriod())
        {
            Debug.Log("请填写完整任务信息: period[]");
            return false;
        }
        return true;
    }
    private int GetRewardDaily()
    {
        return (int)MathF.Pow(2, timeConsumeDd.value);
    }

    private bool[] GetPeriod()
    {
        bool[] period = new bool[7];
        for (int i = 0; i < 7; i++)
        {
            period[i] = weekdayToggles.GetChild(i).GetComponent<Toggle>().isOn;
        }
        return period;
    }

    private bool CheckPeriod()
    {
        int count = 0;
        for (int i = 0; i < 7; i++)
        {
            count += (weekdayToggles.GetChild(i).GetComponent<Toggle>().isOn) ? 1 : 0;
        }
        return count > 0;
    }
}
