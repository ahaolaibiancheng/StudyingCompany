using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animation))]
public class Plant : MonoBehaviour
{
    public float waterAmount;
    public float maxWater;

    private void Start()
    {
        waterAmount = 100f;
        maxWater = 100f;
    }
    void Update()
    {
        waterAmount -= Time.deltaTime;
    }

    void playWaterAni()
    {
        if (GetComponent<Animation>().GetClip("Water") == null)
        {
            Debug.Log("没有找到Water动画");
            return;
        }
        GetComponent<Animation>().Play("Water");
    }

    void playClickedAni()
    {
        if (GetComponent<Animation>().GetClip("Clicked") == null)
        {
            Debug.Log("没有找到Clicked动画");
            return;
        }
        GetComponent<Animation>().Play("Clicked");
    }

}

public class PlantDetail
{
    public string name;
    public string description;
    public string icon;
    public string water;
    public string grow;
    public string harvest;
    public string seed;
    public string seedAmount;
    public string seedPrice;
    public string seedLevel;
}
