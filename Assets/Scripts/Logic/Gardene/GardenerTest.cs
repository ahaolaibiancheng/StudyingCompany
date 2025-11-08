using UnityEngine;

public class GardenerTest : MonoBehaviour
{
    [Header("测试组件")]
    [SerializeField] private GardenerAI gardenerAI;
    [SerializeField] private GardenerAni gardenerAni;
    [SerializeField] private PlantPot testPot;

    void Start()
    {
        // 确保组件引用正确
        if (gardenerAI == null) gardenerAI = GetComponent<GardenerAI>();
        if (gardenerAni == null) gardenerAni = GetComponent<GardenerAni>();

        // 测试日志
        Debug.Log("GardenerAI 和 GardenerAni 组件初始化完成");
        Debug.Log($"GardenerAI 当前状态: {gardenerAI.CurrentState}");
        Debug.Log($"GardenerAni 当前动画状态: {gardenerAni.GetCurrentAnimationState()}");
    }

    void Update()
    {
        // 测试快捷键
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestPlantingTask();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TestWateringTask();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TestForceSit();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TestForceStand();
        }
    }

    /// <summary>
    /// 测试种植任务
    /// </summary>
    private void TestPlantingTask()
    {
        if (testPot != null)
        {
            gardenerAI.AssignPlantingTask(testPot);
            Debug.Log("分配种植任务");
        }
        else
        {
            Debug.LogWarning("未设置测试花盆");
        }
    }

    /// <summary>
    /// 测试浇水任务
    /// </summary>
    private void TestWateringTask()
    {
        if (testPot != null)
        {
            gardenerAI.AssignWateringTask(testPot);
            Debug.Log("分配浇水任务");
        }
        else
        {
            Debug.LogWarning("未设置测试花盆");
        }
    }

    /// <summary>
    /// 测试强制坐下
    /// </summary>
    private void TestForceSit()
    {
        gardenerAni.ForceSit();
        Debug.Log("强制播放坐下动画");
    }

    /// <summary>
    /// 测试强制站立
    /// </summary>
    private void TestForceStand()
    {
        gardenerAni.ForceStand();
        Debug.Log("强制站立");
    }

    /// <summary>
    /// 显示调试信息
    /// </summary>
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("=== Gardener 测试 ===");
        GUILayout.Label($"AI 状态: {gardenerAI.CurrentState}");
        GUILayout.Label($"动画状态: {gardenerAni.GetCurrentAnimationState()}");
        GUILayout.Label($"空闲计时器: {gardenerAni.GetIdleTimer():F1}s");
        GUILayout.Label($"任务队列: {gardenerAI.QueuedTaskCount}");
        GUILayout.Space(10);
        GUILayout.Label("快捷键:");
        GUILayout.Label("1 - 种植任务");
        GUILayout.Label("2 - 浇水任务");
        GUILayout.Label("3 - 强制坐下");
        GUILayout.Label("4 - 强制站立");
        GUILayout.EndArea();
    }
}
