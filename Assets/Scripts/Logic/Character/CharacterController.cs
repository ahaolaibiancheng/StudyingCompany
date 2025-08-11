using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public static CharacterController Instance { get; private set; }

    private Animator animator;
    private TaskPanel taskPanel;

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
    }
    private void OnCharacterStateChangedEvent(CharacterState newState)
    {
    }

    // Called when user clicks "Start Studying" button
    public void OnStartStudyButton()
    {
        if (GameManager.Instance.currentState == CharacterState.Idle)
        {
            TaskManager.Instance.ConfirmTaskStart();
        }
    }

    // Called when user clicks "Pause" button during study
    public void OnPauseButton()
    {
        if (GameManager.Instance.currentState == CharacterState.Studying)
        {
            GameManager.Instance.SetCharacterState(CharacterState.Resting);
            Debug.Log($"OnPauseButton: Studying -> Resting");
        }
    }

    // Called when user clicks "Continue" button during rest
    public void OnContinueButton()
    {
        if (GameManager.Instance.currentState == CharacterState.Resting)
        {
            GameManager.Instance.SetCharacterState(CharacterState.Studying);
            Debug.Log($"OnContinueButton: Resting -> Studying");
        }
    }

    // Animation event handler for when rest animation completes
    public void OnRestAnimationComplete()
    {
        // Only trigger if still in resting state
        if (GameManager.Instance.currentState == CharacterState.Resting)
        {
            GameManager.Instance.SetCharacterState(CharacterState.Studying);
            Debug.Log($"OnRestAnimationComplete: Resting -> Studying");
        }
    }
}
