using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlantPot : MonoBehaviour
{
    [Header("盆栽状态")]
    [SerializeField] private bool isPlanted = false;          // 是否已种植
    [SerializeField] private float currentMoisture = 0f;      // 当前湿度
    [SerializeField] public int currentLevel = 1;            // 当前等级
    [SerializeField] public float currentExp = 0f;           // 当前经验值

    [Header("湿度设置")]
    [SerializeField] private float maxMoisture = 100f;        // 最大湿度
    [SerializeField] private float moistureDecreaseRate = 1f; // 湿度下降速率（每分钟）
    [SerializeField] private float moistureFrequency = 60f;       // 湿度下降速率（每分钟）
    [SerializeField] private float lowMoistureThreshold = 30f; // 低湿度阈值

    [Header("经验设置")]
    [SerializeField] public float[] levelUpExpRequirements = { 100f, 200f, 400f, 800f }; // 每级所需经验

    [Header("小人工作位置")]
    [SerializeField] private Transform workPosition;          // 小人的工作位置


    [Header("事件")]
    [HideInInspector]
    public UnityEvent<PlantPot> OnNeedWatering;              // 需要浇水时触发
    public UnityEvent<PlantPot> OnNeedPlanting;              // 需要种植时触发
    public UnityEvent<int> OnLevelUp;                        // 升级时触发
    public UnityEvent<PlantPot> OnMoistureChanged;           // 湿度变化时触发
    public UnityEvent<float> OnExpChanged;                   // 经验变化时触发

    // 私有属性
    private bool hasRequestedWatering = false;               // 是否已请求浇水
    private bool hasRequestedPlanting = false;               // 是否已请求种植
    private Coroutine moistureDecreaseCoroutine;             // 湿度下降协程

    // 公开属性
    public bool IsPlanted => isPlanted;
    public float CurrentMoisture => currentMoisture;
    public int CurrentLevel => currentLevel;
    public float CurrentExp => currentExp;
    public Transform WorkPosition => workPosition;
    public bool NeedsWatering => currentMoisture < lowMoistureThreshold && isPlanted;
    public bool NeedsPlanting => !isPlanted;

    /*  */
    void Start()
    {
        // 开始湿度下降协程
        StartMoistureDecrease();
    }

    void Update()
    {
        CheckAndRequestActions();
    }

    /// <summary>
    /// 开始湿度自动下降
    /// </summary>
    private void StartMoistureDecrease()
    {
        if (moistureDecreaseCoroutine != null)
            StopCoroutine(moistureDecreaseCoroutine);

        moistureDecreaseCoroutine = StartCoroutine(DecreaseMoistureOverTime());
    }

    /// <summary>
    /// 随时间降低湿度
    /// </summary>
    private IEnumerator DecreaseMoistureOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(moistureFrequency);

            if (isPlanted && currentMoisture > 0)
            {
                currentMoisture = Mathf.Max(0, currentMoisture - moistureDecreaseRate);
                // OnMoistureChanged?.Invoke(this);
            }
        }
    }

    /// <summary>
    /// 检查并请求需要的动作
    /// </summary>
    private void CheckAndRequestActions()
    {
        // 检查是否需要浇水
        if (NeedsWatering && !hasRequestedWatering)
        {
            // 触发UI浇水预警
            GardeningManager.Instance.ShowWarning(this);
        }

        // 检查是否需要种植
        else if (NeedsPlanting && !hasRequestedPlanting)
        {
            // 触发UI种植预警
            GardeningManager.Instance.ShowWarning(this);
        }
        else
        {
            GardeningManager.Instance.CloseWarning(this);
        }
    }

    /// <summary>
    /// 请求浇水
    /// </summary>
    public void RequestWatering()
    {
        if (!NeedsWatering || hasRequestedWatering) return;
        hasRequestedWatering = true;
        OnNeedWatering?.Invoke(this);
        // Debug.Log($"盆栽 {name} 需要浇水！当前湿度: {currentMoisture}");
    }

    /// <summary>
    /// 请求种植
    /// </summary>
    public void RequestPlanting()
    {
        if (!NeedsPlanting || hasRequestedPlanting) return;
        hasRequestedPlanting = true;
        OnNeedPlanting?.Invoke(this);
        // Debug.Log($"盆栽 {name} 需要种植！");
    }

    /// <summary>
    /// 种植植物（由小人调用）
    /// </summary>
    public void Plant()
    {
        if (isPlanted)
        {
            Debug.LogWarning($"盆栽 {name} 已经种植过了！");
            return;
        }

        isPlanted = true;
        hasRequestedPlanting = false;
        currentMoisture = maxMoisture;

        Debug.Log($"盆栽 {name} 种植成功！");

        // 触发湿度变化事件
        // OnMoistureChanged?.Invoke(this);
    }

    /// <summary>
    /// 浇水（由小人调用）
    /// </summary>
    /// <param name="waterAmount">浇水量</param>
    public void Water(float waterAmount)
    {
        if (!isPlanted)
        {
            Debug.LogWarning($"盆栽 {name} 还没有种植，不能浇水！");
            return;
        }

        float oldMoisture = currentMoisture;
        currentMoisture = Mathf.Min(maxMoisture, currentMoisture + waterAmount);
        hasRequestedWatering = false;

        // 计算获得的经验值（基于浇水量）
        float gainedExp = waterAmount * 0.5f;
        AddExp(gainedExp);

        Debug.Log($"盆栽 {name} 浇水成功！湿度: {oldMoisture} -> {currentMoisture}, 获得经验: {gainedExp}");

        // 触发湿度变化事件
        OnMoistureChanged?.Invoke(this);
    }

    /// <summary>
    /// 添加经验值
    /// </summary>
    /// <param name="exp">经验值</param>
    public void AddExp(float exp)
    {
        currentExp += exp;
        OnExpChanged?.Invoke(currentExp);

        // 检查是否升级
        CheckLevelUp();
    }

    /// <summary>
    /// 检查并执行升级
    /// </summary>
    private void CheckLevelUp()
    {
        if (currentLevel - 1 < levelUpExpRequirements.Length)
        {
            float requiredExp = levelUpExpRequirements[currentLevel - 1];

            if (currentExp >= requiredExp)
            {
                LevelUp();
            }
        }
    }

    /// <summary>
    /// 升级
    /// </summary>
    private void LevelUp()
    {
        currentLevel++;
        currentExp = 0f; // 升级后经验清零，或者可以保留超额经验

        Debug.Log($"盆栽 {name} 升级了！当前等级: {currentLevel}");

        // 触发升级事件
        OnLevelUp?.Invoke(currentLevel);

        // 升级后可以增加一些效果
        OnLevelUpEffects();
    }

    /// <summary>
    /// 升级时的特效和变化
    /// </summary>
    private void OnLevelUpEffects()
    {
        // 这里可以添加升级时的视觉效果
        // 例如：改变植物外观、播放粒子效果等

        // 示例：稍微放大植物表示成长
        StartCoroutine(ScaleEffect());
    }

    /// <summary>
    /// 升级时的缩放特效
    /// </summary>
    private IEnumerator ScaleEffect()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;

        // 放大
        float duration = 0.2f;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, timer / duration);
            yield return null;
        }

        // 缩小回原尺寸
        timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, timer / duration);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    /// <summary>
    /// 获取升级所需经验
    /// </summary>
    public float GetRequiredExpForNextLevel()
    {
        if (currentLevel - 1 < levelUpExpRequirements.Length)
        {
            return levelUpExpRequirements[currentLevel - 1];
        }
        return 0f;
    }

    /// <summary>
    /// 获取湿度百分比
    /// </summary>
    public float GetMoisturePercentage()
    {
        return currentMoisture / maxMoisture;
    }

    /// <summary>
    /// 重置请求状态（当小人无法完成任务时调用）
    /// </summary>
    public void ResetRequestStates()
    {
        hasRequestedWatering = false;
        hasRequestedPlanting = false;
    }

    void OnDestroy()
    {
        // 停止所有协程
        if (moistureDecreaseCoroutine != null)
            StopCoroutine(moistureDecreaseCoroutine);
    }

    internal void ShowPlantDetails()
    {
        GardeningManager.Instance.ShowPlantDetails(this);
    }

#if UNITY_EDITOR
    // 在Scene视图中显示调试信息
    void OnDrawGizmosSelected()
    {
        if (workPosition != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(workPosition.position, 0.1f);
            // Gizmos.DrawIcon(workPosition.position + Vector3.up * 0.5f, "PlantPotGizmo");
        }
    }
#endif
}