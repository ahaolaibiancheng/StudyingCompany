using UnityEngine;
using UnityEngine.UI;

public class WeatherControlPanel : BasePanel
{
    [SerializeField] private Slider rainIntensitySlider;
    [SerializeField] private Slider rainVolumeSlider;
    [SerializeField] private Text weatherStatusText;
    
    void Start()
    {
        rainIntensitySlider.onValueChanged.AddListener(OnRainIntensityChanged);
        rainVolumeSlider.onValueChanged.AddListener(OnRainVolumeChanged);
        
        UpdateWeatherDisplay();
    }
    
    private void OnRainIntensityChanged(float value)
    {
        int intensity = Mathf.RoundToInt(value);
        WeatherManager.Instance.SetRainIntensity((RainIntensity)intensity);
        UpdateWeatherDisplay();
    }
    
    private void OnRainVolumeChanged(float volume)
    {
        WeatherManager.Instance.SetRainVolume(volume);
    }
    
    private void UpdateWeatherDisplay()
    {
        weatherStatusText.text = $"Current Weather: {WeatherManager.Instance.currentRainIntensity.ToString()}";
    }
}
