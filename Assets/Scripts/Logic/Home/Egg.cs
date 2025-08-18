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
        Transform configPanel = GameObject.Find("Canvas1").transform;
        if (configPanel == null) return;

        configPanel.transform.GetChild(0).gameObject.SetActive(true);
    }
}