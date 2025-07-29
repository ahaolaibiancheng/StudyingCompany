using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : BasePanel
{
    public Button closeButton;
    public Slider volumeSlider;
    public Text volumeText;

    protected override void Awake()
    {
        base.Awake();
        InitUI();
    }

    private void Start()
    {
        // Initialize volume settings
        LoadVolumeSettings();
    }

    private void InitUI()
    {
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        UpdateVolumeText(volumeSlider.value);
    }
    private void LoadVolumeSettings()
    {
        if (PlayerPrefs.HasKey("SystemVolume"))
        {
            float volume = PlayerPrefs.GetFloat("SystemVolume");
            AudioListener.volume = volume;
            if (volumeSlider != null)
            {
                volumeSlider.value = volume;
            }
        }
        else
        {
            // Default volume
            AudioListener.volume = 0.8f;
            if (volumeSlider != null)
            {
                volumeSlider.value = 0.8f;
            }
        }
    }

    public void OnCloseButtonClicked()
    {
        ClosePanel();
        UIManager.Instance.OpenPanel(UIConst.MainPanel);
    }

    // Volume control methods
    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        UpdateVolumeText(value);
        SaveVolumeSettings();
    }

    private void UpdateVolumeText(float value)
    {
        if (volumeText != null)
        {
            volumeText.text = $"Volume: {Mathf.RoundToInt(value * 100)}%";
        }
    }

    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("SystemVolume", volumeSlider.value);
        PlayerPrefs.Save();
    }

}