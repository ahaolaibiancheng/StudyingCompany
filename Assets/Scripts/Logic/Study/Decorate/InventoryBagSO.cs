using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataTables/InventoryBagSO", fileName = "New InventoryBagSO")]
public class InventoryBagSO : ScriptableObject
{
    public List<InventoryItem> itemList;
}