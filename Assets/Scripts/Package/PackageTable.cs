// 静态的、配置数据
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PackageSystem/PackageTable", fileName = "PackageTable")]
public class PackageTable : ScriptableObject
{
    public List<PackageTableItem> DataList = new List<PackageTableItem>();
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