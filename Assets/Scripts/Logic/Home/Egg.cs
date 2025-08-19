using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Egg : MonoBehaviour
{
    [SerializeField]
    private bool isTriggered = false;

    public void OnEggIsTriggered()
    {
        isTriggered = !isTriggered;

        Transform eggPanel = GameObject.Find("EggPanel").transform;
        if (eggPanel == null) return;

        eggPanel.transform.GetChild(0).gameObject.SetActive(isTriggered);
    }

    public void OnTomatoIsTriggered()
    {
        Transform canvas = GameObject.Find("TomatolCanvas").transform;
        if (canvas == null) return;

        canvas.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void OnTodoListIsTriggered()
    {
        Transform canvas = GameObject.Find("TodoListCanvas").transform;
        if (canvas == null) return;

        canvas.transform.GetChild(0).gameObject.SetActive(true);
    }
}