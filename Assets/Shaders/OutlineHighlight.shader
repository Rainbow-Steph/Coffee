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
 
    [Header(Overlay Settings)]
    [Toggle] _UseOverlay ("Use as Overlay (Keep Original Material)", Float) = 1
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1" }
        LOD 200
        
        // First Pass: Render the outline (back faces)
  Pass
     {
      Name "OUTLINE"
       Tags { "LightMode" = "Always" }
            Cull Front
         ZWrite On
          ZTest LEqual
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
        
    // Second Pass: Render rim lighting overlay (front faces)
        Pass
    {
       Name "RIM_OVERLAY"
       Tags { "LightMode" = "ForwardBase" }
            Cull Back
    ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
            
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
         };
          
     struct v2f
      {
   float4 pos : SV_POSITION;
   float3 worldNormal : TEXCOORD0;
    float3 worldViewDir : TEXCOORD1;
            };
            
            fixed4 _OutlineColor;
         float _RimPower;
    float _RimIntensity;
            float _OutlineIntensity;
      float _UseOverlay;
  
            v2f vert(appdata v)
  {
         v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
          return o;
            }
 
            fixed4 frag(v2f i) : SV_Target
     {
      // Calculate rim lighting
      float3 worldNormal = normalize(i.worldNormal);
         float3 worldViewDir = normalize(i.worldViewDir);
         
    float rim = 1.0 - saturate(dot(worldViewDir, worldNormal));
     rim = pow(rim, _RimPower) * _RimIntensity;
  
       // Add rim lighting with outline color
       fixed3 rimColor = _OutlineColor.rgb * rim * _OutlineIntensity;
        
                // Output as overlay with alpha based on rim intensity
          float alpha = rim * _OutlineIntensity * 0.5;
       return fixed4(rimColor, alpha);
     }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}
