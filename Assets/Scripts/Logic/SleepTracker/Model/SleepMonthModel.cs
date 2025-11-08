using System;
using UnityEngine;

namespace SleepTracker
{
    public class SleepMonthModel
    {
        private readonly ISleepStorage _storage;
        public SleepMonthData Data { get; private set; }

        public SleepMonthModel(ISleepStorage storage, int year, int month)
        {
            _storage = storage;
            Load(year, month);
        }

        public void Load(int year, int month)
        {
            if (!_storage.TryLoad(year, month, out var loaded))
            {
                int days = DateTime.DaysInMonth(year, month);
                loaded = new SleepMonthData(year, month, days);

                // // 也可生成 Demo 数据，便于预览：
                // var rnd = new System.Random(1024);
                // for (int d = 1; d <= days; d++)
                // {
                //     int startQ = 0 + rnd.Next(0, 12); // 22:00 ~ 01:00
                //     int endQ = 32 + rnd.Next(0, 8); // 06:00 ~ 08:00(含)
                //     var e = loaded.Get(d);
                //     e.startQuarter = startQ;
                //     e.endQuarter = endQ;
                //     loaded.Set(e);
                // }
                _storage.Save(loaded);
            }
            Data = loaded;
        }

        public void Save() => _storage.Save(Data);

        public void SetDayQuarters(int day, int startQ, int endQ)
        {
            startQ = SleepEntry.ClampQuarter(startQ);
            endQ = SleepEntry.ClampQuarter(endQ);
            if (endQ < startQ) endQ = startQ; // 简单防御：不允许负时长

            var e = Data.Get(day);
            e.startQuarter = startQ;
            e.endQuarter = endQ;
            Data.Set(e);
        }
    }
}
