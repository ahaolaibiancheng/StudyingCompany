using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReminderPanel : BasePanel
{
    public Text reminderMessage;

    public void ShowReminderMessage(string text)
    {
        reminderMessage.text = text;
    }
}
