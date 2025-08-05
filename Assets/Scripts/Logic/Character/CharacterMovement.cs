using UnityEngine;
using UnityEngine.EventSystems; // For UI detection

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Animator animator;
    
    // Direction parameters (0=front, 1=right, 2=back)
    private int direction = 0;
    
    // State parameters (0=idle, 1=sit, 2=walk)
    private int state = 0;
    
    private Vector2 targetPosition;
    private bool isMoving = false;
    private SpriteRenderer spriteRenderer;
    
    private void Start()
    {
        // Get the SpriteRenderer component for flipping
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        HandleInput();
        MoveCharacter();
    }
    
    private void HandleInput()
    {
        // Skip input handling if pointer is over UI
        if (IsPointerOverUI())
        {
            Debug.Log("Input blocked by UI element");
            return;
        }
        
        // Handle mouse input
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse input detected - not over UI");
            SetTargetPosition(Input.mousePosition);
        }
        
        // Handle touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log("Touch input detected - not over UI");
                SetTargetPosition(touch.position);
            }
        }
    }
    
    // More robust UI detection that works with all input types
    private bool IsPointerOverUI()
    {
        // Check if EventSystem exists
        if (EventSystem.current == null)
        {
            Debug.LogError("EventSystem is missing in the scene!");
            return false;
        }
        
        // Check for mouse over UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Mouse pointer over UI element");
            return true;
        }
        
        // Additional check for touch input
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                {
                    Debug.Log($"Touch {i} over UI element (finger ID: {Input.GetTouch(i).fingerId})");
                    return true;
                }
            }
        }
        
        return false;
    }

    private void SetTargetPosition(Vector3 screenPosition)
    {
        // Convert screen position to world position
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        targetPosition = new Vector2(worldPosition.x, worldPosition.y);
        isMoving = true;
        
        // Debug log to verify input handling
        Debug.Log($"Set target position: {worldPosition}");
    }
    
    private void MoveCharacter()
    {
        if (!isMoving) return;
        
        // Move towards target using Vector2
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 newPosition = Vector2.MoveTowards(currentPos, targetPosition, moveSpeed * Time.deltaTime);
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        
        // Update animation based on direction
        Vector2 moveDirection = (targetPosition - currentPos).normalized;
        UpdateAnimation(moveDirection);
        
        // Check if reached target with a smaller threshold
        if (Vector2.Distance(currentPos, targetPosition) < 0.01f)
        {
            isMoving = false;
            PlayIdleAnimation();
        }
        else
        {
            // Ensure we're always in walk state while moving
            if (state != 2)
            {
                state = 2;
                PlayAnimation();
            }
        }
    }
    
    private void UpdateAnimation(Vector2 moveDirection)
    {
        if (moveDirection == Vector2.zero) return;
        
        // Calculate animation blend based on direction
        float horizontal = moveDirection.x;
        float vertical = moveDirection.y;
        
        // Handle diagonal movement first
        if (Mathf.Abs(horizontal) > 0.3f && Mathf.Abs(vertical) > 0.3f)
        {
            // Diagonal movement
            if (vertical > 0) 
            {
                // Up direction
                if (horizontal > 0) 
                {
                    // Up-right (back direction)
                    direction = 2;
                    FlipSprite(false);
                }
                else 
                {
                    // Up-left (back direction with flip)
                    direction = 2;
                    FlipSprite(true);
                }
            }
            else 
            {
                // Down direction
                if (horizontal > 0) 
                {
                    // Down-right (front direction)
                    direction = 0;
                    FlipSprite(false);
                }
                else 
                {
                    // Down-left (front direction with flip)
                    direction = 0;
                    FlipSprite(true);
                }
            }
        }
        else
        {
            // Non-diagonal movement
            if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
            {
                // Horizontal movement dominates
                if (horizontal > 0) 
                {
                    // Moving right
                    direction = 1;
                    FlipSprite(false);
                }
                else 
                {
                    // Moving left (right direction with flip)
                    direction = 1;
                    FlipSprite(true);
                }
            }
            else
            {
                // Vertical movement dominates
                FlipSprite(false);
                if (vertical > 0) 
                {
                    // Moving up (back direction)
                    direction = 2;
                }
                else 
                {
                    // Moving down (front direction)
                    direction = 0;
                }
            }
        }
        
        // Set state to walk (2)
        state = 2;
        PlayAnimation();
    }
    
    private void FlipSprite(bool flipX)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = flipX;
        }
    }
    
    private void PlayAnimation()
    {
        // Map direction and state to animation names
        string[] directionNames = {"front", "right", "back"};
        string[] stateNames = {"Idle", "Sit", "Walk"};
        
        string animationName = $"{directionNames[direction]}_{stateNames[state]}";
        
        if (animator != null)
        {
            // Try playing the animation with layer index -1 (base layer) and normalized time 0
            // Debug.Log($"PlayAnimation: {animationName}");
            animator.Play(animationName, -1, 0f);
        }
    }
    
    private void PlayIdleAnimation()
    {
        // Set state to idle (0)
        state = 0;
        PlayAnimation();
    }
    
    // Public method to trigger sitting animation
    public void Sit()
    {
        // Set state to sit (1)
        state = 1;
        isMoving = false;
        PlayAnimation();
    }
    
    // Public method to trigger standing up
    public void StandUp()
    {
        // Set state to idle (0)
        state = 0;
        isMoving = false;
        PlayAnimation();
    }
    
    // Automatically sit when entering chair trigger and align centers
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("FrontThing") || other.CompareTag("BackThing"))
        {
            // Stop any ongoing movement
            isMoving = false;
            
            // Align character position with collider center
            Vector3 colliderCenter = other.bounds.center;
            transform.position = new Vector3(colliderCenter.x, colliderCenter.y, transform.position.z);
            
            // Set appropriate direction based on tag
            if (other.CompareTag("FrontThing"))
            {
                direction = 0;  // Front
            }
            else if (other.CompareTag("BackThing"))
            {
                direction = 2;  // Back
            }
            
            Sit();
        }
    }
    
    // Automatically stand up when exiting chair trigger
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("FrontThing") || other.CompareTag("BackThing"))
        {
            StandUp();
        }
    }
}
