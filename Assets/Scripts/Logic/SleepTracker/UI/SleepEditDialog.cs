using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SleepTracker
{
    public class SleepEditDialog : MonoBehaviour
    {
        [Header("Refs")]
        public CanvasGroup Group;
        public TextMeshProUGUI Title;
        public TMP_Dropdown StartDropdown;
        public TMP_Dropdown EndDropdown;
        public Button BtnCancel;
        public Button BtnSave;

        public Action<int, int> OnSave; // 回调：startQuarter, endQuarter

        private readonly List<string> _labels = new();
        private bool _isOpen = false;

        private void Awake()
        {
            BuildQuarterLabels();
            BindDropdowns();

            BtnCancel.onClick.AddListener(() => Close());
            BtnSave.onClick.AddListener(DoSave);

            Close(immediate: true);
        }

        private void BuildQuarterLabels()
        {
            _labels.Clear();
            // 0..48（包含终点），共 49 个刻度标签
            for (int q = 0; q <= 48; q++)
            {
                var ts = SleepEntry.QuarterToTime(q); // 22:00 起算（跨日自动还原）
                _labels.Add($"{(int)ts.TotalHours:00}:{ts.Minutes:00}");
            }
        }

        private void BindDropdowns()
        {
            StartDropdown.ClearOptions();
            EndDropdown.ClearOptions();
            StartDropdown.AddOptions(_labels);
            EndDropdown.AddOptions(_labels);
        }

        public void Open(int year, int month, int day, int startQuarter, int endQuarter)
        {
            Title.text = $"编辑 {month:00}/{day:00} 睡眠";
            StartDropdown.value = Mathf.Clamp(startQuarter, 0, 48);
            EndDropdown.value = Mathf.Clamp(endQuarter, 0, 48);

            _isOpen = true;
            Group.alpha = 1f;
            Group.blocksRaycasts = true;
            Group.interactable = true;
        }

        public void Close(bool immediate = false)
        {
            _isOpen = false;
            Group.alpha = 0f;
            Group.blocksRaycasts = false;
            Group.interactable = false;
        }

        private void DoSave()
        {
            int startQ = StartDropdown.value;
            int endQ = EndDropdown.value;
            if (endQ < startQ) endQ = startQ; // 简单校正；也可以弹 Toast

            OnSave?.Invoke(startQ, endQ);
            Close();
        }
    }
}
