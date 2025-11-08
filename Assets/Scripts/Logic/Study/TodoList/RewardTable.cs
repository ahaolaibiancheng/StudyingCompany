using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum FragmentType
{
    Common, Rare, Epic, Legendary
}

[System.Serializable]
public class RewardTableEntry
{
    public FragmentType fragmentType;
    [Range(0f, 1f)] public float weight; // 概率权重
}

[CreateAssetMenu(fileName = "RewardTable", menuName = "PuzzleSystem/RewardTable")]
public class RewardTable : ScriptableObject
{
    [Header("重要紧急任务")]
    public List<RewardTableEntry> importantUrgentTable;

    [Header("不重要紧急任务")]
    public List<RewardTableEntry> notImportantUrgentTable;

    [Header("重要不紧急任务")]
    public List<RewardTableEntry> importantNotUrgentTable;

    [Header("不重要不紧急任务")]
    public List<RewardTableEntry> notImportantNotUrgentTable;

    /// <summary>
    /// 获取指定类型的任务奖励
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public FragmentType GetRandomFragment(TaskType type)
    {
        List<RewardTableEntry> table = GetTableByType(type);
        float total = table.Sum(e => e.weight);
        float roll = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var entry in table)
        {
            cumulative += entry.weight;
            if (roll <= cumulative)
                return entry.fragmentType;
        }
        return table[0].fragmentType;
    }

    private List<RewardTableEntry> GetTableByType(TaskType type)
    {
        return type switch
        {
            TaskType.ImportantUrgent => importantUrgentTable,
            TaskType.NotImportantUrgent => notImportantUrgentTable,
            TaskType.ImportantNotUrgent => importantNotUrgentTable,
            TaskType.NotImportantNotUrgent => notImportantNotUrgentTable,
            _ => importantUrgentTable
        };
    }
}
