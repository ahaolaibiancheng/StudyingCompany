using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public static CharacterController Instance { get; private set; }

    private Animator animator;
    private GameManager gameManager;

    // Animation parameters
    private const string IDLE_PARAM = "IsIdle";
    private const string STUDYING_PARAM = "IsStudying";
    private const string RESTING_PARAM = "IsResting";
    
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

        if (gameManager != null)
        {
            gameManager.OnGameStateChanged += HandleGameStateChange;
        }
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnGameStateChanged -= HandleGameStateChange;
        }
    }

    private void HandleGameStateChange(GameManager.GameState newState)
    {
        // Reset all animation parameters
        animator.SetBool(IDLE_PARAM, false);
        animator.SetBool(STUDYING_PARAM, false);
        animator.SetBool(RESTING_PARAM, false);

        // Set the appropriate animation state
        switch (newState)
        {
            case GameManager.GameState.Idle:
                animator.SetBool(IDLE_PARAM, true);
                break;
            case GameManager.GameState.Studying:
                animator.SetBool(STUDYING_PARAM, true);
                break;
            case GameManager.GameState.Resting:
                animator.SetBool(RESTING_PARAM, true);
                break;
        }
    }

    // Called when user clicks "Start Studying" button
    public void OnStartStudyButton()
    {
        if (gameManager.CurrentState == GameManager.GameState.Idle)
        {
            gameManager.ConfirmTaskStart();
        }
    }

    // Called when user clicks "Pause" button during study
    public void OnPauseButton()
    {
        if (gameManager.CurrentState == GameManager.GameState.Studying)
        {
            gameManager.SetGameState(GameManager.GameState.Resting);
            Debug.Log($"OnPauseButton: Studying -> Resting");
        }
    }

    // Called when user clicks "Continue" button during rest
    public void OnContinueButton()
    {
        if (gameManager.CurrentState == GameManager.GameState.Resting)
        {
            gameManager.SetGameState(GameManager.GameState.Studying);
            Debug.Log($"OnContinueButton: Resting -> Studying");
        }
    }

    // Animation event handler for when rest animation completes
    public void OnRestAnimationComplete()
    {
        // Only trigger if still in resting state
        if (gameManager.CurrentState == GameManager.GameState.Resting)
        {
            gameManager.SetGameState(GameManager.GameState.Studying);
            Debug.Log($"OnRestAnimationComplete: Resting -> Studying");
        }
    }
}
