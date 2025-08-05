using System;
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
}
