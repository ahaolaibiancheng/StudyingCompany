using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemDetails
{
    public int itemID;
    public string itemname;
    public InventoryItemType itemtype;
    public Sprite itemIcon;
    public string itemDescription;
    public int itemPrice;
    public float sellPercentage;
    // public int star;

    // public bool isNew;
}

// 背包数据
[Serializable]
public struct InventoryItem
{
    public int itemID;
    public int itemAmount;
}
