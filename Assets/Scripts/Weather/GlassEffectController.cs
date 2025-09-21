using UnityEngine;

public class GlassEffectController : MonoBehaviour
{
    [SerializeField] private Material glassMaterial;
    [SerializeField] private Texture2D rainDropTexture; // 雨滴纹理
    [SerializeField] private Texture2D distortionTexture; // 扭曲纹理
    [SerializeField] private float[] rainIntensityValues = new float[] { 0f, 0.3f, 0.6f, 1f }; // 4个强度级别
    [SerializeField] private float[] dropSpeedValues = new float[] { 0f, 0.5f, 1f, 2f }; // 雨滴速度
    [SerializeField] private float[] dropSizeValues = new float[] { 1f, 1.2f, 1.5f, 2f }; // 雨滴大小
    [SerializeField] private float[] distortionValues = new float[] { 0f, 0.01f, 0.02f, 0.03f }; // 扭曲强度
    
    private void Start()
    {
        if (glassMaterial == null)
        {
            glassMaterial = GetComponent<Renderer>().material;
        }
        
        // 设置初始纹理
        if (rainDropTexture != null)
            glassMaterial.SetTexture("_RainDropTex", rainDropTexture);
        if (distortionTexture != null)
            glassMaterial.SetTexture("_DistortionTex", distortionTexture);
    }
    
    public void SetRainIntensity(int intensity)
    {
        if (glassMaterial == null || intensity < 0 || intensity >= rainIntensityValues.Length)
            return;
            
        // 设置雨量强度参数
        glassMaterial.SetFloat("_RainIntensity", rainIntensityValues[intensity]);
        glassMaterial.SetFloat("_DropSpeed", dropSpeedValues[intensity]);
        glassMaterial.SetFloat("_DropSize", dropSizeValues[intensity]);
        glassMaterial.SetFloat("_DistortionAmount", distortionValues[intensity]);
        
        // 设置湿润度 - 根据强度调整
        float wetness = intensity * 0.25f;
        glassMaterial.SetFloat("_Wetness", wetness);
    }
}
