using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{
    private Vector3 mouseWorldPos
    {
        get
        {
            // // 基本检查
            // if (Camera.main == null) return Vector3.zero;
            
            // Vector3 mousePos = Input.mousePosition;
            
            // // 简单的有效性检查
            // if (mousePos.x <= 0 && mousePos.y <= 0) return Vector3.zero;
            // if (float.IsNaN(mousePos.x) || float.IsInfinity(mousePos.x)) return Vector3.zero;
            // if (float.IsNaN(mousePos.y) || float.IsInfinity(mousePos.y)) return Vector3.zero;

            // try
            // {
            //     Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
            //     // 检查转换后的世界坐标是否有效
            //     if (float.IsNaN(worldPos.x) || float.IsNaN(worldPos.y) || float.IsInfinity(worldPos.x) || float.IsInfinity(worldPos.y))
            //     {
            //         return Vector3.zero;
            //     }
            //     return worldPos;
            // }
            // catch
            // {
                return Vector3.zero;
            // }
        }
    }
    private bool canClick;

    // public RectTransform hand;
    // private ItemName currentItemName;
    // private bool isHoldingItem;

    // private void OnEnable()
    // {
    //     EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
    //     EventHandler.ItemUsedEvent += OnItemUsedEvent;
    // }

    // private void OnDisable()
    // {
    //     EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
    //     EventHandler.ItemUsedEvent -= OnItemUsedEvent;
    // }

    // private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    // {
    //     isHoldingItem = isSelected;
    //     hand.gameObject.SetActive(isHoldingItem);
    //     if (isSelected)
    //     {
    //         currentItemName = itemDetails.itemName;
    //     }
    // }

    // private void OnItemUsedEvent(ItemName name)
    // {
    //     currentItemName = ItemName.None;
    //     isHoldingItem = false;
    //     hand.gameObject.SetActive(false);
    // }

    private Collider2D GetObjectWorldPosition()
    {
        Vector3 pos = mouseWorldPos;
        if (pos == Vector3.zero) return null;
        
        return Physics2D.OverlapPoint(pos);
    }

    private void ClickAction(GameObject clickObject)
    {
        switch (clickObject.tag)
        {
            case "Teleport":
                var teleport = clickObject.GetComponent<TelePort>();
                teleport?.TeleportToScene();
                break;
            // case "Item":
            //     var item = clickObject.GetComponent<Item>();
            //     item?.ItemClicked();
            //     break;
            // case "Interactive":
            //     var interactive = clickObject.GetComponent<Interactive>();
            //     if (isHoldingItem)
            //         interactive?.CheckItem(currentItemName);
            //     else
            //         interactive?.OnEmptyAction();
            //     break;
            case "Egg":
                var egg = clickObject.GetComponent<Egg>();
                egg?.OnEggIsTriggered();
                break;
            case "Tomato":
                var todoListUI = clickObject.GetComponent<Egg>();
                todoListUI?.OnTomatoIsTriggered();
                break;
        }
    }

    public void Update()
    {
        canClick = GetObjectWorldPosition();    // 隐式转换，对象为空为false
        if (canClick && (Input.GetMouseButtonDown(0) || 
                        (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
        {
            ClickAction(GetObjectWorldPosition().gameObject);
        }

        // if (InteractWithUI()) return;   // 检测鼠标是否处于UI上，若是，则不响应点击事件

        // if (hand.gameObject.activeInHierarchy)
        // {
        //     hand.position = Input.mousePosition;
        // }
    }

    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;
        else
            return false;
    }
}