using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class DailyItemUI : BaseItem
{
    // Drag and drop variables
    private RectTransform parentRect;
    private int startSiblingIndex;
    private GameObject placeholder;
    // private bool isDragging = false;
    // private bool isOverDeleteArea = false;
    private RectTransform deleteAreaRect;

    private void OnToggleChanged(bool isCompleted)
    {
        // isCompleted必为true
        TodoListEventHandler.CallDailyItemCompleted(baseItem);
        Destroy(gameObject);
    }
}
