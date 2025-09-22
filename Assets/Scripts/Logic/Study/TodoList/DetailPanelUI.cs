using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DetailPanelUI : MonoBehaviour
{
    public TextMeshProUGUI keywords;
    public TextMeshProUGUI type;
    public TextMeshProUGUI importantUrgent;
    public TextMeshProUGUI creationTime;
    public TextMeshProUGUI completionTime;
    public TextMeshProUGUI rewardDaily;
    public TextMeshProUGUI process;
    public TextMeshProUGUI rewardTotal;
    public TextMeshProUGUI description;

    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        UpdatePosition();
    }

    internal void SetupDetail(TodoItem todoItem)
    {
        keywords.text = todoItem.keywords;
        type.text = todoItem.type;
        importantUrgent.text = (todoItem.isUrgent ? "紧急" : "非紧急") + (todoItem.isImportant ? "重要" : "非重要");
        creationTime.text = todoItem.creationTime.ToString("yyyy/MM/dd");
        completionTime.text = (todoItem.isCompleted) ? todoItem.completionTime.ToString("yyyy/MM/dd") : "进行";
        rewardDaily.text = todoItem.rewardDaily.ToString();
        // FIXME: 修改总次数
        process.text = todoItem.completedTimes + "/" + todoItem.period.Length;
        rewardTotal.text = todoItem.rewardTotal.ToString();
        // TODO: period的UI显示
        description.text = todoItem.description;
    }

    void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector3 mousepos = Mouse.current.position.ReadValue();
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        float width = corners[3].x - corners[0].x;
        float height = corners[1].y - corners[0].y;

        // 默认位置在鼠标右上方
        Vector3 targetPosition = mousepos + new Vector3(width * 0.6f, height * 0.6f, 0);

        // 检查右侧是否有足够空间
        if (mousepos.x + width > Screen.width)
        {
            // 右侧空间不足，显示在鼠标左侧
            targetPosition.x = mousepos.x - width * 0.6f;
        }

        // 检查上方是否有足够空间
        if (mousepos.y + height > Screen.height)
        {
            // 上方空间不足，显示在鼠标下方
            targetPosition.y = mousepos.y - height * 0.6f;
        }

        rectTransform.position = targetPosition;
    }
}
