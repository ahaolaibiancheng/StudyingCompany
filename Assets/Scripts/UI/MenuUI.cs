using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [System.Serializable]
    public class ButtonPanelPair
    {
        public Button button;
        public GameObject panel;
    }

    [Header("按钮与面板的映射（同索引对应）")]
    public List<ButtonPanelPair> items = new List<ButtonPanelPair>();

    [Tooltip("启动时默认打开的索引，-1 表示全部关闭")]
    public int defaultOpenIndex = -1;

    [Tooltip("再次点击已激活的按钮时是否关闭其面板")]
    public bool toggleOffOnSecondClick = true;

    private int _currentIndex = -1;

    private void Awake()
    {
        // 绑定按钮事件
        for (int i = 0; i < items.Count; i++)
        {
            int idx = i; // 捕获当前索引
            if (items[i] != null && items[i].button != null)
            {
                items[i].button.onClick.AddListener(() => OnButtonClicked(idx));
            }
        }
    }

    private void Start()
    {
        CloseAllPanels();

        if (defaultOpenIndex >= 0 && defaultOpenIndex < items.Count)
        {
            OpenPanel(defaultOpenIndex);
        }
    }

    private void OnButtonClicked(int index)
    {
        if (index < 0 || index >= items.Count) return;

        if (_currentIndex == index)
        {
            if (toggleOffOnSecondClick)
            {
                ClosePanel(index);
                _currentIndex = -1;
            }
            // 若不允许二次点击关闭，则保持现状
            return;
        }

        OpenPanel(index);
    }

    public void OpenPanel(int index)
    {
        if (index < 0 || index >= items.Count) return;

        // 只允许一个面板处于打开状态
        for (int i = 0; i < items.Count; i++)
        {
            SetPanelActive(i, i == index);
        }
        _currentIndex = index;
    }

    public void ClosePanel(int index)
    {
        if (index < 0 || index >= items.Count) return;
        SetPanelActive(index, false);
        if (_currentIndex == index) _currentIndex = -1;
    }

    public void CloseAllPanels()
    {
        for (int i = 0; i < items.Count; i++)
        {
            SetPanelActive(i, false);
        }
        _currentIndex = -1;
    }

    private void SetPanelActive(int index, bool active)
    {
        var pair = items[index];
        if (pair != null && pair.panel != null)
        {
            pair.panel.SetActive(active);
        }
    }
}

