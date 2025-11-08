using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecorateDetail : MonoBehaviour
{
    private Transform UIStars;
    private Transform UIDescription;
    private Transform UIIcon;
    private Transform UITitle;
    private Transform UILevelText;
    private Transform UISkillDescription;

    private DecorateLocalData decorateLocalData;
    private DecorateTableItem decorateTableItem;
    // private PackagePanel uiParent;

    private void Awake()
    {
        InitUIName();
    }
    private void InitUIName()
    {
        UIStars = transform.Find("Center/Stars");
        UIDescription = transform.Find("Center/Description");
        UIIcon = transform.Find("Center/Icon");
        UITitle = transform.Find("Top/Title");
        // UILevelText = transform.Find("Bottom/LevelPnl/LevelText");
        UISkillDescription = transform.Find("Bottom/Description");
    }

    // public void Refresh(DecorateLocalItem packageLocalData, PackagePanel uiParent)
    // {
    // // 初始化：动态数据、静态数据、父物品逻辑
    // this.decorateLocalData = packageLocalData;
    // this.decorateTableItem = DecorateController.Instance.GetDecorateItemById(packageLocalData.id);
    // this.uiParent = uiParent;
    // // 等级
    // // UILevelText.GetComponent<Text>().text = string.Format("Lv.{0}/40", this.packageLocalData.level.ToString());
    // // 简短描述
    // UIDescription.GetComponent<Text>().text = this.packageTableItem.description;
    // // 详细描述
    // UISkillDescription.GetComponent<Text>().text = this.packageTableItem.skillDescription;
    // // 物品名称
    // UITitle.GetComponent<Text>().name = this.packageTableItem.name;
    // // 图片加载
    // UIIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>(this.packageTableItem.imagePath);
    // // 星级处理
    // RefreshStars();
    // }
    public void RefreshStars()
    {
        // for (int i = 0; i < UIStars.childCount; i++)
        // {
        //     Transform star = UIStars.GetChild(i);
        //     if (this.packageTableItem.star > i)
        //     {
        //         star.gameObject.SetActive(true);
        //     }
        //     else
        //     {
        //         star.gameObject.SetActive(false);
        //     }
        // }
    }
}