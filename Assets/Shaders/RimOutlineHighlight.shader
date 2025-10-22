Shader "Custom/RimOutlineHighlight"
{
    Properties
    {
 _Color ("Main Color", Color) = (1,1,1,1)
     _MainTex ("Base (RGB)", 2D) = "white" {}
        
        [Header(Rim Outline Settings)]
        _RimColor ("Rim Color", Color) = (1,0.5,0,1)
     _RimPower ("Rim Power", Range(0.1, 8.0)) = 3.0
    _RimIntensity ("Rim Intensity", Range(0, 10)) = 3.0
 }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        CGPROGRAM
 #pragma surface surf Lambert
 
        sampler2D _MainTex;
        fixed4 _Color;
    fixed4 _RimColor;
        float _RimPower;
        float _RimIntensity;
   
  struct Input
        {
            float2 uv_MainTex;
       float3 viewDir;
};
        
        void surf (Input IN, inout SurfaceOutput o)
        {
       // Sample texture
  fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
 o.Albedo = c.rgb;
          o.Alpha = c.a;
     
          // Calculate rim effect
          half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            rim = pow(rim, _RimPower);
      
          // Apply rim as emission (makes it glow)
    o.Emission = _RimColor.rgb * rim * _RimIntensity;
   }
        ENDCG
    }
    
    FallBack "Diffuse"
}
