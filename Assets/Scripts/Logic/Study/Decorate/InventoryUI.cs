using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    // public ItemTooltip itemTooltip;

    [Header("拖拽图片")]
    public Image dragItem;

    [Header("装饰背包")]
    [SerializeField] private GameObject decorateBag;
    [SerializeField] private List<SlotUI> decorateSlots;
    public GameObject decorateSlotPrefab;
    private bool decoratebagOpened;

    [Header("交易UI")]
    private TextMeshProUGUI buyprice;
    private TextMeshProUGUI sellprice;
    // public TradeUI tradeUI;
    // public TextMeshProUGUI playerMoneyText;

    private void OnEnable()
    {
        EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
        // EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadedEvent;
        // EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
        // EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;
        // EventHandler.ShowTradeUI += OnShowTradeUI;
    }

    private void OnDisable()
    {
        EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
        // EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadedEvent;
        // EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
        // EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
        // EventHandler.ShowTradeUI -= OnShowTradeUI;
    }


    private void Start()
    {
        decoratebagOpened = decorateBag.activeInHierarchy;
        // playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.B))
    //     {
    //         OpenBagUI();
    //     }
    // }

    // private void OnShowTradeUI(ItemDetails item, bool isSell)
    // {
    //     tradeUI.gameObject.SetActive(true);
    //     tradeUI.SetupTradeUI(item, isSell);
    // }

    /// <summary>
    /// 打开包裹UI事件
    /// </summary>
    /// <param name="slotType"></param>
    /// <param name="bagData"></param>
    private void OnBaseBagOpenEvent(SlotType slotType, InventoryBagSO bagData)
    {
        //生成背包UI
        decorateBag.SetActive(true);

        decorateSlots = new List<SlotUI>();

        for (int i = 0; i < bagData.itemList.Count; i++)
        {
            var slot = Instantiate(decorateSlotPrefab, decorateBag.transform.GetChild(1).GetComponent<ScrollRect>().content).GetComponent<SlotUI>();
            slot.slotIndex = i;
            decorateSlots.Add(slot);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(decorateBag.GetComponent<RectTransform>());

        // if (slotType == SlotType.Shop)
        // {
        //     decorateBag.GetComponent<RectTransform>().pivot = new Vector2(-1, 0.5f);
        //     decorateBag.SetActive(true);
        //     bagOpened = true;
        // }
        //更新UI显示
        OnUpdateInventoryUI(InventoryLocation.Decoration, bagData.itemList);
    }

    /// <summary>
    /// 关闭通用包裹UI事件
    /// </summary>
    /// <param name="slotType"></param>
    /// <param name="bagData"></param>
    private void OnBaseBagCloseEvent(SlotType slotType, InventoryBagSO bagData)
    {
        GameObject bag;
        List<SlotUI> bagSlots;
        switch (slotType)
        {
            case SlotType.Decoration:
                bag = decorateBag;
                bagSlots = decorateSlots;
                break;
            default:
                Debug.LogError("未支持的SlotType: " + slotType);
                return;
        }

        bag.SetActive(false);
        // itemTooltip.gameObject.SetActive(false);
        UpdateSlotHightlight(slotType, -1);

        foreach (var slot in bagSlots)
        {
            Destroy(slot.gameObject);
        }
        bagSlots.Clear();

        // if (slotType == SlotType.Shop)
        // {
        //     decorateBag.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        //     decorateBag.SetActive(false);
        //     bagOpened = false;
        // }
    }


    // private void OnBeforeSceneUnloadedEvent()
    // {
    //     UpdateSlotHightlight(-1);
    // }


    /// <summary>
    /// 更新指定位置的Slot事件函数
    /// </summary>
    /// <param name="location">库存位置</param>
    /// <param name="list">数据列表</param>
    private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
    {
        List<SlotUI> slotSlots = location switch
        {
            InventoryLocation.Decoration => decorateSlots,
            _ => null,
        };

        for (int i = 0; i < slotSlots.Count; i++)
        {
            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
            slotSlots[i].UpdateSlot(item, list[i].itemAmount);
        }

        // playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();
    }

    /// <summary>
    /// 打开关闭背包UI，Button调用事件
    /// </summary>
    public void OpenDecorateBagUI()
    {
        foreach (var slot in decorateSlots)
        {
            Destroy(slot.gameObject);
        }
        decorateSlots.Clear();

        OnBaseBagOpenEvent(SlotType.Decoration, InventoryManager.Instance.decorateBagSO);
        decoratebagOpened = !decoratebagOpened;
        decorateBag.SetActive(decoratebagOpened);
    }


    /// <summary>
    /// 更新Slot高亮显示
    /// </summary>
    /// <param name="index">序号</param>
    public void UpdateSlotHightlight(SlotType slotType, int index)
    {
        List<SlotUI> slotList = slotType switch
        {
            SlotType.Decoration => decorateSlots,
            _ => null,
        };

        if (slotList == null) return; // 添加空值检查

        foreach (var slot in slotList)
        {
            if (slot.isSelected && slot.slotIndex == index)
            {
                slot.slotHightlight.gameObject.SetActive(true);
            }
            else
            {
                slot.isSelected = false;
                slot.slotHightlight.gameObject.SetActive(false);
            }
        }
    }

}
