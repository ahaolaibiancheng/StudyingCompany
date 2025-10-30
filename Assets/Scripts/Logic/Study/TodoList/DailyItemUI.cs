using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using Unity.VisualScripting;

public class DailyItemUI : TodoBaseItem
{
    // Drag and drop variables
    private RectTransform parentRect;
    private int startSiblingIndex;
    private GameObject placeholder;
    // private bool isDragging = false;
    // private bool isOverDeleteArea = false;
    // private RectTransform deleteAreaRect;

    // 特效
    private VFXControl vfxControl;

    protected override void Awake()
    {
        base.Awake();
        vfxControl = FindObjectOfType<VFXControl>();
    }

    protected override void OnToggleChanged(bool isCompleted)
    {
        if (!isCompleted)
        {
            return;
        }

        vfxControl.DailyItemCompleted(transform.position, transform.position);
        TodoListEventHandler.CallDailyItemCompleted(todoBaseItem);
    }
}
