using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCanvas : BasePanel
{
    public Button baseButton;
    public Button mouthButton;
    public Button eysButton;
    public Button cheeksButton;
    public Transform UIScrollView;
    public GameObject DecorateUIItemPrefab;
    public GameObject baseInfoPanel;
    public GameObject decoratePanel;

    void Start()
    {
        baseButton.onClick.AddListener(OnBaseBtnClicked);
        mouthButton.onClick.AddListener(OnMouthBtnClicked);
        eysButton.onClick.AddListener(OnEyeBtnClicked);
        cheeksButton.onClick.AddListener(OnCheeksBtnClicked);

    }
    private void OnBaseBtnClicked()
    {
        baseInfoPanel.SetActive(true);
        decoratePanel.SetActive(false);
    }

    private void OnMouthBtnClicked()
    {
        baseInfoPanel.SetActive(false);
        decoratePanel.SetActive(true);
        // 刷新物品
        RefreshScroll(DecorateType.Mouth);
    }

    private void OnCheeksBtnClicked()
    {
        baseInfoPanel.SetActive(false);
        decoratePanel.SetActive(true);
        // 刷新物品
        RefreshScroll(DecorateType.Cheeks);
    }

    private void OnEyeBtnClicked()
    {
        baseInfoPanel.SetActive(false);
        decoratePanel.SetActive(true);
        // 刷新物品
        RefreshScroll(DecorateType.Eyes);
    }

    private void RefreshScroll(DecorateType type)
    {
        // 清理滚动容器中原本的物品
        RectTransform scrollContent = UIScrollView.GetComponent<ScrollRect>().content;
        for (int i = 0; i < scrollContent.childCount; i++)
        {
            Destroy(scrollContent.GetChild(i).gameObject);
        }

        List<DecorateLocalItem> list = DecorateLocalData.Instance.GetItemListByType(type);
        foreach (DecorateLocalItem localData in list)
        {
            Transform DecorateUIItem = Instantiate(DecorateUIItemPrefab.transform, scrollContent) as Transform;
            DecorateCell decorateCell = DecorateUIItem.GetComponent<DecorateCell>();
            decorateCell.Refresh(localData, type);
        }
    }
}
