using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlantPotIntPair
{
    public PlantPot plantPot;
    public int value;
}

public class GardeningManager : MonoBehaviour
{
    public static GardeningManager Instance;

    [Header("小人设置")]
    public GardenerAI[] gardeners;  // 所有的小人
    [SerializeField] private PlantPotIntPair[] warnUIMapArray;
    private Dictionary<PlantPot, int> warnUIMap = new();
    public PlantPanelUI plantPanel;
    public WarnPanelUI warnPanel;


    private Queue<PlantPot> wateringQueue = new();
    private Queue<PlantPot> plantingQueue = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 注册所有盆栽的事件
        PlantPot[] allPots = FindObjectsOfType<PlantPot>();
        foreach (PlantPot pot in allPots)
        {
            pot.OnNeedWatering.AddListener(OnPotNeedWatering);
            pot.OnNeedPlanting.AddListener(OnPotNeedPlanting);
        }
        foreach (var pair in warnUIMapArray)
        {
            warnUIMap[pair.plantPot] = pair.value;
        }
    }

    /// <summary>
    /// 处理盆栽需要浇水的事件
    /// </summary>
    private void OnPotNeedWatering(PlantPot pot)
    {
        wateringQueue.Enqueue(pot);
        AssignTasks();
    }

    /// <summary>
    /// 处理盆栽需要种植的事件
    /// </summary>
    private void OnPotNeedPlanting(PlantPot pot)
    {
        plantingQueue.Enqueue(pot);
        AssignTasks();
    }

    /// <summary>
    /// 分配任务给空闲的小人
    /// </summary>

    private void AssignTasks()
    {
        // TODO: 最好的任务循环分配给每个小人
        foreach (GardenerAI gardener in gardeners)
        {
            if (!gardener.IsBusy)
            {
                // 优先分配种植任务
                if (plantingQueue.Count > 0)
                {
                    PlantPot pot = plantingQueue.Dequeue();
                    gardener.AssignPlantingTask(pot);
                    break;  // 任务分配出去后跳出循环
                }
                // 然后分配浇水任务
                else if (wateringQueue.Count > 0)
                {
                    PlantPot pot = wateringQueue.Dequeue();
                    gardener.AssignWateringTask(pot);
                    break;
                }
            }
        }

        // 如果没有空闲的小人，则将任务分配给第一个小人
        while (plantingQueue.Count > 0)
        {
            PlantPot pot = plantingQueue.Dequeue();
            gardeners[0].AssignPlantingTask(pot);
        }

        while (wateringQueue.Count > 0)
        {
            PlantPot pot = wateringQueue.Dequeue();
            gardeners[0].AssignPlantingTask(pot);
        }
    }

    #region UI相关
    /// <summary>
    /// 显示盆栽UI详情
    /// </summary>
    /// <param name="plantPot"></param>
    public void ShowPlantDetails(PlantPot plantPot)
    {
        if (plantPanel == null)
        {
            Debug.LogError("PlantPot: plantPanel is null");
            return;
        }

        if (plantPanel.currentPlantPot == plantPot) plantPanel.isPanleOpen = !plantPanel.isPanleOpen;
        else
        {
            plantPanel.SetCurrentPlantPot(plantPot);
            plantPanel.isPanleOpen = true;
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(plantPot.transform.position + new Vector3(0.5f, 1f, 0));

        // 将屏幕坐标转换到画布坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            plantPanel.transform.parent.transform as RectTransform,   // 父节点的世界坐标
            screenPos,
            null, // Overlay 模式下 camera 为 null
            out Vector2 localPos
        );

        plantPanel.transform.localPosition = localPos;
        plantPanel?.gameObject.SetActive(plantPanel.isPanleOpen);
    }

    public void ShowWarning(PlantPot plantPot)
    {
        warnPanel.ShowWarning(plantPot, warnUIMap[plantPot]);
    }

    public void CloseWarning(PlantPot plantPot)
    {
        warnPanel.CloseWarning(plantPot, warnUIMap[plantPot]);
    }

    #endregion

}