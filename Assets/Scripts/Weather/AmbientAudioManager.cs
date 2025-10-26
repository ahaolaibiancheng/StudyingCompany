using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AmbientAudioManager : MonoBehaviour
{
    [System.Serializable]
    public class AmbientItem
    {
        public string name;         // 名称（比如 “Rain”）
        public AudioClip clip;      // 白噪音音频文件
        public Button button;       // 控制按钮
    }

    [Header("环境音列表")]
    public List<AmbientItem> ambientSounds = new List<AmbientItem>();

    [Header("全局音量控制（滑动条）")]
    public Slider volumeSlider;
    [Range(0f, 1f)] public float defaultVolume = 0.5f;

    private AudioSource ambientSource;
    private AmbientItem currentPlaying;  // 当前正在播放的音源
    private bool isPlaying = false;

    void Start()
    {
        // 创建唯一的 AudioSource
        ambientSource = gameObject.AddComponent<AudioSource>();
        ambientSource.loop = true;
        ambientSource.playOnAwake = false;
        ambientSource.spatialBlend = 0f;
        ambientSource.volume = defaultVolume;

        // 初始化滑动条
        if (volumeSlider != null)
        {
            volumeSlider.value = defaultVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // 初始化每个按钮绑定
        foreach (var item in ambientSounds)
        {
            if (item.button != null)
            {
                string clipName = item.name; // 防闭包
                item.button.onClick.AddListener(() => OnAmbientButtonClicked(clipName));
            }
        }
    }

    /// <summary>
    /// 点击按钮播放对应环境音
    /// </summary>
    public void OnAmbientButtonClicked(string soundName)
    {
        // 找到对应音源
        AmbientItem item = ambientSounds.Find(s => s.name == soundName);
        if (item == null || item.clip == null)
        {
            Debug.LogWarning($"[AmbientAudioManager] 未找到环境音：{soundName}");
            return;
        }

        // 如果正在播放同一首，切换为停止
        if (currentPlaying == item && isPlaying)
        {
            StopAmbient();
            return;
        }

        // 播放新音源
        PlayAmbient(item);
    }

    /// <summary>
    /// 播放环境音
    /// </summary>
    private void PlayAmbient(AmbientItem item)
    {
        // 停止当前音源（如有）
        if (isPlaying) StopAmbient();

        currentPlaying = item;
        ambientSource.clip = item.clip;
        ambientSource.volume = volumeSlider ? volumeSlider.value : defaultVolume;
        ambientSource.Play();
        isPlaying = true;
        Debug.Log($"[AmbientAudioManager] 播放环境音：{item.name}");
    }

    /// <summary>
    /// 停止当前环境音
    /// </summary>
    public void StopAmbient()
    {
        if (isPlaying)
        {
            ambientSource.Stop();
            currentPlaying = null;
            isPlaying = false;
            Debug.Log("[AmbientAudioManager] 停止环境音");
        }
    }

    /// <summary>
    /// 滑动条控制音量
    /// </summary>
    public void SetVolume(float value)
    {
        ambientSource.volume = value;
    }

    /// <summary>
    /// 获取当前播放的音源名（可用于UI显示）
    /// </summary>
    public string GetCurrentAmbientName()
    {
        return currentPlaying != null ? currentPlaying.name : "None";
    }
}
