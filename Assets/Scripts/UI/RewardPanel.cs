using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanel : BasePanel
{
    public Transform Icon;
    public Text LevelText;
    public Transform stars;
    public Button ConfirmBtn;
    private DecorateTable packageTable;
    private DecorateLocalItem packageLocalItem;

    private void OnEnable()
    {
        EventHandler.StudyEndEvent += OnStudyEndEvent;
    }

    private void OnDisable()
    {
        EventHandler.StudyEndEvent -= OnStudyEndEvent;
    }

    protected override void Awake()
    {
        // packageTable = DecorateController.Instance.GetDecorateTable();
    }

    private void Start()
    {
        ConfirmBtn.GetComponent<Button>().onClick.AddListener(OnClickConfirmBtn);
    }

    private void OnClickConfirmBtn()
    {
        ClosePanel();
    }

    private void OnStudyEndEvent()
    {
        // int index = UnityEngine.Random.Range(0, packageTable.packageTableItems.Count - 1);
        // PackageTableItem item = PackageController.Instance.GetPackageItemById(index);

        // Icon.GetComponent<Image>().sprite = Resources.Load<Sprite>(item.imagePath);
        // LevelText.GetComponent<Text>().text = "Lv." + item.star.ToString();

        // // 刷新星级
        // for (int i = 0; i < stars.childCount; i++)
        // {
        //     Transform star = stars.GetChild(i);
        //     if (item.star > i)
        //     {
        //         star.gameObject.SetActive(true);
        //     }
        //     else
        //     {
        //         star.gameObject.SetActive(false);
        //     }
        // }

        // // 存储
        // string uid = PackageController.Instance.FindUidByTableId(item.id);
        // packageLocalItem = PackageController.Instance.GetPackageLocalItemByUId(uid);
        // if (packageLocalItem != null)
        // {
        //     packageLocalItem.num++;
        // }
        // else
        // {
        //     packageLocalItem = new PackageLocalItem
        //     {
        //         uid = Guid.NewGuid().ToString(),
        //         id = item.id,
        //         num = 1,
        //         level = item.star,
        //         isNew = true
        //     };
        //     PackageLocalData.Instance.packageLocalItems.Add(packageLocalItem);
        // }
    }
}
