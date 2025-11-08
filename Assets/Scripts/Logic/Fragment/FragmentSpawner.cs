using UnityEngine;

public class FragmentSpawner : MonoBehaviour
{
    [Header("控制器引用")]
    public TodoListController todoListController;
    [Header("碎片Prefab配置")]
    public GameObject commonFragmentPrefab;
    public GameObject rareFragmentPrefab;
    public GameObject epicFragmentPrefab;
    public GameObject legendaryFragmentPrefab;

    [Header("拼图目标")]
    public PuzzleManager puzzleManager;

    [Header("碎片飞行参数")]
    public float spawnRadius = 300f;
    public float flyDuration = 1.2f;

    void OnEnable()
    {
        todoListController.OnTaskComplete += HandleTaskComplete;
    }

    void OnDisable()
    {
        todoListController.OnTaskComplete -= HandleTaskComplete;
    }

    private void HandleTaskComplete(FragmentType fragType)
    {
        // 获取目标插槽
        PuzzleSlot targetSlot = puzzleManager.GetNextEmptySlot();
        if (targetSlot == null) return;

        // 随机生成碎片起点（屏幕边缘或随机半径）
        Vector3 startPos = Random.insideUnitCircle * spawnRadius;
        GameObject fragPrefab = GetPrefabByType(fragType);

        GameObject fragment = Instantiate(fragPrefab, startPos, Quaternion.identity, transform);
        FragmentController controller = fragment.GetComponent<FragmentController>();
        controller.Initialize(targetSlot, flyDuration);
    }

    private GameObject GetPrefabByType(FragmentType type)
    {
        return type switch
        {
            FragmentType.Common => commonFragmentPrefab,
            FragmentType.Rare => rareFragmentPrefab,
            FragmentType.Epic => epicFragmentPrefab,
            FragmentType.Legendary => legendaryFragmentPrefab,
            _ => commonFragmentPrefab
        };
    }
}
