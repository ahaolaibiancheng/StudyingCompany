// 静态的、配置数据
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataTables/PackageTable", fileName = "PackageTable")]
public class PackageTable : ScriptableObject, IDataTable<PackageTableItem>
{
    public List<PackageTableItem> packageTableItems = new List<PackageTableItem>();
    
    public void AddItem(PackageTableItem item)
    {
        packageTableItems.Add(item);
    }
}

[System.Serializable]
public class PackageTableItem
{
    public int id;
    public int type;
    public int star;
    public int num;
    public string name;
    public string description;
    public string skillDescription;
    public string imagePath;
}
