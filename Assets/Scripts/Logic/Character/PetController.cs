using UnityEngine;

public class PetController : MonoBehaviour
{
    public static PetController Instance;
    private Animator animator;
    private GameManager gameManager;

    // Animation parameters
    private const string IDLE_PARAM = "IsIdle";
    private const string EATING_PARAM = "IsEating";
    private const string DRESSING_PARAM = "IsDressing";
    private const string REMINDING_PARAM = "IsReminding";

    private float reminderInterval = 300f; // 5 minutes in seconds
    private float lastReminderTime;
    private bool isReminding = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        animator = GetComponent<Animator>();
        gameManager = GameManager.Instance;
    }

    private void Start()
    {
        SetPetState(IDLE_PARAM);
    }

    private void OnEnable()
    {
        EventHandler.CharacterStateChangedEvent += OnCharacterStateChangedEvent;
        EventHandler.StudyTimeUpdatedEvent += OnStudyTimeUpdatedEvent;
    }

    private void OnDisable()
    {
        EventHandler.CharacterStateChangedEvent += OnCharacterStateChangedEvent;
        EventHandler.StudyTimeUpdatedEvent -= OnStudyTimeUpdatedEvent;
    }

    private void OnCharacterStateChangedEvent(CharacterState newState)
    {
        if (newState == CharacterState.Idle)
        {
            SetPetState(IDLE_PARAM);
        }
        else if (newState == CharacterState.Studying && isReminding)
        {
            // Stop reminding when study resumes
            SetPetState(IDLE_PARAM);
            isReminding = false;
        }
    }

    private void OnStudyTimeUpdatedEvent(float remainingTime)
    {
        // Trigger reminder when study time is up and not already reminding
        if (remainingTime <= 0 && !isReminding && gameManager.currentState == CharacterState.Studying)
        {
            TriggerReminder();
        }
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
        if (gameManager.currentState != CharacterState.Studying) return;

        SetPetState(REMINDING_PARAM);
        isReminding = true;
        lastReminderTime = Time.time;
    }

    // Called when user feeds the pet
    public void FeedPet()
    {
        if (gameManager.currentState == CharacterState.Resting)
        {
            SetPetState(EATING_PARAM);
        }
    }

    // Called when user dresses the pet
    public void DressPet()
    {
        if (gameManager.currentState == CharacterState.Resting)
        {
            SetPetState(DRESSING_PARAM);
        }
    }

    private void SetPetState(string stateParam)
    {
        // Reset all animation parameters
        animator.SetBool(IDLE_PARAM, false);
        animator.SetBool(EATING_PARAM, false);
        animator.SetBool(DRESSING_PARAM, false);
        animator.SetBool(REMINDING_PARAM, false);

        // Set the active state
        animator.SetBool(stateParam, true);
    }

    // Animation event handlers
    public void OnEatingComplete()
    {
        if (animator.GetBool(EATING_PARAM))
        {
            SetPetState(IDLE_PARAM);
        }
    }

    public void OnDressingComplete()
    {
        if (animator.GetBool(DRESSING_PARAM))
        {
            SetPetState(IDLE_PARAM);
        }
    }

    public void OnRemindingComplete()
    {
        if (animator.GetBool(REMINDING_PARAM))
        {
            SetPetState(IDLE_PARAM);
        }
    }
}
