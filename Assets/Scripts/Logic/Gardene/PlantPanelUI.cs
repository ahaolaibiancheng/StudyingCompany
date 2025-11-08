using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantPanelUI : MonoBehaviour
{
    public Transform exp;
    public Transform moisture;
    public Button plantBtn;
    public Button waterBtn;
    [HideInInspector] public PlantPot currentPlantPot;
    [HideInInspector] public bool isPanleOpen = false;

    public void SetCurrentPlantPot(PlantPot plantPot)
    {
        currentPlantPot = plantPot;
        if (currentPlantPot.IsPlanted == false)
        {
            plantBtn.gameObject.SetActive(true);
            waterBtn.gameObject.SetActive(false);
        }
        else
        {
            plantBtn.gameObject.SetActive(false);
            waterBtn.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        exp.GetComponent<Image>().fillAmount = currentPlantPot.currentExp / currentPlantPot.levelUpExpRequirements[currentPlantPot.currentLevel - 1];
        moisture.GetComponent<Image>().fillAmount = currentPlantPot.GetMoisturePercentage();
    }

    private void Start()
    {
        gameObject.SetActive(false);
        plantBtn.onClick.AddListener(() =>
        {
            currentPlantPot.RequestPlanting();
            plantBtn.gameObject.SetActive(false);
            waterBtn.gameObject.SetActive(true);
        });
        waterBtn.onClick.AddListener(() => { currentPlantPot.RequestWatering(); });
    }
}
