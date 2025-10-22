Shader "Custom/OutlineHighlight"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        
      [Header(Outline Settings)]
        _OutlineColor ("Outline Color", Color) = (1,0.5,0,1)
        _OutlineWidth ("Outline Width", Range(0.0, 0.1)) = 0.03
    _OutlineIntensity ("Outline Intensity", Range(0, 5)) = 2.0
        
 [Header(Rim Light Settings)]
        _RimPower ("Rim Power", Range(0.1, 8.0)) = 3.0
        _RimIntensity ("Rim Intensity", Range(0, 3)) = 1.5
    }
    
    SubShader
    {
     Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200
        
        // First Pass: Render the outline
        Pass
 {
            Name "OUTLINE"
          Tags { "LightMode" = "Always" }
            Cull Front
            ZWrite On
ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha
            
    CGPROGRAM
            #pragma vertex vert
     #pragma fragment frag
            #include "UnityCG.cginc"
  
            struct appdata
            {
      float4 vertex : POSITION;
       float3 normal : NORMAL;
            };
            
            struct v2f
            {
     float4 pos : SV_POSITION;
fixed4 color : COLOR;
       };
       
         uniform float _OutlineWidth;
            uniform float4 _OutlineColor;
        uniform float _OutlineIntensity;
          
 v2f vert(appdata v)
        {
        v2f o;
       
    // Expand the vertex along the normal to create outline
          float3 norm = normalize(v.normal);
     float3 expandedPos = v.vertex.xyz + norm * _OutlineWidth;
      
o.pos = UnityObjectToClipPos(float4(expandedPos, 1));
          o.color = _OutlineColor * _OutlineIntensity;
        
           return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
  {
                return i.color;
  }
            ENDCG
        }
        
   // Second Pass: Render the object with rim lighting
        Pass
        {
            Name "BASE"
            Tags { "LightMode" = "ForwardBase" }
  
   CGPROGRAM
            #pragma vertex vert
       #pragma fragment frag
        #pragma multi_compile_fwdbase
      #include "UnityCG.cginc"
        #include "Lighting.cginc"
    
struct appdata
            {
  float4 vertex : POSITION;
       float3 normal : NORMAL;
 float2 uv : TEXCOORD0;
       };
            
         struct v2f
     {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
       float3 worldNormal : TEXCOORD1;
         float3 worldViewDir : TEXCOORD2;
   float3 worldPos : TEXCOORD3;
          };
          
            sampler2D _MainTex;
float4 _MainTex_ST;
            fixed4 _Color;
     fixed4 _OutlineColor;
            float _RimPower;
            float _RimIntensity;
            float _OutlineIntensity;
            
       v2f vert(appdata v)
 {
                v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
     o.uv = TRANSFORM_TEX(v.uv, _MainTex);
o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
           o.worldNormal = UnityObjectToWorldNormal(v.normal);
          o.worldViewDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
            return o;
      }
     
     fixed4 frag(v2f i) : SV_Target
      {
        // Sample the texture
      fixed4 texColor = tex2D(_MainTex, i.uv) * _Color;
       
    // Calculate rim lighting
      float3 worldNormal = normalize(i.worldNormal);
        float3 worldViewDir = normalize(i.worldViewDir);

       float rim = 1.0 - saturate(dot(worldViewDir, worldNormal));
                rim = pow(rim, _RimPower) * _RimIntensity;
          
        // Add rim lighting with outline color
    fixed3 rimColor = _OutlineColor.rgb * rim * _OutlineIntensity;
          
         // Simple diffuse lighting
  float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
             float NdotL = max(0, dot(worldNormal, lightDir));
       fixed3 diffuse = texColor.rgb * _LightColor0.rgb * NdotL;
             
     // Combine ambient, diffuse, and rim
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb * texColor.rgb;
       fixed3 finalColor = ambient + diffuse + rimColor;
    
      return fixed4(finalColor, texColor.a);
       }
    ENDCG
        }
    }
  
    FallBack "Diffuse"
}
