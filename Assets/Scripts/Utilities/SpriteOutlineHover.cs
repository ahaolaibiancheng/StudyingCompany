using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteOutlineHover : MonoBehaviour
{
    private static readonly int GlowStrengthProp = Shader.PropertyToID("_GlowStrength");
    private static readonly int OutlineColorProp = Shader.PropertyToID("_OutlineColor");
    private static readonly int BaseColorProp = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorProp = Shader.PropertyToID("_Color");
    private static readonly int OutlineThicknessProp = Shader.PropertyToID("_OutlineThickness");
    private static readonly int OutlineSoftnessProp = Shader.PropertyToID("_OutlineSoftness");
    private static readonly int AlphaClipProp = Shader.PropertyToID("_AlphaClip");

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Material materialInstance;
    private bool hasGlowProperty;
    private float targetGlow;
    private float currentGlow;

    [Header("Hover Settings")]
    public Color hoverColor = Color.yellow;
    public float hoverGlow = 2f;
    public float normalGlow = 0f;
    public float lerpSpeed = 6f;

    [Header("Material Settings")]
    [Tooltip("Optional: outline-enabled URP 2D material template. When null the current material is cloned.")]
    public Material outlineMaterialTemplate;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer != null ? spriteRenderer.sharedMaterial : null;
    }

    private void OnEnable()
    {
        EnsureMaterialInstance();
        currentGlow = normalGlow;
        targetGlow = normalGlow;
        ApplyGlow(currentGlow);
    }

    private void EnsureMaterialInstance()
    {
        if (materialInstance != null || spriteRenderer == null)
        {
            return;
        }

        Material template = outlineMaterialTemplate != null ? outlineMaterialTemplate : originalMaterial;
        if (template == null)
        {
            Debug.LogWarning($"{nameof(SpriteOutlineHover)} on {gameObject.name}: no material template found.");
            return;
        }

        materialInstance = Instantiate(template);
        spriteRenderer.sharedMaterial = materialInstance;

        hasGlowProperty = materialInstance.HasProperty(GlowStrengthProp);
        if (!hasGlowProperty)
        {
            Debug.LogWarning($"{nameof(SpriteOutlineHover)}: missing _GlowStrength property; cannot drive outline intensity.", this);
        }

        if (materialInstance.HasProperty(OutlineColorProp))
        {
            materialInstance.SetColor(OutlineColorProp, hoverColor);
        }

        if (materialInstance.HasProperty(ColorProp))
        {
            materialInstance.SetColor(ColorProp, Color.white);
        }
        else if (materialInstance.HasProperty(BaseColorProp))
        {
            materialInstance.SetColor(BaseColorProp, Color.white);
        }

        if (materialInstance.HasProperty(OutlineThicknessProp))
        {
            materialInstance.SetFloat(OutlineThicknessProp, 1f);
        }

        if (materialInstance.HasProperty(OutlineSoftnessProp))
        {
            materialInstance.SetFloat(OutlineSoftnessProp, 0.3f);
        }

        if (materialInstance.HasProperty(AlphaClipProp))
        {
            materialInstance.SetFloat(AlphaClipProp, 0.01f);
        }
    }

    private void OnMouseEnter()
    {
        targetGlow = hoverGlow;
    }

    private void OnMouseExit()
    {
        targetGlow = normalGlow;
    }

    private void Update()
    {
        if (materialInstance == null)
        {
            return;
        }

        currentGlow = Mathf.Lerp(currentGlow, targetGlow, Time.deltaTime * lerpSpeed);
        ApplyGlow(currentGlow);
    }

    private void OnDisable()
    {
        if (spriteRenderer != null && originalMaterial != null)
        {
            spriteRenderer.sharedMaterial = originalMaterial;
        }

        if (materialInstance != null)
        {
            Destroy(materialInstance);
            materialInstance = null;
        }

        currentGlow = normalGlow;
        targetGlow = normalGlow;
        hasGlowProperty = false;
    }

    private void ApplyGlow(float value)
    {
        if (!hasGlowProperty || materialInstance == null)
        {
            return;
        }

        materialInstance.SetFloat(GlowStrengthProp, value);
    }
}
