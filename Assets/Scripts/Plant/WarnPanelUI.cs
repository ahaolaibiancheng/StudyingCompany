using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarnPanelUI : MonoBehaviour
{
    public void ShowWarning(PlantPot plantPot, int id)
    {
        var warn = transform.GetChild(id);
        bool active = plantPot.NeedsWatering || plantPot.NeedsPlanting;
        warn.GetComponent<Image>().color = plantPot.NeedsWatering ? Color.yellow : Color.red;
        warn.gameObject.SetActive(active);
    }

    internal void CloseWarning(PlantPot plantPot, int id)
    {
        transform.GetChild(id).gameObject.SetActive(false);
    }
}