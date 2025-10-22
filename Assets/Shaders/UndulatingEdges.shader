Shader "Custom/UI/UndulatingEdges"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

    [Header(Wave Settings)]
 _Amplitude ("Wave Amplitude", Range(0, 0.1)) = 0.02
        _Frequency ("Wave Frequency", Range(0, 50)) = 10
   _Speed ("Wave Speed", Range(-10, 10)) = 2
        
        [Header(Edge Settings)]
     _EdgeWidth ("Edge Width", Range(0, 0.5)) = 0.1
   _EdgeSoftness ("Edge Softness", Range(0, 1)) = 0.1
        _EdgeColor ("Edge Color", Color) = (1,1,1,1)

        // Required for UI
 _StencilComp ("Stencil Comparison", Float) = 8
 _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
{
        Tags
   {
      "Queue"="Transparent"
     "IgnoreProjector"="True"
     "RenderType"="Transparent"
     "PreviewType"="Plane"
  "CanUseSpriteAtlas"="True"
     }

        Stencil
        {
       Ref [_Stencil]
         Comp [_StencilComp]
    Pass [_StencilOp]
        ReadMask [_StencilReadMask]
       WriteMask [_StencilWriteMask]
        }

        Cull Off
      Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
 Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
      Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
   #include "UnityCG.cginc"
     #include "UnityUI.cginc"

struct appdata_t
{
     float4 vertex : POSITION;
   float2 texcoord : TEXCOORD0;
     float4 color : COLOR;
  };

 struct v2f
            {
  float4 vertex : SV_POSITION;
       float2 texcoord : TEXCOORD0;
         float4 worldPosition : TEXCOORD1;
                float4 color : COLOR;
            };

 sampler2D _MainTex;
            float4 _MainTex_ST;
        fixed4 _Color;
     float _Amplitude;
          float _Frequency;
            float _Speed;
        float _EdgeWidth;
      float _EdgeSoftness;
     fixed4 _EdgeColor;
            float4 _ClipRect;

            v2f vert(appdata_t v)
      {
             v2f OUT;

           // Calculate wave distortion
            float2 uv = v.texcoord;
     float time = _Time.y * _Speed;

             // Calculate distance from edges
                float edgeDistX = min(uv.x, 1 - uv.x);
      float edgeDistY = min(uv.y, 1 - uv.y);
  float edgeFactor = saturate(1 - (edgeDistX + edgeDistY) / _EdgeWidth);

           // Apply sine wave distortion near edges
          float waveX = sin(uv.y * _Frequency + time) * _Amplitude * edgeFactor;
                float waveY = sin(uv.x * _Frequency + time) * _Amplitude * edgeFactor;
            
     // Apply distortion
      v.vertex.x += waveX;
     v.vertex.y += waveY;

     OUT.worldPosition = v.vertex;
       OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
         OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
         OUT.color = v.color * _Color;

    return OUT;
   }

     fixed4 frag(v2f IN) : SV_Target
     {
        // Sample base texture
            half4 color = tex2D(_MainTex, IN.texcoord) * IN.color;

  // Calculate edge factor for color blending
  float2 center = abs(IN.texcoord - 0.5) * 2;
                float edgeDist = max(center.x, center.y);
       float edgeAlpha = smoothstep(1 - _EdgeWidth - _EdgeSoftness, 
         1 - _EdgeWidth + _EdgeSoftness, 
           edgeDist);

    // Blend with edge color
    color.rgb = lerp(color.rgb, _EdgeColor.rgb, edgeAlpha * _EdgeColor.a);

         // Apply UI clipping
         color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

        return color;
         }
            ENDCG
        }
    }
}