using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class ShowItemUI : TodoBaseItem
{
    // Drag and drop variables
    private RectTransform parentRect;
    private int startSiblingIndex;
    private GameObject placeholder;
    // private bool isDragging = false;
    // private bool isOverDeleteArea = false;
    private RectTransform deleteAreaRect;

    protected override void OnToggleChanged(bool isCompleted)
    {
        todoBaseItem.isCompleted = isCompleted;
        TodoListEventHandler.CallTodoItemCompleted(todoBaseItem);
    }
}
