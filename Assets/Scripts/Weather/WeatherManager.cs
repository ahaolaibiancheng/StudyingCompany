using UnityEngine;

public enum RainIntensity
{
    None,
    Light,
    Medium,
    Heavy
}

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance;
    
    [SerializeField] public RainIntensity currentRainIntensity = RainIntensity.None;
    [SerializeField] private float rainSoundVolume = 0.5f;
    
    // References to components
    private ParticleSystem rainParticleSystem;
    private AudioSource rainAudioSource;
    private GlassEffectController glassEffect;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        InitializeComponents();
    }
    
    private void InitializeComponents()
    {
        rainParticleSystem = GetComponentInChildren<ParticleSystem>();
        rainAudioSource = GetComponent<AudioSource>();
        glassEffect = FindObjectOfType<GlassEffectController>();
    }
    
    public void SetRainIntensity(RainIntensity intensity)
    {
        currentRainIntensity = intensity;
        UpdateRainEffects();
    }
    
    public void SetRainVolume(float volume)
    {
        rainSoundVolume = Mathf.Clamp01(volume);
        if (rainAudioSource != null)
        {
            rainAudioSource.volume = rainSoundVolume;
        }
    }
    
    private void UpdateRainEffects()
    {
        UpdateRainParticles();
        UpdateGlassEffects();
        UpdateRainSound();
    }
    
    private void UpdateRainParticles()
    {
        if (rainParticleSystem == null) return;
        
        var emission = rainParticleSystem.emission;
        var main = rainParticleSystem.main;
        
        switch (currentRainIntensity)
        {
            case RainIntensity.None:
                emission.rateOverTime = 0;
                break;
            case RainIntensity.Light:
                emission.rateOverTime = 50;
                main.startSpeed = 5f;
                break;
            case RainIntensity.Medium:
                emission.rateOverTime = 200;
                main.startSpeed = 8f;
                break;
            case RainIntensity.Heavy:
                emission.rateOverTime = 500;
                main.startSpeed = 12f;
                break;
        }
        
        if (currentRainIntensity == RainIntensity.None)
        {
            rainParticleSystem.Stop();
        }
        else if (!rainParticleSystem.isPlaying)
        {
            rainParticleSystem.Play();
        }
    }
    
    private void UpdateGlassEffects()
    {
        if (glassEffect != null)
        {
            glassEffect.SetRainIntensity((int)currentRainIntensity);
        }
    }
    
    private void UpdateRainSound()
    {
        if (rainAudioSource == null) return;
        
        if (currentRainIntensity == RainIntensity.None)
        {
            rainAudioSource.Stop();
        }
        else
        {
            if (!rainAudioSource.isPlaying)
            {
                rainAudioSource.Play();
            }
            
            // Adjust sound pitch based on intensity
            rainAudioSource.pitch = 0.8f + (0.2f * (int)currentRainIntensity);
        }
    }
}
