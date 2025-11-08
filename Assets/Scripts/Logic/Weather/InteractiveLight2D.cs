using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 可交互2D灯光（鼠标点击开关 + 悬停高亮）
/// 支持发光Sprite、篝火闪烁与鼠标高亮效果
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class InteractiveLight2D : MonoBehaviour
{
    [Header("发光Sprite（可选）")]
    public SpriteRenderer emissionSprite;

    [Header("是否为篝火（自动闪烁）")]
    public bool isCampfire = false;

    [Header("发光颜色参数")]
    public Color emissionColor = Color.white;
    public float emissionIntensity = 2f;

    private Light2D light2D;
    private bool isOn = false;
    private Material _matInstance;

    private float baseIntensity; // 记录原始光强
    private Color baseColor;     // 记录原始颜色

    void Start()
    {
        light2D = GetComponentInChildren<Light2D>();

        if (emissionSprite)
            _matInstance = emissionSprite.material;

        baseIntensity = light2D.intensity;
        baseColor = emissionColor;

        // 确保 Collider 可点击
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;

        SetLight(false);
    }

    void Update()
    {
        // 🔥 篝火闪烁逻辑
        if (isCampfire && isOn)
        {
            float flicker = Mathf.PerlinNoise(Time.time * 3f, 0) * 0.4f + 0.8f;
            float targetIntensity = baseIntensity * flicker;
            light2D.intensity = targetIntensity;

            if (_matInstance)
                _matInstance.SetColor("_Color", baseColor * emissionIntensity * flicker);
        }
    }

    /// <summary>
    /// 鼠标点击开关
    /// </summary>
    void OnMouseDown()
    {
        ToggleLight();
    }

    /// <summary>
    /// 鼠标移入高亮
    /// </summary>
    void OnMouseEnter()
    {
        if (_matInstance)
            _matInstance.SetColor("_Color", baseColor * emissionIntensity);
    }

    /// <summary>
    /// 鼠标移出恢复
    /// </summary>
    void OnMouseExit()
    {
        if (isOn)
            light2D.intensity = baseIntensity;

        if (_matInstance)
            _matInstance.SetColor("_Color", baseColor * emissionIntensity);
    }

    public void ToggleLight()
    {
        SetLight(!isOn);
    }

    public void SetLight(bool on)
    {
        isOn = on;
        light2D.enabled = on;

        if (on)
        {
            if (_matInstance)
                _matInstance.SetColor("_Color", baseColor * emissionIntensity);
        }
        else
        {
            if (_matInstance)
                _matInstance.SetColor("_Color", Color.black);
        }
    }
}
