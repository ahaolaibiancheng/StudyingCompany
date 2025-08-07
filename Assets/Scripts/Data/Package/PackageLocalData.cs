// 动态的、核心数据
using UnityEngine;
using System.Collections.Generic;

public class PackageLocalData
{
    private static PackageLocalData _instance;

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


    public List<PackageLocalItem> packageLocalItems;

    public void SavePackage()
    {
        string inventoryJson = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("PackageLocalData", inventoryJson);
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
            string inventoryJson = PlayerPrefs.GetString("PackageLocalData");
            PackageLocalData packageLocalData = JsonUtility.FromJson<PackageLocalData>(inventoryJson);
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
    public string uid;  //物品唯一标识符
    public int id;  //物品ID（对应配置表中的ID）
    public int num;
    public int level;
    public bool isNew;

    public override string ToString()
    {
        return string.Format("[id]:{0} [num]:{1}", id, num);
    }
}