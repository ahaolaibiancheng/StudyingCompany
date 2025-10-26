using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class VFXControl : MonoBehaviour
{
    [Header("Trail VFX")]
    public Transform trail;
    public Transform trailPath;
    public TrainControl trainControl;

    [Header("Rain VFX")]
    public Transform raindrop;
    private Material rainMaterial;

    private void Awake()
    {
        rainMaterial = raindrop.GetComponent<SpriteRenderer>().material;
    }

    public void DailyItemCompleted(Vector3 startPos, Vector3 endPos)
    {
        // 将画布坐标转换为世界坐标
        startPos = Camera.main.ScreenToWorldPoint(startPos);
        endPos = Camera.main.ScreenToWorldPoint(endPos);

        // 设置起点终点位置
        trailPath.GetComponent<CinemachineSmoothPath>().m_Waypoints[0].position = startPos;
        // trailPath.GetComponent<CinemachineSmoothPath>().m_Waypoints[4].position = endPos;

        trainControl.SetTrainMoving(true);
    }

    public void LightRainDrop()
    {
        rainMaterial.SetFloat("_DynamicDrops", 2.0f);
        rainMaterial.SetFloat("_StaticDrops", 0.5f);
    }

    public void Middlerain()
    {
        rainMaterial.SetFloat("_DynamicDrops", 3.5f);
        rainMaterial.SetFloat("_StaticDrops", 3.25f);
    }

    public void HeavyRain()
    {
        rainMaterial.SetFloat("_DynamicDrops", 5.0f);
        rainMaterial.SetFloat("_StaticDrops", 5.0f);
    }


}