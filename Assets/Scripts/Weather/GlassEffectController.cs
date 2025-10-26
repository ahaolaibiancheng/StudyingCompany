using UnityEngine;

public class GlassEffectController : MonoBehaviour
{
    [SerializeField] private Material glassMaterial;
    [SerializeField] private float[] dropSpeedValues = new float[] { 0f, 0.1f, 0.2f, 0.4f }; // 雨滴速度
    [SerializeField] private float[] dynamicSizeValues = new float[] { 0f, 2f, 3f, 5f }; // 雨滴大小
    [SerializeField] private float[] staticSizeValues = new float[] { 0f, 0.5f, 2f, 5f }; // 扭曲强度
    public RainIntensity currentRainIntensity = RainIntensity.None;

    private void Awake()
    {
        glassMaterial = GetComponent<Renderer>().material;
    }

    public void SetRainIntensity(RainIntensity intensity)
    {
        if (glassMaterial == null ||
            (int)intensity > dropSpeedValues.Length - 1 ||
            (int)intensity > dynamicSizeValues.Length - 1 ||
            (int)intensity > staticSizeValues.Length - 1)
            return;

        // 设置雨量强度参数
        glassMaterial.SetFloat("_RainSpeed", dropSpeedValues[(int)intensity]);
        glassMaterial.SetFloat("_DynamicDrops", dynamicSizeValues[(int)intensity]);
        glassMaterial.SetFloat("_StaticDrops", staticSizeValues[(int)intensity]);
    }

    public void UpdateGlassEffects(RainIntensity intensity)
    {
        currentRainIntensity = intensity;
        SetRainIntensity(currentRainIntensity);
    }
}

public enum RainIntensity
{
    None,
    Light,
    Medium,
    Heavy
}
