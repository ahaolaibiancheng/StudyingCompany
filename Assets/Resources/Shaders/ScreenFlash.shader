Shader "Custom/ScreenFlash"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlashAmount ("Flash Amount", Range(0, 1)) = 0
        _FlashColor ("Flash Color", Color) = (1, 1, 1, 1)
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _FlashAmount;
            fixed4 _FlashColor;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the main texture
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Apply flash effect by blending with flash color
                fixed4 flash = lerp(col, _FlashColor, _FlashAmount);
                
                // Optional: additive blending for more intense flash
                flash.rgb = col.rgb + _FlashColor.rgb * _FlashAmount;
                
                return flash;
            }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}
