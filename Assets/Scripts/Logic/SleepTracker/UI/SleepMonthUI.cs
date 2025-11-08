using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SleepTracker
{
    public class SleepMonthUI : MonoBehaviour
    {
        [Header("Header")]
        public Button BtnPrev;
        public Button BtnNext;
        public TextMeshProUGUI TxtTitle;

        [Header("List")]
        public Transform Content;           // ScrollView/Content
        public SleepDayRow RowPrefab;

        [Header("Dialog")]
        public SleepEditDialog EditDialog;

        private ISleepStorage _storage;
        private SleepMonthModel _model;

        public int Year;
        public int Month;

        private void Awake()
        {
            if (Year <= 0 || Month <= 0)
            {
                var now = DateTime.Now;
                Year = now.Year;
                Month = now.Month;
            }

            _storage = new SleepStorageFile();
            _model = new SleepMonthModel(_storage, Year, Month);

            BtnPrev.onClick.AddListener(() => ChangeMonth(-1));
            BtnNext.onClick.AddListener(() => ChangeMonth(+1));

            Refresh();
        }

        private void ChangeMonth(int delta)
        {
            var dt = new DateTime(Year, Month, 1).AddMonths(delta);
            Year = dt.Year; Month = dt.Month;
            _model.Load(Year, Month);
            Refresh();
        }

        private void ClearRows()
        {
            for (int i = Content.childCount - 1; i >= 0; i--)
            {
                Destroy(Content.GetChild(i).gameObject);
            }
        }

        private void Refresh()
        {
            TxtTitle.text = $"{Year}年{Month:00}月 睡眠时间";
            ClearRows();

            int days = DateTime.DaysInMonth(Year, Month);
            for (int d = 1; d <= days; d++)
            {
                var entry = _model.Data.Get(d);
                var date = new DateTime(Year, Month, d);

                var row = Instantiate(RowPrefab, Content);
                row.Bind(date, entry.startQuarter, entry.endQuarter);
                row.OnRowClicked = OnRowClicked;
            }
        }

        private void OnRowClicked(int day)
        {
            var entry = _model.Data.Get(day);
            EditDialog.OnSave = (startQ, endQ) =>
            {
                _model.SetDayQuarters(day, startQ, endQ);
                _model.Save();   // 持久化
                Refresh();       // 立即刷新 UI
            };
            EditDialog.Open(Year, Month, day, entry.startQuarter, entry.endQuarter);
        }
    }
}
