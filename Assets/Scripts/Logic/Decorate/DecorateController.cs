using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DecorateController : MonoBehaviour
{
    public static DecorateController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public string FindUidByTableId(int tableId)
    {
        List<DecorateLocalItem> allItems = GetAllDecorateItems();
        DecorateLocalItem item = allItems.Find(i => i.id == tableId);
        return item?.uid; // 返回uid或null
    }

    private DecorateTable decorateTable;

    public DecorateTable GetDecorateTable(DecorateType type)
    {
        string path = "";
        switch (type)
        {
            case DecorateType.Mouth:
                path = "TableData/MouthList";
                break;
            case DecorateType.Eyes:
                path = "TableData/EyesList";
                break;
            case DecorateType.Cheeks:
                path = "TableData/CheeksList";
                break;
            default:
                break;
        }
        if (decorateTable == null)
        {
            decorateTable = Resources.Load<DecorateTable>(path);
        }
        return decorateTable;
    }

    public DecorateLocalData GetDecorateLocalData()
    {
        return DecorateLocalData.Instance.LoadDecorate();
    }

    // 获取所有装饰品项目的合并列表
    public List<DecorateLocalItem> GetAllDecorateItems()
    {
        DecorateLocalData data = GetDecorateLocalData();
        List<DecorateLocalItem> allItems = new List<DecorateLocalItem>();
        allItems.AddRange(data.mouthList);
        allItems.AddRange(data.eyesList);
        allItems.AddRange(data.cheeksList);
        return allItems;
    }

    public DecorateTableItem GetDecorateItemById(DecorateType type, int id)
    {
        List<DecorateTableItem> decorateDataList = GetDecorateTable(type).decorateTableItems;
        foreach (DecorateTableItem item in decorateDataList)
        {
            if (item.id == id)
            {
                return item;
            }
        }
        return null;
    }

    public DecorateLocalItem GetDecorateLocalItemByUId(string uid)
    {
        List<DecorateLocalItem> allItems = GetAllDecorateItems();
        foreach (DecorateLocalItem item in allItems)
        {
            if (item.uid == uid)
            {
                return item;
            }
        }
        return null;
    }

    public List<DecorateLocalItem> GetSortDecorateLocalData(DecorateType type)
    {
        List<DecorateLocalItem> allItems = GetAllDecorateItems();
        allItems.Sort(new DecorateItemComparer(type));
        return allItems;
    }
}

public class DecorateItemComparer : IComparer<DecorateLocalItem>
{
    private DecorateType _type;

    public DecorateItemComparer(DecorateType type)
    {
        _type = type;
    }

    public int Compare(DecorateLocalItem a, DecorateLocalItem b)
    {
        if (a == null && b == null) return 0;
        if (a == null) return -1;
        if (b == null) return 1;

        DecorateTableItem x = DecorateController.Instance.GetDecorateItemById(_type, a.id);
        DecorateTableItem y = DecorateController.Instance.GetDecorateItemById(_type, b.id);

        // Handle null table items
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        // First sort by star descending
        int starComparison = y.star.CompareTo(x.star);
        if (starComparison != 0)
        {
            return starComparison;
        }

        // If stars are equal, sort by id descending
        return y.id.CompareTo(x.id);
    }
}
