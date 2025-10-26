using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeatherPanelUI : BasePanel
{
    [Header("Rain")]
    [SerializeField] private Slider rainIntensitySlider;
    [SerializeField] private TextMeshProUGUI weatherStatusText;
    public GlassEffectController glassEffectController;

    protected override void Awake()
    {
        rainIntensitySlider.onValueChanged.AddListener(OnRainIntensityChanged);
    }

    private void Start()
    {
        glassEffectController.UpdateGlassEffects(RainIntensity.None);
        UpdateWeatherDisplay();
    }

    private void OnRainIntensityChanged(float value)
    {
        int intensity = Mathf.RoundToInt(value);
        glassEffectController.UpdateGlassEffects((RainIntensity)intensity);
        UpdateWeatherDisplay();
    }

    private void UpdateWeatherDisplay()
    {
        weatherStatusText.text = $"Current Rain: {glassEffectController.currentRainIntensity.ToString()}";
    }
}
