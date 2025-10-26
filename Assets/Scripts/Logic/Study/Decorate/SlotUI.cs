using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("组件获取")]
    [SerializeField] private Image slotImage;
    [SerializeField] private TextMeshProUGUI amountText;
    public Image slotHightlight;
    [SerializeField] private Button button;

    [Header("格子类型")]
    public SlotType slotType;
    public bool isSelected;
    public int slotIndex;

    //物品信息
    public ItemDetails itemDetails;
    public int itemAmount;

    //     public InventoryLocation Location
    //     {
    //         get
    //         {
    //             return slotType switch
    //             {
    //                 SlotType.Bag => InventoryLocation.Player,
    //                 SlotType.Box => InventoryLocation.Box,
    //                 _ => InventoryLocation.Player
    //             };
    //         }
    //     }

    public InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

    private void Start()
    {
        isSelected = false;
        if (itemDetails == null)
        {
            UpdateEmptySlot();
        }
    }

    /// <summary>
    /// 更新格子UI和信息
    /// </summary>
    /// <param name="item">ItemDetails</param>
    /// <param name="amount">持有数量</param>
    public void UpdateSlot(ItemDetails item, int amount)
    {
        itemDetails = item;
        slotImage.sprite = item.itemIcon;
        slotImage.color = (amount > 0) ? Color.white : Color.black;
        itemAmount = amount;
        amountText.text = amount.ToString();
        slotImage.enabled = true;
        button.interactable = (amount > 0) ? true : false;
    }

    /// <summary>
    /// 将Slot更新为空
    /// </summary>
    public void UpdateEmptySlot()
    {
        if (isSelected)
        {
            isSelected = false;

            inventoryUI.UpdateSlotHightlight(slotType, -1);
            // EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
        }
        itemDetails = null;
        slotImage.enabled = false;
        amountText.text = string.Empty;
        button.interactable = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemDetails == null) return;
        isSelected = !isSelected;

        inventoryUI.UpdateSlotHightlight(slotType, slotIndex);

        // if (slotType == SlotType.Bag)
        // {
        //     //通知物品被选中的状态和信息
        //     EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
        // }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemAmount != 0)
        {
            inventoryUI.dragItem.enabled = true;
            inventoryUI.dragItem.sprite = slotImage.sprite;
            // inventoryUI.dragItem.SetNativeSize();

            isSelected = true;
            inventoryUI.UpdateSlotHightlight(slotType, slotIndex);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        inventoryUI.dragItem.transform.position = Mouse.current.position.ReadValue();
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        inventoryUI.dragItem.enabled = false;
        // Debug.Log(eventData.pointerCurrentRaycast.gameObject);

        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            // 鼠标触碰到非背包格子，返回
            if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() == null)
                return;

            var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
            int targetIndex = targetSlot.slotIndex;

            //在Player自身背包范围内交换
            // if (slotType == SlotType.Decoration && targetSlot.slotType == SlotType.Decoration)
            // {
            //     InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
            // }
            //             else if (slotType == SlotType.Shop && targetSlot.slotType == SlotType.Bag)  //买
            //             {
            //                 EventHandler.CallShowTradeUI(itemDetails, false);
            //             }
            //             else if (slotType == SlotType.Bag && targetSlot.slotType == SlotType.Shop)  //卖
            //             {
            //                 EventHandler.CallShowTradeUI(itemDetails, true);
            //             }
            //             else if (slotType != SlotType.Shop && targetSlot.slotType != SlotType.Shop && slotType != targetSlot.slotType)
            //             {
            //                 //跨背包数据交换物品
            //                 InventoryManager.Instance.SwapItem(Location, slotIndex, targetSlot.Location, targetSlot.slotIndex);
            //             }
            //清空所有高亮显示
            inventoryUI.UpdateSlotHightlight(slotType, -1);
        }
        else    //测试扔在地上
        {
            if (slotType == SlotType.Decoration && itemAmount > 0)
            {
                //鼠标对应世界地图坐标
                var mousePosition = Mouse.current.position.ReadValue();
                var pos = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -Camera.main.transform.position.z));

                EventHandler.CallInstantiateItemInScene(itemDetails.itemID, pos);
            }
        }
    }
}