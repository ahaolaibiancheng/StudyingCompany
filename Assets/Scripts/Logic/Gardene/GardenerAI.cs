using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class GardenerAI : MonoBehaviour
{
    [Header("组件引用")]
    // [SerializeField] private Seeker seeker;
    [SerializeField] private AIPath path;
    private AIDestinationSetter setter;

    [Header("移动设置")]
    [SerializeField] private float rotationSpeed = 5f; // 旋转速度
    [SerializeField] private float stoppingDistance = 0.1f; // 停止距离
    [SerializeField] private float walkSpeed = 1f; // 行走速度
    [SerializeField] private float runSpeed = 3.5f; // 奔跑速度

    [Header("任务设置")]
    [SerializeField] private float workDuration = 5f; // 工作持续时间
    [SerializeField] private float restDuration = 10f; // 休息持续时间

    // 任务队列
    private Queue<GardeningTask> taskQueue = new Queue<GardeningTask>();
    private GardeningTask currentTask;

    // 状态变量
    private bool isBusy = false;
    private bool isResting = false;
    private GardenerState currentState = GardenerState.Idle;

    // 事件
    public UnityEvent<GardenerAI> OnTaskCompleted;
    public UnityEvent<GardenerAI> OnTaskStarted;
    public UnityEvent<GardenerAI> OnStartedResting;

    // 公开属性
    public bool IsBusy => isBusy;
    public GardenerState CurrentState => currentState;
    public GardeningTaskType CurrentTaskType => currentTask.taskType;
    public int QueuedTaskCount => taskQueue.Count;
    public float currentSpeed => path.maxSpeed;
    public Vector2 movementDirection => path.velocity.normalized;


    void Start()
    {
        // 初始化组件
        InitializeComponents();

        // 设置初始状态
        SetState(GardenerState.Idle);
    }

    void Update()
    {
        // 状态机更新
        switch (currentState)
        {
            case GardenerState.Idle:
                UpdateIdleState();
                break;
            case GardenerState.MovingToTask:
                UpdateMovingState();
                break;
            case GardenerState.Working:
                // 工作状态由协程处理
                break;
            case GardenerState.Resting:
                UpdateRestingState();
                break;
        }
    }

    /// <summary>
    /// 初始化组件和参数
    /// </summary>
    private void InitializeComponents()
    {
        if (setter == null) setter = GetComponent<AIDestinationSetter>();
        // if (modelTransform == null) modelTransform = transform;
    }


    /// <summary>
    /// 设置状态
    /// </summary>
    private void SetState(GardenerState newState)
    {
        currentState = newState;
        // Debug.Log($"小人 {name} 状态改变: {newState}");
    }

    /// <summary>
    /// 更新空闲状态
    /// </summary>
    private void UpdateIdleState()
    {
        // 检查是否有任务
        if (taskQueue.Count > 0 && !isBusy)
        {
            StartNextTask();
        }
    }

    /// <summary>
    /// 更新移动状态
    /// </summary>
    private void UpdateMovingState()
    {
        if (currentTask == null) return;

        // 检查是否到达目的地
        if (Vector2.Distance(transform.position, currentTask.workPosition.position) < stoppingDistance)
        {
            StartCoroutine(PerformWork());
        }
    }

    /// <summary>
    /// 更新休息状态
    /// </summary>
    private void UpdateRestingState()
    {
        // 检查是否有任务
        if (taskQueue.Count > 0 && !isBusy)
        {
            isResting = false;
            SetState(GardenerState.Idle);
        }
    }

    /// <summary>
    /// 分配种植任务
    /// </summary>
    public void AssignPlantingTask(PlantPot pot)
    {
        if (pot == null) return;

        GardeningTask newTask = new GardeningTask
        {
            taskType = GardeningTaskType.Planting,
            targetPot = pot,
            workPosition = pot.WorkPosition
        };

        AddTask(newTask);
    }

    /// <summary>
    /// 分配浇水任务
    /// </summary>
    public void AssignWateringTask(PlantPot pot)
    {
        if (pot == null) return;

        GardeningTask newTask = new GardeningTask
        {
            taskType = GardeningTaskType.Watering,
            targetPot = pot,
            workPosition = pot.WorkPosition
        };

        AddTask(newTask);
    }

    /// <summary>
    /// 添加任务到队列
    /// </summary>
    public void AddTask(GardeningTask task)
    {
        taskQueue.Enqueue(task);
        // Debug.Log($"小人 {name} 收到新任务: {task.taskType}, 队列长度: {taskQueue.Count}");
    }

    /// <summary>
    /// 开始执行下一个任务
    /// </summary>
    private void StartNextTask()
    {
        if (taskQueue.Count == 0 || isBusy) return;

        currentTask = taskQueue.Dequeue();
        isBusy = true;

        // 触发任务开始事件
        OnTaskStarted?.Invoke(this);

        // 移动到任务位置
        MoveToTaskLocation();
    }

    /// <summary>
    /// 移动到任务位置
    /// </summary>
    private void MoveToTaskLocation()
    {
        SetState(GardenerState.MovingToTask);

        // 设置移动速度
        // agent.speed = currentTask.isUrgent ? runSpeed : walkSpeed;

        // 设置目的地
        path.maxSpeed = 0.8f;
        setter.target = currentTask.workPosition;

    }

    /// <summary>
    /// 执行工作
    /// </summary>
    private IEnumerator PerformWork()
    {
        SetState(GardenerState.Working);
        path.maxSpeed = 0f;


        // 等待工作动画播放
        yield return new WaitForSeconds(workDuration);

        // 执行具体的工作逻辑
        ExecuteTaskAction();

        // 完成任务
        CompleteTask(true);

        // 检查是否休息
        if (taskQueue.Count == 0)
        {
            StartCoroutine(Rest());
        }
    }

    /// <summary>
    /// 执行具体的任务动作
    /// </summary>
    private void ExecuteTaskAction()
    {
        if (currentTask?.targetPot == null) return;

        switch (currentTask.taskType)
        {
            case GardeningTaskType.Planting:
                currentTask.targetPot.Plant();
                break;
            case GardeningTaskType.Watering:
                currentTask.targetPot.Water(50f); // 浇水量50
                break;
        }

        // Debug.Log($"小人 {name} 完成了 {currentTask.taskType} 动作");
    }

    /// <summary>
    /// 完成任务
    /// </summary>
    private void CompleteTask(bool success)
    {
        // 触发任务完成事件
        OnTaskCompleted?.Invoke(this);

        // 重置状态
        isBusy = false;
        currentTask = null;

        SetState(GardenerState.Idle);

        // Debug.Log($"小人 {name} 完成任务, 成功: {success}");
    }

    /// <summary>
    /// 休息
    /// </summary>
    private IEnumerator Rest()
    {
        SetState(GardenerState.Resting);
        isResting = true;

        // 触发开始休息事件
        OnStartedResting?.Invoke(this);
        yield return null;
    }

    /// <summary>
    /// 获取下一个空闲位置（用于坐下休息）
    /// </summary>
    public Vector3 GetNextRestingPosition()
    {
        // 这里可以实现在花盆边缘找到空闲位置
        // 简单实现：返回当前位置附近的一个随机点
        Vector2 randomCircle = Random.insideUnitCircle * 2f;
        return transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    /// <summary>
    /// 立即停止当前任务
    /// </summary>
    public void StopCurrentTask()
    {
        if (currentTask != null)
        {
            StopAllCoroutines();

            CompleteTask(false);
        }
    }

    /// <summary>
    /// 清空所有任务
    /// </summary>
    public void ClearAllTasks()
    {
        taskQueue.Clear();
        StopCurrentTask();
    }

    /// <summary>
    /// 在Scene视图中显示调试信息
    /// </summary>
    void OnDrawGizmos()
    {
        // 状态标签
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1f,
            $"状态: {currentState}\n队列: {taskQueue.Count}");
#endif
    }
}

// 任务类型枚举
public enum GardeningTaskType
{
    Planting = 0,   // 种植
    Watering = 1    // 浇水
}

// 小人状态枚举
public enum GardenerState
{
    Idle = 0,           // 空闲
    MovingToTask = 1,   // 移动中
    Working = 2,        // 工作中
    Resting = 3         // 休息中
}

// 任务数据结构
[System.Serializable]
public class GardeningTask
{
    public GardeningTaskType taskType;
    public PlantPot targetPot;
    public Transform workPosition;
    public bool isUrgent; // 是否紧急任务
}
