using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DecorateCell : MonoBehaviour , IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Transform UIIcon;
    private Transform UIHead;
    private Transform UINew;
    public Transform UISelect;
    public Transform UILevel;

    public Transform UIStars;
    private Transform UIDeleteSelect;

    private Transform UISelectAni;
    private Transform UIMouseOverAni;

    private DecorateLocalItem decorateLocalData;
    private DecorateTableItem decorateTableItem;

    private void Awake()
    {
        InitUIName();
    }
    private void InitUIName()
    {
        // UIIcon = transform.Find("Top/Icon");
        // // UIHead = transform.Find("Top/Head");
        // // UINew = transform.Find("Top/New");
        // UILevel = transform.Find("Bottom/LevelText");
        // UIStars = transform.Find("Bottom/Stars");
        // UISelect = transform.Find("Select");
        // UIDeleteSelect = transform.Find("DelectSelect");
        // // UIMouseOverAni = transform.Find("MouseOverAni");
        // // UISelectAni = transform.Find("SelectAni");

        // // UIDeleteSelect.gameObject.SetActive(false);
        // // UIMouseOverAni.gameObject.SetActive(false);
        // // UISelectAni.gameObject.SetActive(false);
    }

    public void Refresh(DecorateLocalItem decorateLocalData, DecorateType type)
    {
        // 数据初始化
        this.decorateLocalData = decorateLocalData;
        this.decorateTableItem = DecorateController.Instance.GetDecorateItemById(type, decorateLocalData.id);
        // 等级信息
        UILevel.GetComponent<Text>().text = "Lv." + this.decorateLocalData.star.ToString();
        // 是否是新获得？
        // UINew.gameObject.SetActive(this.decorateLocalData.isNew);
        // 物品的图片
        UIIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>(this.decorateTableItem.imagePath);
        // 刷新星级
        RefreshStars();
    }
    public void RefreshStars()
    {
        for (int i = 0; i < UIStars.childCount; i++)
        {
            Transform star = UIStars.GetChild(i);
            if (this.decorateTableItem.star > i)
            {
                star.gameObject.SetActive(true);
            }
            else
            {
                star.gameObject.SetActive(false);
            }
        }
    }
    // public void RefreshDeleteState()
    // {
    //     if (this.uiParent.deleteChooseUid.Contains(this.decorateLocalData.uid))
    //     {
    //         this.UIDeleteSelect.gameObject.SetActive(true);
    //     }
    //     else
    //     {
    //         this.UIDeleteSelect.gameObject.SetActive(false);

    //     }
    // }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log("OnPointerClick: " + eventData.ToString());
        DecorateEventHandler.CallDecorateChooseUIDChanged(this.decorateLocalData.uid);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("OnPointerEnter: " + eventData.ToString());
        // UIMouseOverAni.gameObject.SetActive(true);
        // UIMouseOverAni.GetComponent<Animator>().SetTrigger("In");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("OknPointerExit: " + eventData.ToString());
        // UIMouseOverAni.GetComponent<Animator>().SetTrigger("Out");
    }

    // public void OnSelectAniInCb()
    // {
    //     UISelectAni.gameObject.SetActive(false);
    // }
    // public void OnMouseOverAniOutCb()
    // {
    //     UIMouseOverAni.gameObject.SetActive(false);
    // }
}
