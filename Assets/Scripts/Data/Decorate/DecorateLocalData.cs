using UnityEngine;
using System.Collections.Generic;

public class DecorateLocalData
{
    private static DecorateLocalData _instance;
    public List<DecorateLocalItem> mouthList;
    public List<DecorateLocalItem> eyesList;
    public List<DecorateLocalItem> cheeksList;
    
    public static DecorateLocalData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DecorateLocalData();
            }
            return _instance;
        }
    }

    public void SaveDecorate()
    {
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("DecorateLocalData", json);
        PlayerPrefs.Save();
    }

    public DecorateLocalData LoadDecorate()
    {
        if (PlayerPrefs.HasKey("DecorateLocalData"))
        {
            string json = PlayerPrefs.GetString("DecorateLocalData");
            DecorateLocalData decorateLocalData = JsonUtility.FromJson<DecorateLocalData>(json);
            mouthList = decorateLocalData.mouthList ?? new List<DecorateLocalItem>();
            eyesList = decorateLocalData.eyesList ?? new List<DecorateLocalItem>();
            cheeksList = decorateLocalData.cheeksList ?? new List<DecorateLocalItem>();
        }
        else
        {
            mouthList = new List<DecorateLocalItem>();
            eyesList = new List<DecorateLocalItem>();
            cheeksList = new List<DecorateLocalItem>();
        }
        return this;
    }
    
    // 获取指定类型的装饰品列表
    public List<DecorateLocalItem> GetItemListByType(DecorateType type)
    {
        LoadDecorate();
        switch (type)
        {
            case DecorateType.Mouth:
                return mouthList;
            case DecorateType.Eyes:
                return eyesList;
            case DecorateType.Cheeks:
                return cheeksList;
            default:
                return new List<DecorateLocalItem>();
        }
    }
    
    // 添加装饰品到指定类型列表
    public void AddItem(DecorateLocalItem item, DecorateType type)
    {
        List<DecorateLocalItem> targetList = GetItemListByType(type);
        targetList.Add(item);
    }
}

[System.Serializable]
public class DecorateLocalItem
{
    public string uid;  // 物品唯一标识符
    public int id;  // 对应配置表中的id
    public int type;
    public int star;
    public int num;
    public bool isNew;

    public override string ToString()
    {
        return string.Format("[uid]:{0} [id]:{1} [type]:{2}", uid, id, type);
    }
}

// 装饰品类型枚举
public enum DecorateType
{
    Mouth = 0,
    Eyes = 1,
    Cheeks = 2
}