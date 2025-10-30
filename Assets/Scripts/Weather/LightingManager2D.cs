using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Collections.Generic;

public class LightingManager2D : MonoBehaviour
{
    [Header("全局光源（Global Light 2D）")]
    public Light2D globalLight;

    [Header("昼夜颜色")]
    public Color dayColor = new Color(1f, 0.95f, 0.85f, 1f);
    public Color cloudyColor = new Color(0.7f, 0.75f, 0.8f, 1f);
    public Color nightColor = new Color(0.1f, 0.1f, 0.3f, 1f);

    [Header("光强度")]
    [Range(0, 1)] public float dayIntensity = 1.0f;
    [Range(0, 1)] public float cloudyIntensity = 0.6f;
    [Range(0, 1)] public float nightIntensity = 0.2f;

    [Header("过渡时间（秒）")]
    public float transitionDuration = 2f;

    [Header("UI 控制按钮")]
    public Button dayButton;
    public Button cloudyButton;
    public Button nightButton;

    [Header("交互光源列表")]
    public List<InteractiveLight2D> interactiveLights = new List<InteractiveLight2D>();

    private Coroutine transitionCoroutine;

    void Start()
    {
        if (dayButton) dayButton.onClick.AddListener(() => SetLightingSmooth(dayColor, dayIntensity, false));
        if (cloudyButton) cloudyButton.onClick.AddListener(() => SetLightingSmooth(cloudyColor, cloudyIntensity, false));
        if (nightButton) nightButton.onClick.AddListener(() => SetLightingSmooth(nightColor, nightIntensity, true));
    }

    public void SetLightingSmooth(Color targetColor, float targetIntensity, bool enableLights)
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(TransitionLighting(targetColor, targetIntensity, enableLights));
    }

    IEnumerator TransitionLighting(Color targetColor, float targetIntensity, bool enableLights)
    {
        if (globalLight == null) yield break;

        Color startColor = globalLight.color;
        float startIntensity = globalLight.intensity;
        float t = 0f;

        // 夜晚开启所有灯光（提前点亮）
        if (enableLights)
        {
            foreach (var l in interactiveLights)
                l.SetLight(true);
        }

        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.SmoothStep(0, 1, t / transitionDuration);
            globalLight.color = Color.Lerp(startColor, targetColor, lerp);
            globalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, lerp);
            yield return null;
        }

        // 白天或阴天关闭灯光
        if (!enableLights)
        {
            foreach (var l in interactiveLights)
                l.SetLight(false);
        }

        globalLight.color = targetColor;
        globalLight.intensity = targetIntensity;
    }
}
