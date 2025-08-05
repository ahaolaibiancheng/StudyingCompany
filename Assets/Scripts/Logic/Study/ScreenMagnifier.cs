using UnityEngine;
using UnityEngine.UI;

public class ScreenMagnifier : MonoBehaviour
{
    [Header("Magnifier Settings")]
    public float magnification = 4.0f;  // 放大倍数
    public Vector2 magnifierSize = new Vector2(600, 800);   // 摄像头显示的图像的区域
    [Range(0.5f, 2f)] public float resolutionScale = 1.0f;
    
    [Header("Target Area Settings")]
    public Vector2 targetAreaPosition = new Vector2(1400, 600); // Display的位置
    public Vector2 targetAreaSize = new Vector2(4000, 4000);
    public Transform areaReference;

    [Header("Component References")]
    public Camera mainCamera;
    public Camera magnifierCamera;
    public RawImage magnifierDisplay;

    private RenderTexture renderTexture;
    private RectTransform magnifierRect;
    private bool isActive = true;

    void Start()
    {
        // 空引用检查
        if (mainCamera == null) mainCamera = Camera.main;
        if (magnifierCamera == null || magnifierDisplay == null)
        {
            Debug.LogError("ScreenMagnifier: Critical components missing!");
            enabled = false;
            return;
        }
        
        // 创建渲染纹理
        CreateRenderTexture();
        
        // 设置放大镜摄像机
        magnifierCamera.targetTexture = renderTexture;
        magnifierCamera.enabled = true;
        magnifierCamera.depth = mainCamera.depth + 10; // 使用固定深度偏移避免冲突
        
        // 设置UI显示
        magnifierDisplay.texture = renderTexture;
        magnifierRect = magnifierDisplay.GetComponent<RectTransform>();
        magnifierRect.sizeDelta = magnifierSize;
        magnifierRect.pivot = new Vector2(0.5f, 0.5f);
        
        // 初始定位
        UpdateMagnifierPosition();
    }

    void CreateRenderTexture()
    {
        int width = (int)(magnifierSize.x * resolutionScale);
        int height = (int)(magnifierSize.y * resolutionScale);
        
        // Create new render texture
        renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp,
            antiAliasing = 4
        };
        renderTexture.Create();
    }
    
    // 清除渲染纹理内容
    void ClearRenderTexture()
    {
        if (renderTexture == null) return;
        
        // 临时激活渲染纹理
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;
        
        // 清除为透明
        GL.Clear(true, true, Color.clear);
        
        // 恢复原渲染目标
        RenderTexture.active = currentRT;
    }

    void Update()
    {
        if (!isActive || !enabled) return;
        
        // 安全模式：组件缺失时跳过更新
        if (mainCamera == null || magnifierCamera == null || magnifierRect == null)
        {
            return;
        }
        
        // 更新放大镜位置
        UpdateMagnifierPosition();
        
        // 应用放大效果
        ApplyMagnification();
    }
    
    void UpdateMagnifierPosition()
    {
        Vector2 screenPosition = targetAreaPosition;
        
        // 优先使用参考对象的位置
        if (areaReference != null)
        {
            screenPosition = mainCamera.WorldToScreenPoint(areaReference.position);
        }
        
        // 转换到Canvas空间
        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            magnifierRect.parent.GetComponent<RectTransform>(),
            screenPosition,
            null,
            out canvasPosition
        );
        
        // 应用位置（考虑父级Canvas的缩放）
        magnifierRect.localPosition = canvasPosition;
        
        // 计算并设置摄像机视口矩形
        Rect viewportRect = CalculateViewportRect(screenPosition);
        magnifierCamera.rect = viewportRect;
    }
    
    void ApplyMagnification()
    {
        if (magnifierCamera.orthographic)
        {
            magnifierCamera.orthographicSize = mainCamera.orthographicSize / magnification;
        }
        else
        {
            magnifierCamera.fieldOfView = mainCamera.fieldOfView / magnification;
        }
    }

    Rect CalculateViewportRect(Vector2 center)
    {
        // Convert screen coordinates to viewport coordinates
        float x = center.x / Screen.width;
        float y = center.y / Screen.height;
        
        // Calculate width/height in viewport space
        float width = targetAreaSize.x / Screen.width;
        float height = targetAreaSize.y / Screen.height;
        
        // Adjust for center pivot
        x -= width / 2;
        y -= height / 2;
        
        return new Rect(x, y, width, height);
    }

    public void ToggleMagnifier(bool active)
    {
        isActive = active;
        if (magnifierDisplay != null)
        {
            magnifierDisplay.gameObject.SetActive(active);
            
            // 关闭时清除残留画面
            if (!active)
            {
                ClearRenderTexture();
            }
        }
    }

    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }
}
