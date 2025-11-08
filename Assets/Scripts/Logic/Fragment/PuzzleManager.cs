using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    [Header("拼图设置")]
    public int rows = 5;                  // 行数
    public int cols = 5;                  // 列数
    public GameObject puzzleSlotPrefab;   // 拼图格Prefab
    public Transform gridParent;          // GridLayoutGroup挂载点

    [Header("奖励与进度")]
    public RewardController rewardController;

    [HideInInspector]
    public List<PuzzleSlot> slots = new List<PuzzleSlot>();

    void Awake()
    {
        // 自动生成拼图格
        if (gridParent != null && puzzleSlotPrefab != null && slots.Count == 0)
        {
            GenerateGrid();
        }
        else
        {
            // 若已手动拖入 slots 列表，则直接使用
            slots = GetComponentsInChildren<PuzzleSlot>().ToList();
        }
    }

    /// <summary>
    /// 动态生成拼图格
    /// </summary>
    public void GenerateGrid()
    {
        // 清理旧子物体（如有）
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }
        slots.Clear();

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject slotGO = Instantiate(puzzleSlotPrefab, gridParent);
                slotGO.name = $"Slot_{r}_{c}";
                PuzzleSlot slot = slotGO.GetComponent<PuzzleSlot>();
                slots.Add(slot);
            }
        }
    }

    /// <summary>
    /// 获取下一个未填充的拼图槽
    /// </summary>
    public PuzzleSlot GetNextEmptySlot()
    {
        return slots.FirstOrDefault(s => !s.isFilled);
    }

    /// <summary>
    /// 添加碎片到拼图槽并检测完成
    /// </summary>
    public void AddFragmentToSlot(PuzzleSlot slot)
    {
        slot.isFilled = true;
        CheckCompletion();
    }

    private void CheckCompletion()
    {
        if (slots.All(s => s.isFilled))
        {
            Debug.Log("🎉 拼图完成！");
            rewardController?.TriggerStory();
        }
    }
}
