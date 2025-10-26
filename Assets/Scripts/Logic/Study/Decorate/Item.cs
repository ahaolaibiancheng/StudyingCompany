using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 世界里的物品
public class Item : MonoBehaviour
{
    public int itemID;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D coll;
    private ItemDetails itemDetail;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        coll = GetComponentInChildren<BoxCollider2D>();
    }

    private void Start()
    {
        if (itemID != 0)
        {
            Initialize(itemID);
        }
    }

    public void Initialize(int itemID)
    {
        this.itemID = itemID;
        itemDetail = InventoryManager.Instance.GetItemDetails(itemID);
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = itemDetail.itemIcon;
            // FIXME:重置coll的大小,考虑各种物理的碰撞体
            coll.size = itemDetail.itemIcon.bounds.size;
        }
    }
}
