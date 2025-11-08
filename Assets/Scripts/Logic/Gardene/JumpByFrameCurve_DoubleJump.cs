using UnityEngine;

public class JumpByFrameCurve_DoubleJump : MonoBehaviour
{
    [Header("跳跃参数设置")]
    public float jumpHeight = 2f;           // 跳跃高度
    public float plantHeight = 1.5f;          // 高台的高度
    public float jumpDuration = 1f;         // 每段跳跃持续时间
    public float speedX = 1f;           // 横向运动的速率
    public AnimationCurve jumpCurve;        // 跳跃曲线(0~1 → 高度比例)
    public AnimationCurve fallCurve;        // 跳跃曲线(0~1 → 高度比例)

    [Header("状态")]
    private bool isJumping = false;         // 当前是否处于跳跃状态
    private bool isFalling = false;         // 是否在下落
    private float jumpTimer = 0f;

    private Vector3 startPos;
    private Vector3 apexPos;
    private Vector3 dropPos;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        startPos = transform.position;
    }

    void Update()
    {
        Jump();
    }

    void Jump()
    {
        if (isJumping)
        {
            jumpTimer += Time.deltaTime;
            float t = Mathf.Clamp01(jumpTimer / jumpDuration);
            float curveValue = jumpCurve.Evaluate(t);
            float targetY = Mathf.Lerp(startPos.y, apexPos.y, curveValue);
            transform.position = new Vector3(transform.position.x + Time.deltaTime * speedX, targetY, startPos.z);
        }
        else if (isFalling)
        {
            jumpTimer += Time.deltaTime;
            float t = Mathf.Clamp01(jumpTimer / jumpDuration);
            float curveValue = fallCurve.Evaluate(t);

            float targetY = Mathf.Lerp(apexPos.y, dropPos.y, curveValue);
            transform.position = new Vector3(transform.position.x + Time.deltaTime * speedX, targetY, startPos.z);
        }
    }

    // -------------------------------
    // 🧩 以下函数由动画帧事件(AnimationEvent)调用
    // -------------------------------

    // 第一段跳的起跳帧事件
    public void OnJumpStart()
    {
        Debug.Log("FrameEvent → OnJumpStart");
        isJumping = true;
        jumpTimer = 0f;
        startPos = transform.position;
        apexPos = startPos + Vector3.up * jumpHeight;
    }

    // 第一段跳最高点帧事件
    public void OnJumpApex()
    {
        Debug.Log("FrameEvent → OnJumpApex");
        isJumping = false;
        isFalling = true;
        jumpTimer = 0f;
        dropPos = startPos + Vector3.up * plantHeight;
    }

    // 落地帧事件（动画最后一帧）
    public void OnJumpEnd()
    {
        Debug.Log("FrameEvent → OnJumpEnd");
        isFalling = false;
    }
}
