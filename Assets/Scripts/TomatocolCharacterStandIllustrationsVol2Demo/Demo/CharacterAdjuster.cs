using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// Handles adjustments for character display such as position, scale, eye color, and bangs toggle.
/// </summary>
public class CharacterAdjuster : MonoBehaviour
{
    [Header("Target Character (for getting info and parts)")]
    public GameObject targetCharacter;

    [Header("Character Root (used for scaling and movement)")]
    public Transform characterRoot;

    // [Header("Bangs Toggle")]
    // public Toggle bangsToggle;

    // [Header("Bangs Transparency")]
    // public Slider bangsAlphaSlider;
    // public TMP_Text bangsAlphaCommandLabel;

    // [Header("Eye Color Sliders")]
    // public Slider eyeHueSlider;
    // public Slider eyeSaturationSlider;

    // [Header("Command Labels")]
    // public TMP_Text hueCommandLabel;
    // public TMP_Text saturationCommandLabel;
    // public TMP_Text bangsCommandLabel;

    // [Header("Character Transform Sliders")]
    // public Slider scaleSlider;
    // public Slider positionYSlider;

    private FaceExpressionController faceController;
    private Dictionary<string, (float hue, float saturation)> colorMap = new();
    private string currentCharacterKey = null;

    private const float ScaleMultiplier = 0.01f;
    // private float baseYPosition = 0f;
    // private float currentYOffset = 0f;

    // private bool isDragging = false;
    private Vector3 dragStartPos;
    private float dragStartSliderValue;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        // bangsToggle?.onValueChanged.AddListener(OnBangsToggleChanged);
        // eyeHueSlider?.onValueChanged.AddListener(OnHueChanged);
        // eyeSaturationSlider?.onValueChanged.AddListener(OnSaturationChanged);
        // bangsAlphaSlider?.onValueChanged.AddListener(OnBangsAlphaChanged);


        // if (scaleSlider != null)
        // {
        //     scaleSlider.onValueChanged.AddListener(OnScaleChanged);
        //     OnScaleChanged(scaleSlider.value);
        // }

        // positionYSlider?.onValueChanged.AddListener(UpdateCharacterPosition);
    }

    public void SetTargetCharacter(GameObject character)
    {
        if (faceController != null && !string.IsNullOrEmpty(currentCharacterKey))
        {
            colorMap[currentCharacterKey] = (
                faceController.eyeHueShift,
                faceController.eyeSaturation
            );
        }

        targetCharacter = character;
        faceController = targetCharacter?.GetComponentInChildren<FaceExpressionController>();
        currentCharacterKey = faceController?.characterName ?? character.name;

        var spriteRenderer = targetCharacter?.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.GetComponent<Collider>() == null)
        {
            spriteRenderer.gameObject.AddComponent<BoxCollider>();
        }

        if (faceController != null)
        {
            float hue = faceController.eyeHueShift;
            float saturation = faceController.eyeSaturation;

            if (colorMap.TryGetValue(currentCharacterKey, out var saved))
            {
                hue = saved.hue;
                saturation = saved.saturation;
            }

            // eyeHueSlider?.SetValueWithoutNotify(hue);
            // eyeSaturationSlider?.SetValueWithoutNotify(saturation);

            // faceController.SetEyeHue(hue);
            // faceController.SetEyeSaturation(saturation);

            // bangsToggle?.SetIsOnWithoutNotify(IsBangsVisible());
        }

        // UpdateCommandLabels();

        // if (characterRoot != null)
        // {
        //     baseYPosition = characterRoot.localPosition.y - currentYOffset;
        //     positionYSlider?.SetValueWithoutNotify(currentYOffset);
        // }

        // if (bangsAlphaSlider != null && faceController != null)
        // {
        //     bangsAlphaSlider.SetValueWithoutNotify(100f); // 100%
        // }
    }

    // void OnBangsToggleChanged(bool isOn)
    // {
    //     faceController?.SetBangs(isOn);
    //     UpdateBangsLabel(isOn);
    // }

    // void OnBangsAlphaChanged(float value)
    // {
    //     float normalized = value / 100f;
    //     faceController?.SetBangsAlpha(normalized);

    //     if (bangsAlphaCommandLabel != null)
    //         bangsAlphaCommandLabel.text = $"SetBangsAlpha({normalized:F2});";
    // }


    bool IsBangsVisible()
    {
        var bangs = FaceExpressionController.FindDeepChild(faceController.transform, "Bangs");
        return bangs != null && bangs.gameObject.activeSelf;
    }

    // void OnHueChanged(float value)
    // {
    //     faceController?.SetEyeHue(value);
    //     UpdateHueLabel(value);
    // }

    // void OnSaturationChanged(float value)
    // {
    //     faceController?.SetEyeSaturation(value);
    //     UpdateSaturationLabel(value);
    // }

    void OnScaleChanged(float value)
    {
        if (characterRoot != null)
        {
            float normalizedScale = value * ScaleMultiplier;
            characterRoot.localScale = Vector3.one * normalizedScale;
        }
    }

    // void UpdateCharacterPosition(float _)
    // {
    //     if (characterRoot == null) return;

    //     currentYOffset = positionYSlider?.value ?? 0f;
    //     Vector3 pos = characterRoot.localPosition;
    //     pos.y = baseYPosition + currentYOffset;
    //     characterRoot.localPosition = pos;
    // }

    // void Update()
    // {
    //     if (MouseWheelBlocker.IsBlocking)
    //         return;

    //     float scroll = Input.mouseScrollDelta.y;
    //     if (Mathf.Abs(scroll) > 0.01f && scaleSlider != null)
    //     {
    //         float step = 8f;
    //         float newValue = Mathf.Clamp(scaleSlider.value + scroll * step, scaleSlider.minValue, scaleSlider.maxValue);
    //         scaleSlider.value = newValue;
    //     }

    //     // Begin dragging with right click
    //     if (Input.GetMouseButtonDown(1))
    //     {
    //         isDragging = true;
    //         dragStartPos = Input.mousePosition;
    //         dragStartSliderValue = positionYSlider?.value ?? 0f;
    //     }

    //     // End drag
    //     if (Input.GetMouseButtonUp(1))
    //     {
    //         isDragging = false;
    //     }

    //     // Update slider during drag
    //     if (isDragging && positionYSlider != null)
    //     {
    //         float dragSpeed = 0.07f;
    //         float deltaY = (Input.mousePosition.y - dragStartPos.y) * dragSpeed;
    //         float newY = Mathf.Clamp(dragStartSliderValue + deltaY, positionYSlider.minValue, positionYSlider.maxValue);
    //         positionYSlider.value = newY;
    //     }
    // }

    // void UpdateCommandLabels()
    // {
    //     if (faceController == null) return;

    //     UpdateHueLabel(faceController.eyeHueShift);
    //     UpdateSaturationLabel(faceController.eyeSaturation);

    //     bool isOn = IsBangsVisible();
    //     UpdateBangsLabel(isOn);
    // }

    // void UpdateHueLabel(float value)
    // {
    //     if (hueCommandLabel != null)
    //         hueCommandLabel.text = $"SetEyeHue({value:F2});";
    // }

    // void UpdateSaturationLabel(float value)
    // {
    //     if (saturationCommandLabel != null)
    //         saturationCommandLabel.text = $"SetEyeSaturation({value:F2});";
    // }

    // void UpdateBangsLabel(bool isOn)
    // {
    //     if (bangsCommandLabel != null)
    //         bangsCommandLabel.text = $"SetBangs({isOn.ToString().ToLower()});";
    // }
}
