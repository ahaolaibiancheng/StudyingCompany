using UnityEngine;
using UnityEngine.UI;

public class StudyUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Text sessionTimeText;
    public Text remainingTimeText;
    public Button pauseButton;
    public Button continueButton;
    // public Button magnifierButton; // 放大镜控制按钮
    // public Image magnifierIcon;    // 放大镜图标（可选视觉反馈）

    // [Header("Magnifier Settings")]
    // public ScreenMagnifier screenMagnifier; // 引用放大镜组件
    private TaskPanel taskPanel;

    private void OnEnable()
    {
        EventHandler.StudyTimeUpdatedEvent += OnStudyTimeUpdatedEvent;
    }

    private void OnDisable()
    {
        EventHandler.StudyTimeUpdatedEvent -= OnStudyTimeUpdatedEvent;
    }

    private void Start()
    {
        OnStudyTimeUpdatedEvent(TaskSystem.Instance.remainingStudyTime);

        pauseButton.onClick.AddListener(OnPauseButtonClicked);
        continueButton.onClick.AddListener(OnContinueButtonClicked);
        // magnifierButton.onClick.AddListener(ToggleMagnifier);

        continueButton.gameObject.SetActive(false);
        // // 初始状态：放大镜关闭
        // if (screenMagnifier != null)
        // {
        //     screenMagnifier.ToggleMagnifier(false);
        //     UpdateMagnifierIcon(false);
        // }
    }
    private void OnStudyTimeUpdatedEvent(float remainingTime)
    {
        // Format remaining time as minutes:seconds
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        remainingTimeText.text = $"Remaing: {minutes:00}:{seconds:00}";

        // Update session time display
        int sessionMinutes = Mathf.FloorToInt(TaskSystem.Instance.currentSessionTime / 60);
        sessionTimeText.text = $"Session: {sessionMinutes} min";
    }

    private void OnPauseButtonClicked()
    {
        TaskSystem.Instance.PauseStudy();
        pauseButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);
    }

    private void OnContinueButtonClicked()
    {
        TaskSystem.Instance.ResumeStudy();
        continueButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
    }

    // // 放大镜开关功能
    // private void ToggleMagnifier()
    // {
    //     if (screenMagnifier == null) return;

    //     bool newState = !screenMagnifier.gameObject.activeSelf;
    //     screenMagnifier.ToggleMagnifier(newState);
    //     UpdateMagnifierIcon(newState);
    // }

    // // 更新放大镜图标状态
    // private void UpdateMagnifierIcon(bool isActive)
    // {
    //     if (magnifierIcon != null)
    //     {
    //         magnifierIcon.color = isActive ? Color.white : new Color(1, 1, 1, 0.5f);
    //     }
    // }
}
