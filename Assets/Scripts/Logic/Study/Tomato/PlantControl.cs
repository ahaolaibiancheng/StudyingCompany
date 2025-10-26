using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantControl : MonoBehaviour
{
    public GameObject PlantPanel;
    private Scrollbar water;

    private void Awake()
    {
        water = PlantPanel.transform.GetChild(0).GetComponent<Scrollbar>();
    }

    public void UpdateUI(Plant plant)
    {
        water.image.fillAmount = plant.waterAmount / plant.maxWater;
    }

    public void OpenPlantPanelUI()
    {
        PlantPanel.gameObject.SetActive(true);
    }

    public void OnPointerClickEvent(GameObject obj)
    {
        UpdateUI(obj.GetComponent<Plant>());
        // 播放动画
        obj.GetComponent<Animation>().Play("Plant_Open");
        OpenPlantPanelUI();

    }
}
