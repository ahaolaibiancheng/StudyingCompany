using System;
using System.Collections.Generic;
using UnityEngine;

namespace SleepTracker
{
    // 以 15 分钟为单位：一格 = 15min；0 表示 22:00；48 表示次日 10:00
    [Serializable]
    public struct SleepEntry
    {
        public int year;
        public int month;
        public int day;            // 所属“入睡当日”
        public int startQuarter;   // [0..48]
        public int endQuarter;     // [0..48]

        public DateTime Date => new DateTime(year, month, day);

        public static readonly TimeSpan WindowStart = new TimeSpan(22, 0, 0); // 22:00
        public static readonly TimeSpan WindowEnd = new TimeSpan(10, 0, 0); // 次日10:00

        public static int ClampQuarter(int q) => Mathf.Clamp(q, 0, 48);

        public static int TimeToQuarter(TimeSpan t)
        {
            // 将 22:00 作为 0；跨午夜到次日 10:00 = 48
            var delta = (t - WindowStart).TotalMinutes;
            if (delta < 0) delta += 24 * 60; // 跨日
            int q = Mathf.RoundToInt((float)(delta / 15.0));
            return ClampQuarter(q);
        }

        public static TimeSpan QuarterToTime(int quarter)
        {
            quarter = ClampQuarter(quarter);
            double minutes = quarter * 15.0;
            var ts = WindowStart + TimeSpan.FromMinutes(minutes);
            if (ts.TotalMinutes >= 24 * 60) ts -= TimeSpan.FromDays(1); // 还原到 0..24h 表
            return ts; // 注意：00:00~10:00 属于次日
        }

        public TimeSpan SleepStartTime() => QuarterToTime(startQuarter);
        public TimeSpan WakeEndTime() => QuarterToTime(endQuarter);

        public TimeSpan Duration()
        {
            // 以“0..48”窗口计算，总共 48 格
            int quarters = Mathf.Max(0, endQuarter - startQuarter);
            return TimeSpan.FromMinutes(quarters * 15);
        }
    }

    [Serializable]
    public class SleepMonthData
    {
        public int year;
        public int month;
        public List<SleepEntry> entries = new();

        public SleepMonthData() { }
        public SleepMonthData(int y, int m, int daysInMonth)
        {
            year = y; month = m;
            entries = new List<SleepEntry>(daysInMonth);
            for (int d = 1; d <= daysInMonth; d++)
            {
                entries.Add(new SleepEntry
                {
                    year = y,
                    month = m,
                    day = d,
                    startQuarter = 0, // 默认为 0~0：无记录
                    endQuarter = 0
                });
            }
        }

        public SleepEntry Get(int day) => entries[day - 1];
        public void Set(SleepEntry e) => entries[e.day - 1] = e;
    }

    public interface ISleepStorage
    {
        bool TryLoad(int year, int month, out SleepMonthData data);
        void Save(SleepMonthData data);
    }
}
