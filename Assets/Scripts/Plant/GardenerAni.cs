using System.Collections;
using UnityEngine;

public class GardenerAni : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private Animator animator;
    [SerializeField] private GardenerAI gardenerAI;

    [Header("动画设置")]
    [SerializeField] private float idleToSitDuration = 30f;

    // 动画参数名称
    private string speedParam = "Speed";
    private string isWorkingParam = "IsWorking";
    private string workTypeParam = "WorkType";
    private string isSittingParam = "IsSitting";

    // 动画哈希（优化性能）
    private int speedHash;
    private int isWorkingHash;
    private int workTypeHash;
    private int isSittingHash;

    // 状态变量
    private float idleTimer = 0f;
    private bool isIdle = false;
    private Vector3 lastPosition;
    private float currentSpeed = 0f;

    // 动画状态
    private AnimationState currentAnimationState = AnimationState.Idle;

    void Start()
    {
        InitializeComponents();
        lastPosition = transform.position;
    }

    void Update()
    {
        UpdateSpeed();
        HandleStateAnimations();
    }

    /// <summary>
    /// 初始化组件和参数
    /// </summary>
    private void InitializeComponents()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (gardenerAI == null) gardenerAI = GetComponent<GardenerAI>();

        // 缓存动画参数哈希
        speedHash = Animator.StringToHash(speedParam);
        isWorkingHash = Animator.StringToHash(isWorkingParam);
        workTypeHash = Animator.StringToHash(workTypeParam);
        isSittingHash = Animator.StringToHash(isSittingParam);
    }

    /// <summary>
    /// 更新移动速度
    /// </summary>
    private void UpdateSpeed()
    {
        // Vector3 currentPosition = transform.position;
        // float distance = Vector3.Distance(currentPosition, lastPosition);
        // currentSpeed = distance / Time.deltaTime;
        // lastPosition = currentPosition;

        // if (animator != null)
        // {
        // }
        animator.SetFloat(speedHash, gardenerAI.currentSpeed);
    }

    /// <summary>
    /// 处理状态动画
    /// </summary>
    private void HandleStateAnimations()
    {
        if (gardenerAI == null) return;

        // TODO: 暂用监听GardenerAI的CurrentState属性来响应状态变化，可用GardenerAI进行通知
        switch (gardenerAI.CurrentState)
        {
            case GardenerState.Idle:
                HandleIdleState();
                break;
            case GardenerState.MovingToTask:
                HandleMovingState();
                break;
            case GardenerState.Working:
                HandleWorkingState();
                break;
            case GardenerState.Resting:
                HandleRestingState();
                break;
        }
    }

    /// <summary>
    /// 处理空闲状态动画
    /// </summary>
    private void HandleIdleState()
    {
        if (!isIdle)
        {
            isIdle = true;
            idleTimer = 0f;
            SetAnimationState(AnimationState.Idle);
        }

        idleTimer += Time.deltaTime;

        // 如果空闲时间超过设定值，播放坐下动画
        if (idleTimer >= idleToSitDuration)
        {
            SetAnimationState(AnimationState.Sitting);
        }
        else
        {
            SetAnimationState(AnimationState.Idle);
        }
    }

    /// <summary>
    /// 处理移动状态动画
    /// </summary>
    private void HandleMovingState()
    {
        isIdle = false;
        idleTimer = 0f;
        SetAnimationState(AnimationState.Moving);

        // 根据移动方向设置对应的移动动画
        UpdateMovementDirection();
    }

    /// <summary>
    /// 处理工作状态动画
    /// </summary>
    private void HandleWorkingState()
    {
        isIdle = false;
        idleTimer = 0f;
        SetAnimationState(AnimationState.Working);
        SetWorkAnimation(gardenerAI.CurrentTaskType);

        // 设置工作动画 - 具体的工作类型动画将在GardenerAI中设置
        // 这里只设置工作状态，具体的工作类型由GardenerAI在PerformWork中设置
    }

    /// <summary>
    /// 处理休息状态动画
    /// </summary>
    private void HandleRestingState()
    {
        isIdle = false;
        idleTimer = 0f;
        SetAnimationState(AnimationState.Sitting);
    }

    /// <summary>
    /// 更新移动方向
    /// </summary>
    private void UpdateMovementDirection()
    {
        Vector2 movementDirection = gardenerAI.movementDirection;
        animator.SetFloat("Horizontal", movementDirection.x);
        animator.SetFloat("Vertical", movementDirection.y);
        gameObject.GetComponent<SpriteRenderer>().flipX = movementDirection.x > 0;
        // 可视化指向目标的射线
        Debug.DrawRay(transform.position, gardenerAI.movementDirection * 2, Color.red);
    }

    /// <summary>
    /// 设置工作动画
    /// </summary>
    private void SetWorkAnimation(GardeningTaskType taskType)
    {
        if (animator == null) return;

        // 根据任务类型设置对应的动画
        switch (taskType)
        {
            case GardeningTaskType.Planting:
                // 播放挖土动画
                animator.SetInteger(workTypeHash, 0); // Planting = 0
                break;
            case GardeningTaskType.Watering:
                // 播放浇水动画
                animator.SetInteger(workTypeHash, 1); // Watering = 1
                break;
        }

        animator.SetBool(isWorkingHash, true);
    }

    /// <summary>
    /// 设置动画状态
    /// </summary>
    private void SetAnimationState(AnimationState newState)
    {
        if (currentAnimationState == newState) return;

        currentAnimationState = newState;

        if (animator == null) return;

        // 重置所有状态参数
        animator.SetBool(isWorkingHash, false);
        animator.SetBool(isSittingHash, false);

        // 设置新状态参数
        switch (newState)
        {
            case AnimationState.Idle:
                // 空闲状态 - 使用默认Idle动画
                break;
            case AnimationState.Moving:
                // 移动状态 - 使用Walk动画
                animator.SetTrigger("Moving");
                break;
            case AnimationState.Working:
                // 工作状态 - 在HandleWorkingState中设置具体工作类型
                animator.SetBool(isWorkingHash, true);
                break;
            case AnimationState.Sitting:
                // 坐下状态
                animator.SetBool(isSittingHash, true);
                break;
        }
    }

    /// <summary>
    /// 强制播放坐下动画（用于外部调用）
    /// </summary>
    public void ForceSit()
    {
        SetAnimationState(AnimationState.Sitting);
        idleTimer = idleToSitDuration; // 设置计时器为最大值，避免立即切换回Idle
    }

    /// <summary>
    /// 强制站立（用于外部调用）
    /// </summary>
    public void ForceStand()
    {
        SetAnimationState(AnimationState.Idle);
        idleTimer = 0f;
        isIdle = false;
    }

    /// <summary>
    /// 设置Idle到Sit的持续时间
    /// </summary>
    public void SetIdleToSitDuration(float duration)
    {
        idleToSitDuration = duration;
    }

    /// <summary>
    /// 获取当前动画状态
    /// </summary>
    public AnimationState GetCurrentAnimationState()
    {
        return currentAnimationState;
    }

    /// <summary>
    /// 获取空闲计时器值
    /// </summary>
    public float GetIdleTimer()
    {
        return idleTimer;
    }

    public void SetAniFlipX(int flip)
    {
        gameObject.GetComponent<SpriteRenderer>().flipX = flip == 1;
    }
}

// 动画状态枚举
public enum AnimationState
{
    Idle = 0,       // 空闲
    Moving = 1,     // 移动
    Working = 2,    // 工作
    Sitting = 3     // 坐下
}
