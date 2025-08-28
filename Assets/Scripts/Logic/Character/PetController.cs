using UnityEngine;
using System.Collections;

public class PetController : MonoBehaviour
{
    public static PetController Instance;
    private Animator animator;
    private GameManager gameManager;
    private float reminderInterval = 300f; // 5 minutes in seconds
    private float lastReminderTime;
    private bool isReminding = false;

    private const int CAT_IDLE_PARAM = 0;
    private const int CAT_EAT_PARAM = 1;
    private const int CAT_DRESS_PARAM = 2;
    private const int CAT_REMIND_PARAM = 3;
    private const int CAT_WALK_PARAM = 4;
    private const int CAT_READ_PARAM = 5;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        animator = GetComponent<Animator>();
        gameManager = GameManager.Instance;
        // reminderPanel should be assigned via Unity Inspector, not GetComponent
    }

    private void Start()
    {
        SetPetState(PetState.Reading);
    }

    private void OnEnable()
    {
        EventHandler.StudyTimeUpdatedEvent += OnStudyTimeUpdatedEvent;
        ToolEventHandler.ToolBreakStartEvent += () => StartCoroutine(OnFocusTimeEnd());
        ToolEventHandler.ToolCycleCompleteEvent += () => StartCoroutine(OnBreakTimeEnd());
    }

    private void OnDisable()
    {
        EventHandler.StudyTimeUpdatedEvent -= OnStudyTimeUpdatedEvent;
        ToolEventHandler.ToolBreakStartEvent -= () => StartCoroutine(OnFocusTimeEnd());
        ToolEventHandler.ToolCycleCompleteEvent -= () => StartCoroutine(OnBreakTimeEnd());
    }

    private void OnStudyTimeUpdatedEvent(float remainingTime)
    {
        // Trigger reminder when study time is up and not already reminding
        if (remainingTime <= 0 && !isReminding)
        {
            TriggerReminder();
        }
    }
    private IEnumerator OnFocusTimeEnd()
    {
        SetPetState(PetState.Reminding);

        UIManager.Instance.OpenPanel(UIConst.ReminderPanel);
        (UIManager.Instance.GetPanel(UIConst.ReminderPanel) as ReminderPanel)?.ShowReminderMessage("该休息了");  
        // 等待5秒后关闭面板
        yield return new WaitForSeconds(5);
        UIManager.Instance.ClosePanel(UIConst.ReminderPanel);

        yield return new WaitForSeconds(3f);
        SetPetState(PetState.Idle);
    }
    private IEnumerator OnBreakTimeEnd()
    {
        SetPetState(PetState.Reminding);

        UIManager.Instance.OpenPanel(UIConst.ReminderPanel);
        (UIManager.Instance.GetPanel(UIConst.ReminderPanel) as ReminderPanel)?.ShowReminderMessage("该学习了");  
        // 等待5秒后关闭面板
        yield return new WaitForSeconds(5);
        UIManager.Instance.ClosePanel(UIConst.ReminderPanel);

        yield return new WaitForSeconds(3f);
        SetPetState(PetState.Reading);
    }
    private void Update()
    {
        // Handle periodic reminders
        if (isReminding && Time.time - lastReminderTime >= reminderInterval)
        {
            TriggerReminder();
        }
    }


    public void TriggerReminder()
    {
        SetPetState(PetState.Reminding);
        isReminding = true;
        lastReminderTime = Time.time;
    }

    private void SetPetState(PetState state)
    {
        // Set the active state based on PetState
        Debug.Log("SetPetState: " + state);
        switch (state)
        {
            case PetState.Idle:
                animator.SetInteger("petState", CAT_IDLE_PARAM);
                break;
            case PetState.Eating:
                animator.SetInteger("petState", CAT_EAT_PARAM);
                break;
            case PetState.Dressing:
                animator.SetInteger("petState", CAT_DRESS_PARAM);
                break;
            case PetState.Reminding:
                animator.SetInteger("petState", CAT_REMIND_PARAM);
                break;
            case PetState.Walking:
                animator.SetInteger("petState", CAT_WALK_PARAM);
                break;
            case PetState.Reading:
                animator.SetInteger("petState", CAT_READ_PARAM);
                break;
        }
    }

    // Animation event handlers
    public void OnEatingCompleted()
    {
        if (animator.GetInteger("petState") != CAT_EAT_PARAM) return;
        SetPetState(PetState.Idle);
    }

    public void OnDressingCompleted()
    {
        if (animator.GetInteger("petState") != CAT_DRESS_PARAM) return;
        SetPetState(PetState.Idle);
    }

    public void OnRemindingCompleted()
    {
        if (animator.GetInteger("petState") != CAT_REMIND_PARAM) return;
        if (TomatoController.Instance?.IsFocusPhase() == true)
        {
            SetPetState(PetState.Reading);
        }
        else
        {
            SetPetState(PetState.Idle);
        }
    }
}
