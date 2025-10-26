using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public ItemDataListSO itemDataSO;
    [Header("背包数据")]
    public InventoryBagSO decorateBagSO;

    [Header("交易")]
    public int playerMoney;

    private void Start()
    {
        // ISaveable saveable = this;
        // saveable.RegisterSaveable();
        playerMoney = 1000;
    }

    /// <summary>
    /// 通过ID获取物品详情
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    public ItemDetails GetItemDetails(int itemID)
    {
        return itemDataSO.itemDetailsList.Find(x => x.itemID == itemID);
    }

    /// <summary>
    /// 添加世界物品到背包中，或者是从商城中购买物品
    /// </summary>
    /// <param name="item"></param>
    /// <param name="isdestry">是否销毁物品</param>
    public void AddItem(Item item, bool isdestry)
    {
        int index = GetItemIndexInBag(item.itemID);

        AddItemAtIndex(item.itemID, index, 1);

        if (isdestry) Destroy(item.gameObject);
        //更新UI
        EventHandler.CallUpdateInventoryUI(InventoryLocation.Decoration, decorateBagSO.itemList);
    }

    /// <summary>
    /// 获取物品在背包中的位置（顺序），找不到则返回-1
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    private int GetItemIndexInBag(int itemID)
    {
        for (int i = 0; i < decorateBagSO.itemList.Count; i++)
        {
            if (decorateBagSO.itemList[i].itemID == itemID)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 在背包的指点位置添加物品
    /// </summary>
    /// <param name="id"></param>
    /// <param name="index"></param>
    /// <param name="amount"></param>
    private void AddItemAtIndex(int id, int index, int amount)
    {
        if (index == -1)    // 背包里没有
        {
            var item = new InventoryItem { itemID = id, itemAmount = amount };
            // TODO: 找到空位进行添加
        }
        else    //背包有这个物品
        {
            int currentAmount = decorateBagSO.itemList[index].itemAmount + amount;
            var item = new InventoryItem { itemID = id, itemAmount = currentAmount };

            decorateBagSO.itemList[index] = item;
        }
    }

    /// <summary>
    /// 移除指定数量的背包物品
    /// </summary>
    /// <param name="ID">物品ID</param>
    /// <param name="removeAmount">数量</param>
    private void RemoveItem(int ID, int removeAmount)
    {
        var index = GetItemIndexInBag(ID);

        if (decorateBagSO.itemList[index].itemAmount >= removeAmount)
        {
            var amount = decorateBagSO.itemList[index].itemAmount - removeAmount;
            var item = new InventoryItem { itemID = ID, itemAmount = amount };
            decorateBagSO.itemList[index] = item;
        }
        // else if (decorateBagSO.itemList[index].itemAmount == removeAmount)
        // {
        //     var item = new InventoryItem();
        //     decorateBagSO.itemList[index] = item;
        // }

        EventHandler.CallUpdateInventoryUI(InventoryLocation.Decoration, decorateBagSO.itemList);
    }

    /// <summary>
    /// decorate背包范围内交换物品
    /// </summary>
    /// <param name="fromIndex">起始序号</param>
    /// <param name="targetIndex">目标数据序号</param>
    public void SwapItem(int fromIndex, int targetIndex)
    {
        InventoryItem currentItem = decorateBagSO.itemList[fromIndex];
        InventoryItem targetItem = decorateBagSO.itemList[targetIndex];

        if (targetItem.itemID != 0)
        {
            decorateBagSO.itemList[fromIndex] = targetItem;
            decorateBagSO.itemList[targetIndex] = currentItem;
        }
        else
        {
            decorateBagSO.itemList[targetIndex] = currentItem;
            decorateBagSO.itemList[fromIndex] = new InventoryItem();
        }

        EventHandler.CallUpdateInventoryUI(InventoryLocation.Decoration, decorateBagSO.itemList);
    }

    /// <summary>
    /// 交易物品
    /// </summary>
    /// <param name="itemDetails">物品信息</param>
    /// <param name="amount">交易数量</param>
    /// <param name="isSellTrade">是否卖东西</param>
    public void TradeItem(ItemDetails itemDetails, int amount, bool isSellTrade)
    {
        int cost = itemDetails.itemPrice * amount;
        //获得物品背包位置
        int index = GetItemIndexInBag(itemDetails.itemID);

        if (isSellTrade)    //卖
        {
            if (decorateBagSO.itemList[index].itemAmount >= amount)
            {
                RemoveItem(itemDetails.itemID, amount);
                //卖出总价
                cost = (int)(cost * itemDetails.sellPercentage);
                playerMoney += cost;
            }
        }
        else if (playerMoney - cost >= 0)   //买
        {
            // if (CheckBagCapacity())
            // {
            // }
            AddItemAtIndex(itemDetails.itemID, index, amount);
            playerMoney -= cost;
        }
        //刷新UI
        EventHandler.CallUpdateInventoryUI(InventoryLocation.Decoration, decorateBagSO.itemList);
    }
}
