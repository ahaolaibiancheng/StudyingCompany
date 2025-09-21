using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        // Debug.Log("Screen.width/height: " + Screen.width + "/" + Screen.height);
    }

    // In your existing GameManager.cs or similar
    public void ChangeWeather(RainIntensity intensity)
    {
        WeatherManager.Instance.SetRainIntensity(intensity);
        
        // Optional: Notify other systems about weather change
        // EventHandler.Instance.TriggerWeatherChanged(intensity);
    }
}
