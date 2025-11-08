using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 全局音频管理器 AudioManager
/// 功能：BGM播放 / SFX音效播放 / 淡入淡出 / 音量控制
/// 挂载位置：任意场景中存在的 GameObject（建议命名为 "AudioManager"）
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("音量设置")]
    [Range(0f, 1f)] public float bgmVolume = 0.8f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("BGM 播放源")]
    public AudioSource bgmSource;

    [Header("SFX 播放源（池）")]
    public AudioSource sfxPrefab;
    private List<AudioSource> sfxPool = new List<AudioSource>();

    [Header("音频资源库")]
    public List<AudioClip> bgmClips;
    public List<AudioClip> sfxClips;

    private Dictionary<string, AudioClip> bgmDict = new();
    private Dictionary<string, AudioClip> sfxDict = new();

    void Awake()
    {
        // 单例
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 构建资源字典
        foreach (var clip in bgmClips)
            bgmDict[clip.name] = clip;
        foreach (var clip in sfxClips)
            sfxDict[clip.name] = clip;

        // 初始化BGM
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
        }
    }

    // ================== BGM ===================

    public void PlayBGM(string name, float fadeTime = 1f)
    {
        if (!bgmDict.ContainsKey(name))
        {
            Debug.LogWarning($"[AudioManager] 未找到 BGM：{name}");
            return;
        }

        AudioClip newClip = bgmDict[name];
        if (bgmSource.clip == newClip) return; // 已经在播放同一首

        StopAllCoroutines();
        StartCoroutine(FadeBGM(newClip, fadeTime));
    }

    IEnumerator FadeBGM(AudioClip newClip, float fadeTime)
    {
        // 淡出旧BGM
        float t = 0;
        float startVol = bgmSource.volume;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVol, 0, t / fadeTime);
            yield return null;
        }

        // 切换
        bgmSource.clip = newClip;
        bgmSource.Play();

        // 淡入新BGM
        t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0, bgmVolume, t / fadeTime);
            yield return null;
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // ================== SFX ===================

    public void PlaySFX(string name, Vector3? position = null, bool? loop = false)
    {
        if (!sfxDict.ContainsKey(name))
        {
            Debug.LogWarning($"[AudioManager] 未找到 SFX：{name}");
            return;
        }

        AudioSource src = GetAvailableSFXSource();
        src.transform.position = position ?? Vector3.zero;
        src.clip = sfxDict[name];
        src.loop = loop ?? false;
        src.volume = sfxVolume;
        src.spatialBlend = position == null ? 0f : 1f; // 若传入位置，则为3D音效
        src.Play();
        StartCoroutine(ReleaseAfterPlay(src));
    }

    private AudioSource GetAvailableSFXSource()
    {
        foreach (var sfx in sfxPool)
        {
            if (!sfx.isPlaying)
                return sfx;
        }

        AudioSource newSfx = Instantiate(sfxPrefab, transform);
        sfxPool.Add(newSfx);
        return newSfx;
    }

    IEnumerator ReleaseAfterPlay(AudioSource src)
    {
        yield return new WaitWhile(() => src.isPlaying);
        src.clip = null;
    }

    // ================== 通用 ===================

    public void SetBGMVolume(float v)
    {
        bgmVolume = v;
        bgmSource.volume = v;
    }

    public void SetSFXVolume(float v)
    {
        sfxVolume = v;
        foreach (var sfx in sfxPool)
            sfx.volume = v;
    }
}
