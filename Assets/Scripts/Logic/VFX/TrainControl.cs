using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.UI;

public class TrainControl : MonoBehaviour
{
    private TrailRenderer[] trailRen;
    private CinemachineDollyCart dollyCart;
    private CinemachinePathBase smoothPath;
    public float defaultSpeed = 2f;
    public float waypointSpeed = 1.0f;
    public float speedChangeDuration = 0.5f;

    [Tooltip("触发速度变化的路径点索引")]
    public int[] waypointIndices = new int[] { 3 }; // 示例路径点索引
    private int lastWaypointIndex = -1;
    bool isTrainMoving = false;

    private void Awake()
    {
        trailRen = GetComponentsInChildren<TrailRenderer>();
        // 获取CinemachineDollyCart组件
        dollyCart = GetComponent<CinemachineDollyCart>();
        smoothPath = dollyCart.m_Path;
    }

    void Update()
    {
        if (!isTrainMoving) return;
        if (dollyCart != null && smoothPath != null)
        {
            // 获取当前路径百分比位置
            float currentPosition = dollyCart.m_Position;
            int currentWaypointIndex = Mathf.FloorToInt(currentPosition);

            // 检查是否经过了指定的路径点
            foreach (int waypointIndex in waypointIndices)
            {
                // 检查是否刚刚经过该路径点
                if (currentWaypointIndex >= waypointIndex && lastWaypointIndex != waypointIndex)
                {
                    // 触发速度变化
                    StartCoroutine(ChangeSpeedOverTime(waypointSpeed, speedChangeDuration));
                    lastWaypointIndex = waypointIndex;
                    break;
                }
            }

            // 更新上一个路径点索引
            lastWaypointIndex = currentWaypointIndex;
            // 拖尾的头随时间变细
            trailRen[0].textureScale += new Vector2(1, 1) * Time.deltaTime;
            trailRen[1].textureScale += new Vector2(1, 1) * Time.deltaTime;
        }

        if (lastWaypointIndex == 4) // 结束后重置
        {
            isTrainMoving = false;
            dollyCart.m_Speed = defaultSpeed;
            // dollyCart.m_Position = 4f;
            lastWaypointIndex = -1;
        }
    }

    // 平滑改变速度的方法
    private IEnumerator ChangeSpeedOverTime(float targetSpeed, float duration)
    {
        float startSpeed = dollyCart.m_Speed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // 使用平滑插值
            t = t * t * (3f - 2f * t); // SmoothStep

            dollyCart.m_Speed = Mathf.Lerp(startSpeed, targetSpeed, t);
            yield return null;
        }

        dollyCart.m_Speed = targetSpeed;

        // // 可选：一段时间后恢复默认速度
        // yield return new WaitForSeconds(2f);
        // StartCoroutine(ChangeSpeedOverTime(defaultSpeed, speedChangeDuration));
    }

    public void SetTrainMoving(bool isMoving)
    {
        this.isTrainMoving = isMoving;
        dollyCart.m_Speed = defaultSpeed;
        dollyCart.m_Position = isMoving ? 0f : 4f;
        trailRen[0].textureScale = new Vector2(1, 1);
        trailRen[1].textureScale = new Vector2(1, 1);
    }
}