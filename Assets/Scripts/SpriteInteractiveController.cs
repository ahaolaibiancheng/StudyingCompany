using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // 添加新输入系统引用
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class SpriteInteractiveController : MonoBehaviour
{
    [Header("Target Sprite")]
    public GameObject targetSprite;
    private GameObject pretargetSprite;

    [Header("Control Buttons")]

    public Slider scaleSlider; // 新增可拖动滑块
    public Button resetButton;
    public Button rotationButton;
    public Button deleteButton;
    public Button turnButton;
    public Button sortingOrderUpButton;
    public Button sortingOrderDownButton;

    [Header("Scale Settings")]
    public float scaleStep = 0.1f;
    public float minScale = 0.1f;
    public float maxScale = 5f;

    [Header("Outline Settings")]
    public Color outlineColor = Color.yellow;
    public float outlineWidth = 0.1f;
    public bool showOutlineWhenSelected = true;

    [Header("Edit Panel Settings")]
    public RectTransform editPanel;
    public Vector2 panelOffset = new Vector2(20, -20); // 相对于Sprite的偏移
    public float panelFollowSpeed = 10f; // 面板跟随速度

    private SpriteRenderer spriteRenderer;
    private LineRenderer outlineRenderer;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private int originalSortingOrder;
    public bool isDragging = false;
    public bool isSelected = false;
    public bool isMouseOverPanel = false; // 鼠标是否在EditPanel上
    private Vector3 offset;
    private Camera mainCamera;
    private Canvas canvas;
    private Mouse currentMouse; // 添加鼠标引用

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return;
        }

        // 获取鼠标实例
        currentMouse = Mouse.current;
        if (currentMouse == null)
        {
            Debug.LogError("Mouse not found!");
            return;
        }

        InitializeUI();
    }

    void Update()
    {
        HandleMouseInput();
        UpdateOutline();
        UpdateEditPanel();
    }

    void CreateOutlineRenderer()
    {
        // 创建轮廓渲染器
        outlineRenderer = targetSprite.AddComponent<LineRenderer>();
        outlineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        outlineRenderer.startColor = outlineColor;
        outlineRenderer.endColor = outlineColor;
        outlineRenderer.startWidth = outlineWidth;
        outlineRenderer.endWidth = outlineWidth;
        outlineRenderer.loop = true;
        outlineRenderer.useWorldSpace = false;
        outlineRenderer.enabled = false; // 默认隐藏
    }

    void InitializeUI()
    {
        // 缩放滑块
        if (scaleSlider != null)
        {
            scaleSlider.minValue = minScale;
            scaleSlider.maxValue = maxScale;
            scaleSlider.value = targetSprite.transform.localScale.x;
            scaleSlider.onValueChanged.AddListener(OnScaleSliderChanged);
        }

        // 重置按钮
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetScaleAndRotation);

        // 旋转按钮
        if (rotationButton != null)
            rotationButton.onClick.AddListener(Rotate45);

        // 删除按钮
        if (deleteButton != null)
            deleteButton.onClick.AddListener(DeleteSprite);

        // 翻转按钮
        if (turnButton != null)
            turnButton.onClick.AddListener(TurnSprite);

        // 图层控制按钮
        if (sortingOrderUpButton != null)
            sortingOrderUpButton.onClick.AddListener(SortingOrderUp);

        if (sortingOrderDownButton != null)
            sortingOrderDownButton.onClick.AddListener(SortingOrderDown);
    }

    void HandleMouseInput()
    {
        // 这里不能添加这行
        // if (InteractWithUI()) return;

        // 鼠标按下开始拖动或选择
        if (currentMouse.leftButton.wasPressedThisFrame)
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(currentMouse.position.ReadValue());
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            // 检测当前位置的Sprite碰撞体
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            // 点击了不同物体或者为空时，targetSprite重新赋值
            if (hit.collider != null && hit.collider.gameObject != targetSprite)
                SetTargetSprite(hit.collider.gameObject);

            // 点击同一物体
            if (hit.collider != null && hit.collider.gameObject == targetSprite)
            {
                isDragging = true;  // 默认拖拽状态
                isSelected = true;
                offset = targetSprite.transform.position - mousePos;
                offset.z = 0; // 确保Z轴为0
            }
            else
            {
                // 点击其他地方取消选择
                isSelected = false;
            }
        }

        // 鼠标拖动
        if (isDragging && currentMouse.leftButton.isPressed)
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(currentMouse.position.ReadValue());
            Vector3 newPosition = mousePos + offset;    // offset保证结束拖拽时，鼠标与物体的相对位置保持不变
            newPosition.z = 0; // 固定Z轴为0

            targetSprite.transform.position = newPosition;
        }

        // 鼠标释放结束拖动
        if (currentMouse.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }

        // 检测鼠标是否在EditPanel上
        CheckMouseOverEditPanel();
    }

    #region 按钮绑定事件
    // 滑块缩放方法
    public void OnScaleSliderChanged(float value)
    {
        if (targetSprite == null) return;

        targetSprite.transform.localScale = new Vector3(value, value, 1f);
        // 更新滑块值
        if (scaleSlider != null)
        {
            scaleSlider.SetValueWithoutNotify(value);
        }
    }

    // 重置大小和旋转
    public void ResetScaleAndRotation()
    {
        if (targetSprite == null) return;

        targetSprite.transform.localScale = originalScale;
        targetSprite.transform.rotation = originalRotation;

        // 更新滑块
        if (scaleSlider != null)
        {
            scaleSlider.value = originalScale.x;
        }
    }

    // 旋转45度
    public void Rotate45()
    {
        if (targetSprite == null) return;

        float currentRotation = targetSprite.transform.eulerAngles.z;
        float newRotation = currentRotation + 45f;
        targetSprite.transform.rotation = Quaternion.Euler(0f, 0f, newRotation);
    }

    // 删除Sprite
    public void DeleteSprite()
    {
        if (targetSprite != null)
        {
            // 删除Sprite的父节点
            Destroy(targetSprite.transform.parent.gameObject);
            // Destroy(targetSprite);
            // 可选：禁用控制器或显示消息
            Debug.Log("Sprite deleted");
        }
    }

    // 翻转Sprite
    public void TurnSprite()
    {
        if (targetSprite != null)
        {
            Vector3 currentScale = targetSprite.transform.localScale;
            targetSprite.transform.localScale = new Vector3(-currentScale.x, currentScale.y, currentScale.z);
        }
    }

    // 图层控制
    public void SortingOrderUp()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder++;
        }
    }

    public void SortingOrderDown()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder--;
        }
    }
    #endregion

    #region 公共方法供其他脚本调用
    public void SetTargetSprite(GameObject newTarget)
    {
        targetSprite = newTarget;
        pretargetSprite = targetSprite;
        spriteRenderer = targetSprite.GetComponent<SpriteRenderer>();

        // 重新创建轮廓渲染器
        if (outlineRenderer != null)
        {
            Destroy(outlineRenderer);
        }
        CreateOutlineRenderer();

        // 更新原始值
        originalScale = targetSprite.transform.localScale;
        originalRotation = targetSprite.transform.rotation;
        originalSortingOrder = spriteRenderer != null ? spriteRenderer.sortingOrder : 0;
        // 确保Z轴为0
        targetSprite.transform.position = new Vector3(
            targetSprite.transform.position.x,
            targetSprite.transform.position.y,
            0f
        );
        // 更新UI
        if (scaleSlider != null)
        {
            scaleSlider.value = originalScale.x;
        }
    }

    public Vector2 GetCurrentPosition()
    {
        return targetSprite != null ? (Vector2)targetSprite.transform.position : Vector2.zero;
    }

    public float GetCurrentScale()
    {
        return targetSprite != null ? targetSprite.transform.localScale.x : 1f;
    }

    public float GetCurrentRotation()
    {
        return targetSprite != null ? targetSprite.transform.eulerAngles.z : 0f;
    }

    public int GetCurrentSortingOrder()
    {
        return spriteRenderer != null ? spriteRenderer.sortingOrder : 0;
    }

    // 选择控制
    public void SelectSprite()
    {
        isSelected = true;
    }

    public void DeselectSprite()
    {
        isSelected = false;
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    // 设置缩放步长
    public void SetScaleStep(float step)
    {
        scaleStep = Mathf.Max(0.01f, step);
    }

    // 设置缩放范围
    public void SetScaleRange(float min, float max)
    {
        minScale = Mathf.Max(0.01f, min);
        maxScale = Mathf.Max(minScale, max);

        if (scaleSlider != null)
        {
            scaleSlider.minValue = minScale;
            scaleSlider.maxValue = maxScale;
        }
    }

    // 轮廓设置
    public void SetOutlineColor(Color color)
    {
        outlineColor = color;
        if (outlineRenderer != null)
        {
            outlineRenderer.startColor = color;
            outlineRenderer.endColor = color;
        }
    }

    public void SetOutlineWidth(float width)
    {
        outlineWidth = width;
        if (outlineRenderer != null)
        {
            outlineRenderer.startWidth = width;
            outlineRenderer.endWidth = width;
        }
    }

    public void ShowOutline(bool show)
    {
        showOutlineWhenSelected = show;
        if (outlineRenderer != null)
        {
            outlineRenderer.enabled = show && isSelected;
        }
    }

    #endregion

    void UpdateEditPanel()
    {
        if (editPanel == null) return;
        if (isMouseOverPanel) return;

        // 控制EditPanel的显示和隐藏
        editPanel.gameObject.SetActive(isSelected);

        // 当鼠标点击targetSprite后
        if (isSelected && targetSprite != null)
        {
            // 获取Sprite在屏幕上的位置
            Vector3 spriteScreenPos = mainCamera.WorldToScreenPoint(targetSprite.transform.position);
            // 计算目标位置
            Vector2 targetPosition;

            if (isDragging)
            {
                // 拖动时实时跟随鼠标位置
                Vector2 mousePos = currentMouse.position.ReadValue();
                targetPosition = new Vector2(mousePos.x + panelOffset.x, mousePos.y + panelOffset.y);
            }
            else
            {
                // 非拖动时显示在Sprite右上角
                targetPosition = new Vector2(spriteScreenPos.x + panelOffset.x, spriteScreenPos.y + panelOffset.y);
            }

            // 如果鼠标在EditPanel上，停止移动以避免干扰按钮点击
            if (isMouseOverPanel && !isDragging)
            {
                return;
            }

            // 确保面板在屏幕范围内
            Rect panelRect = editPanel.rect;
            float panelWidth = panelRect.width * editPanel.localScale.x;
            float panelHeight = panelRect.height * editPanel.localScale.y;

            // 限制在屏幕范围内
            targetPosition.x = Mathf.Clamp(targetPosition.x, panelWidth * 0.5f, Screen.width - panelWidth * 0.5f);
            targetPosition.y = Mathf.Clamp(targetPosition.y, panelHeight * 0.5f, Screen.height - panelHeight * 0.5f);

            // 平滑移动面板
            Vector2 currentPosition = editPanel.position;
            // 已到达目标位置
            if (Vector2.Distance(currentPosition, targetPosition) < 0.1f) return;
            editPanel.position = Vector2.Lerp(currentPosition, targetPosition, panelFollowSpeed * Time.deltaTime);
        }
    }

    void CheckMouseOverEditPanel()
    {
        if (editPanel == null || currentMouse == null) return;

        // 鼠标屏幕位置
        Vector2 mousePos = currentMouse.position.ReadValue();
        isMouseOverPanel = RectTransformUtility.RectangleContainsScreenPoint(editPanel, mousePos, null);
    }

    // FIXME: 是否在创建时渲染一次就可以了
    void UpdateOutline()
    {
        if (outlineRenderer != null && isSelected && showOutlineWhenSelected)
        {
            Sprite sprite = spriteRenderer.sprite;
            if (sprite != null)
            {
                // 使用精灵的纹理边界来创建更精确的轮廓
                Vector2[] vertices = GetSpriteOutlineVertices(sprite);

                outlineRenderer.positionCount = vertices.Length;
                Vector3[] points = new Vector3[vertices.Length];

                // 直接使用顶点坐标（相对于pivot），不需要额外的坐标转换
                for (int i = 0; i < vertices.Length; i++)
                {
                    points[i] = new Vector3(vertices[i].x, vertices[i].y, 0);
                }

                outlineRenderer.SetPositions(points);

                // 确保轮廓渲染器启用
                outlineRenderer.enabled = true;
            }
            else
            {
                // 回退到边界框方法
                DrawBoundingBoxOutline();
            }
        }
        else if (outlineRenderer != null)
        {
            outlineRenderer.enabled = false;
        }
    }

    private Vector2[] GetSpriteOutlineVertices(Sprite sprite)
    {
        // 获取精灵的纹理矩形
        Rect rect = sprite.textureRect;
        Vector2[] vertices = new Vector2[4];

        // 获取像素与单位的转换比例
        float pixelsPerUnit = sprite.pixelsPerUnit;

        // 将像素坐标转换为Unity单位，并相对于pivot
        Vector2 min = (new Vector2(rect.xMin, rect.yMin) - sprite.pivot) / pixelsPerUnit;
        Vector2 max = (new Vector2(rect.xMax, rect.yMax) - sprite.pivot) / pixelsPerUnit;

        // 创建矩形顶点（顺时针方向）
        vertices[0] = new Vector2(min.x, min.y);
        vertices[1] = new Vector2(max.x, min.y);
        vertices[2] = new Vector2(max.x, max.y);
        vertices[3] = new Vector2(min.x, max.y);

        return vertices;
    }

    private void DrawBoundingBoxOutline()
    {
        Bounds bounds = spriteRenderer.bounds;
        Vector3[] points = new Vector3[4];

        // 转换为局部坐标
        points[0] = targetSprite.transform.InverseTransformPoint(new Vector3(bounds.min.x, bounds.min.y, 0));
        points[1] = targetSprite.transform.InverseTransformPoint(new Vector3(bounds.max.x, bounds.min.y, 0));
        points[2] = targetSprite.transform.InverseTransformPoint(new Vector3(bounds.max.x, bounds.max.y, 0));
        points[3] = targetSprite.transform.InverseTransformPoint(new Vector3(bounds.min.x, bounds.max.y, 0));

        outlineRenderer.positionCount = 4;
        outlineRenderer.SetPositions(points);
    }
}