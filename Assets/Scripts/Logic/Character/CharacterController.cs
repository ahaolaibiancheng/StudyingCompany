using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public static CharacterController Instance { get; private set; }

    private Animator animator;
    private GameManager gameManager;
    private TaskPanel taskPanel;
    private TaskSystem taskSystem;

    // Animation parameters
    private const string IDLE_PARAM = "IsIdle";
    private const string STUDYING_PARAM = "IsStudying";
    private const string RESTING_PARAM = "IsResting";

    private void OnEnable()
    {
        EventHandler.CharacterStateChangedEvent += OnCharacterStateChangedEvent;
    }

    private void OnDisable()
    {
        EventHandler.CharacterStateChangedEvent += OnCharacterStateChangedEvent;
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        animator = GetComponent<Animator>();
        gameManager = GameManager.Instance;
        taskSystem = TaskSystem.Instance;
    }
    private void OnCharacterStateChangedEvent(CharacterState newState)
    {
        // Reset all animation parameters
        animator.SetBool(IDLE_PARAM, false);
        animator.SetBool(STUDYING_PARAM, false);
        animator.SetBool(RESTING_PARAM, false);

        // Set the appropriate animation state
        switch (newState)
        {
            case CharacterState.Idle:
                animator.SetBool(IDLE_PARAM, true);
                break;
            case CharacterState.Studying:
                animator.SetBool(STUDYING_PARAM, true);
                break;
            case CharacterState.Resting:
                animator.SetBool(RESTING_PARAM, true);
                break;
        }
    }

    // Called when user clicks "Start Studying" button
    public void OnStartStudyButton()
    {
        if (gameManager.currentState == CharacterState.Idle)
        {
            taskSystem.ConfirmTaskStart();
        }
    }

    // Called when user clicks "Pause" button during study
    public void OnPauseButton()
    {
        if (gameManager.currentState == CharacterState.Studying)
        {
            gameManager.SetCharacterState(CharacterState.Resting);
            Debug.Log($"OnPauseButton: Studying -> Resting");
        }
    }

    // Called when user clicks "Continue" button during rest
    public void OnContinueButton()
    {
        if (gameManager.currentState == CharacterState.Resting)
        {
            gameManager.SetCharacterState(CharacterState.Studying);
            Debug.Log($"OnContinueButton: Resting -> Studying");
        }
    }

    // Animation event handler for when rest animation completes
    public void OnRestAnimationComplete()
    {
        // Only trigger if still in resting state
        if (gameManager.currentState == CharacterState.Resting)
        {
            gameManager.SetCharacterState(CharacterState.Studying);
            Debug.Log($"OnRestAnimationComplete: Resting -> Studying");
        }
    }
}
