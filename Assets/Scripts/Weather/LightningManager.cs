using UnityEngine;
using System.Collections;

public class LightningManager : MonoBehaviour
{
    public static LightningManager Instance;
    
    [Header("Lightning Settings")]
    public Light directionalLight;           // 方向光引用
    public float minLightningInterval = 10f; // 最小闪电间隔
    public float maxLightningInterval = 30f; // 最大闪电间隔
    public float lightningDuration = 0.2f;   // 闪电持续时间
    public float maxLightIntensity = 3f;     // 最大光强度

    [Header("Thunder Settings")] 
    public AudioClip[] thunderClips;        // 雷声音频剪辑数组
    public AudioSource thunderAudioSource;  // 音频源
    public float minThunderDelay = 0.5f;
    public float maxThunderDelay = 3f;
    public float minThunderVolume = 0.3f;
    public float maxThunderVolume = 1f;
    
    [Header("Screen Flash Effect")]
    public Material screenFlashMaterial;    // 屏幕闪光材质
    public float flashIntensity = 0.8f;     // 闪光强度
    public float flashFadeSpeed = 2f;       // 闪光淡出速度
    
    [Header("Control Settings")]
    [SerializeField]
    private bool _autoTriggerLightning = true;
    public bool autoTriggerLightning 
    { 
        get { return _autoTriggerLightning; }
        set 
        { 
            if (_autoTriggerLightning != value)
            {
                _autoTriggerLightning = value;
                HandleAutoTriggerChange();
            }
        }
    }
    public bool enableThunder = true;
    
    private float originalLightIntensity;
    private bool isLightningActive = false;
    private float flashAmount = 0f;
    private Coroutine lightningRoutine;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        if (GetComponent<Camera>() == null)
        {
            Debug.LogError("LightningManager must be attached to a Camera object for screen flash effects to work!");
        }
        
        if (directionalLight != null)
        {
            originalLightIntensity = directionalLight.intensity;
        }
    }
    
    private void Start()
    {
        SetupLightningSystem();
        // 根据 autoTriggerLightning 状态决定是否启动
        if (autoTriggerLightning)
        {
            StartLightningSequence();
        }
    }

    void SetupLightningSystem()
    {
        // GameObject lightningObj = new GameObject("LightningManager");
        // IndependentLightningManager manager = lightningObj.AddComponent<IndependentLightningManager>();

        // 自动查找方向光
        this.directionalLight = FindObjectOfType<Light>();

        // 创建音频源
        this.thunderAudioSource = gameObject.AddComponent<AudioSource>();

        // 创建屏幕闪光材质
        this.screenFlashMaterial = new Material(Shader.Find("Custom/ScreenFlash"));   
    }

    private void HandleAutoTriggerChange()
    {
        if (_autoTriggerLightning)
        {
            StartLightningSequence();
        }
        else
        {
            StopLightningSequence();
        }
    }
    
    private void Update()
    {
        // Update screen flash effect
        if (screenFlashMaterial != null)
        {
            if (flashAmount > 0)
            {
                flashAmount = Mathf.Max(0, flashAmount - Time.deltaTime * flashFadeSpeed);
                screenFlashMaterial.SetFloat("_FlashAmount", flashAmount);
            }
        }
    }
    
    public void StartLightningSequence()
    {
        if (lightningRoutine != null)
        {
            StopCoroutine(lightningRoutine);
        }
        lightningRoutine = StartCoroutine(LightningRoutine());
    }
    
    public void StopLightningSequence()
    {
        // 启动自动闪电协程
        if (lightningRoutine != null)
        {
            StopCoroutine(lightningRoutine);
            lightningRoutine = null;
        }
    }
    
    private IEnumerator LightningRoutine()
    {
        // 按随机间隔触发闪电
        while (true) // 永远运行协程
        {
            if (autoTriggerLightning) // 只在启用时触发
            {
                // Wait for random interval
                float waitTime = Random.Range(minLightningInterval, maxLightningInterval);
                yield return new WaitForSeconds(waitTime);

                TriggerLightning();
            }
            else
            {
                // 如果未启用，短暂等待后继续检查
                yield return new WaitForSeconds(1f);
            }
        }
    }
    
    public void TriggerLightning()
    {
        // 触发单次闪电效果
        if (isLightningActive) return;
        
        StartCoroutine(LightningEffect());
    }
    
    private IEnumerator LightningEffect()
    {
        isLightningActive = true;
        
        // 两次闪光效果（模拟真实闪电）
        FlashLightning(0.1f, 0.7f); // 快速弱闪光
        yield return new WaitForSeconds(0.1f);
        FlashLightning(lightningDuration, 1f); // 主闪光
        
        // 屏幕闪光
        if (screenFlashMaterial != null)
        {
            flashAmount = flashIntensity;
            screenFlashMaterial.SetFloat("_FlashAmount", flashAmount);
        }
        
        // 延迟播放雷声
        if (enableThunder)
        {
            float thunderDelay = Random.Range(minThunderDelay, maxThunderDelay);
            yield return new WaitForSeconds(thunderDelay);
            PlayThunder();
        }
        
        isLightningActive = false;
    }
    
    private void FlashLightning(float duration, float intensityMultiplier)
    {
        if (directionalLight != null)
        {
            StartCoroutine(LightFlash(duration, intensityMultiplier));
        }
    }
    
    private IEnumerator LightFlash(float duration, float intensityMultiplier)
    {
        // 平滑改变光照强度模拟闪光
        float elapsed = 0f;
        float targetIntensity = maxLightIntensity * intensityMultiplier;
        
        // 闪光上升
        while (elapsed < duration / 2)
        {
            directionalLight.intensity = Mathf.Lerp(originalLightIntensity, targetIntensity, elapsed / (duration / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 闪光下降
        elapsed = 0f;
        while (elapsed < duration / 2)
        {
            directionalLight.intensity = Mathf.Lerp(targetIntensity, originalLightIntensity, elapsed / (duration / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        directionalLight.intensity = originalLightIntensity;
    }
    
    private void PlayThunder()
    {
        if (thunderAudioSource != null && thunderClips.Length > 0)
        {
            AudioClip clip = thunderClips[Random.Range(0, thunderClips.Length)];
            float volume = Random.Range(minThunderVolume, maxThunderVolume);
            
            thunderAudioSource.PlayOneShot(clip, volume);
        }
    }
    
    public void SetLightningFrequency(float minInterval, float maxInterval)
    {
        minLightningInterval = minInterval;
        maxLightningInterval = maxInterval;
    }
    
    public void SetThunderSettings(bool enable, float minDelay, float maxDelay, float minVol, float maxVol)
    {
        enableThunder = enable;
        minThunderDelay = minDelay;
        maxThunderDelay = maxDelay;
        minThunderVolume = minVol;
        maxThunderVolume = maxVol;
    }
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // 使用材质处理屏幕渲染实现闪光效果
        if (screenFlashMaterial != null && flashAmount > 0)
        {
            Graphics.Blit(source, destination, screenFlashMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
