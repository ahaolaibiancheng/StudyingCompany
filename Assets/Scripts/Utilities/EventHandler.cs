using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }
    public static event Action AfterSceneloadEvent;
    public static void CallAfterSceneloadEvent()
    {
        AfterSceneloadEvent?.Invoke();
    }
    public static event Action<CharacterState> CharacterStateChangedEvent;
    public static void CallCharacterStateChangedEvent(CharacterState CharacterState)
    {
        CharacterStateChangedEvent?.Invoke(CharacterState);
    }

    public static event Action<float> StudyTimeUpdatedEvent;
    public static void CallStudyTimeUpdatedEvent(float CharacterState)
    {
        StudyTimeUpdatedEvent?.Invoke(CharacterState);
    }

    public static event Action StudyEndEvent;
    public static void CallStudyEndEvent()
    {
        StudyEndEvent?.Invoke();
    }

    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;
    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
    {
        UpdateInventoryUI?.Invoke(location, list);
    }

    public static event Action<int, Vector3> InstantiateItemInScene;
    public static void CallInstantiateItemInScene(int ID, Vector3 pos)
    {
        InstantiateItemInScene?.Invoke(ID, pos);
    }

    // public static event Action<ItemDetails, bool> ItemSelectedEvent;
    // public static void CallItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    // {
    //     ItemSelectedEvent?.Invoke(itemDetails, isSelected);
    // }
}

public static class ToolEventHandler
{
    // 番茄计时每个阶段开始学习时触发的事件
    public static event Action ToolFocusStartEvent;
    public static void CallToolFocusStartEvent()
    {
        ToolFocusStartEvent?.Invoke();
    }
    // 番茄计时每个阶段开始休息时触发的事件
    public static event Action ToolBreakStartEvent;
    public static void CallToolBreakStartEvent()
    {
        ToolBreakStartEvent?.Invoke();
    }
    public static event Action ToolCycleCompleteEvent;
    public static void CallToolCycleCompleteEvent()
    {
        ToolCycleCompleteEvent?.Invoke();
    }
    public static event Action ToolTimerCancelEvent;
    public static void CallToolTimerCancelEvent()
    {
        ToolTimerCancelEvent?.Invoke();
    }

}

public static class DecorateEventHandler
{
    public static event Action<string> DecorateChooseUIDChanged;
    public static void CallDecorateChooseUIDChanged(string uid)
    {
        DecorateChooseUIDChanged?.Invoke(uid);
    }

}
