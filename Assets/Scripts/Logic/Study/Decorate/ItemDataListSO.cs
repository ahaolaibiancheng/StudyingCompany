using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataTables/ItemDataListSO", fileName = "New ItemDataListSO")]
public class ItemDataListSO : ScriptableObject
{
    public List<ItemDetails> itemDetailsList;
}
