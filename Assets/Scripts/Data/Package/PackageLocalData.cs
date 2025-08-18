// 动态的、核心数据
using UnityEngine;
using System.Collections.Generic;

public class PackageLocalData
{
    private static PackageLocalData _instance;
    public List<PackageLocalItem> packageLocalItems;

    public static PackageLocalData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PackageLocalData();
            }
            return _instance;
        }
    }

    public void SavePackage()
    {
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("PackageLocalData", json);
        PlayerPrefs.Save();        
    }

    public List<PackageLocalItem> LoadPackage()
    {
        if (packageLocalItems != null)
        {
            return packageLocalItems;
        }
        if (PlayerPrefs.HasKey("PackageLocalData"))
        {
            string json = PlayerPrefs.GetString("PackageLocalData");
            PackageLocalData packageLocalData = JsonUtility.FromJson<PackageLocalData>(json);
            packageLocalItems = packageLocalData.packageLocalItems;
            return packageLocalItems;
        }
        else
        {
            packageLocalItems = new List<PackageLocalItem>();
            return packageLocalItems;
        }
    }
}

[System.Serializable]
public class PackageLocalItem
{
    public string uid;  // 物品唯一标识符
    public int id;  // 物品ID（对应配置表中的ID）
    public int type;
    public int num;
    public int level;
    public bool isNew;

    public override string ToString()
    {
        return string.Format("[uid]:{0} [id]:{1} [num]:{2}", uid, id, num);
    }
}