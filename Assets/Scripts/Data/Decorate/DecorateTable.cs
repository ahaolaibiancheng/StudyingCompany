using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataTables/DecorateTable", fileName = "DecorateTable")]
public class DecorateTable : ScriptableObject, IDataTable<DecorateTableItem>
{
    public List<DecorateTableItem> decorateTableItems = new List<DecorateTableItem>();

    public void AddItem(DecorateTableItem item)
    {
        decorateTableItems.Add(item);
    }
}

[System.Serializable]
public class DecorateTableItem
{
    public int id;
    public int type;    // 用以区分上衣、裤子、鞋子等
    public int star;
    public int num;
    public string name;
    public string description;
    public string skillDescription;
    public string imagePath;
}