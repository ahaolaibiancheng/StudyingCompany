using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace SleepTracker
{
    public class SleepDayRow : MonoBehaviour, IPointerClickHandler
    {
        [Header("Refs")]
        public TextMeshProUGUI LblDate;
        public RectTransform AxisTrack;
        public RectTransform Bar;
        public TextMeshProUGUI LblDuration;

        public Action<int> OnRowClicked; // 参数：day(1..31)

        private int _day; // 当前行对应的日

        const float TOTAL_HOURS = 12f; // 22:00 → 10:00

        public void Bind(DateTime date, int startQuarter, int endQuarter)
        {
            _day = date.Day;
            LblDate.text = $"{date:dd}日 {GetWeekdayCN(date)}";

            // 计算时长
            int quarters = Mathf.Max(0, endQuarter - startQuarter);
            var duration = TimeSpan.FromMinutes(quarters * 15);
            LblDuration.text = $"{Mathf.FloorToInt((float)duration.TotalHours)}h{duration.Minutes:00}m";

            // 转换为 [0,12] 小时上的百分比
            float startH = (startQuarter / 4f); // 1 quarter = 0.25h
            float endH = (endQuarter / 4f);
            startH = Mathf.Clamp(startH, 0f, TOTAL_HOURS);
            endH = Mathf.Clamp(endH, 0f, TOTAL_HOURS);

            float trackWidth = AxisTrack.rect.width;
            float pxPerHour = trackWidth / TOTAL_HOURS;

            float barX = startH * pxPerHour;
            float barW = Mathf.Max(2f, (endH - startH) * pxPerHour);

            // 设置 Bar
            var size = Bar.sizeDelta; size.x = barW; Bar.sizeDelta = size;
            var pos = Bar.anchoredPosition; pos.x = barX; Bar.anchoredPosition = pos;

            // 睡眠时长颜色（可按需调整）
            var img = Bar.GetComponent<Image>();
            if (img != null)
            {
                if (duration.TotalHours >= 10) img.color = Color.yellow;
                else if (duration.TotalHours >= 8) img.color = Color.blue;
                else if (duration.TotalHours >= 6) img.color = Color.green;
                else img.color = Color.red;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnRowClicked?.Invoke(_day);
        }

        private static string GetWeekdayCN(DateTime d)
        {
            string[] map = { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
            return map[(int)d.DayOfWeek];
        }
    }
}
