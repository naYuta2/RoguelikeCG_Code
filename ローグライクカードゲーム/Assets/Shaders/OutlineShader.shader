Shader "Custom/OutlineShader"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,1,0,1) //アウトラインの色
        _OutlineWidth ("Outline Width", Range(0.0, 0.1)) = 0.02 //アウトラインの太さ
    }
    SubShader
    {
        Tags {"RenderType"="Opaque"}

        Pass{
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }

            Cull Front // 裏面のみ描画

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t{
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f{
                float4 pos : SV_POSITION;
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            v2f vert(appdata_t v){
                v2f o;
                float3 normal = normalize(v.normal);
                v.vertex.xyz += normal * _OutlineWidth;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target{
                return _OutlineColor;
            }
            ENDCG
        }

        Pass{
            Name "BASE"
            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t{
                float4 vertex : POSITION
                float2 uv : TEXCOORD0;
            };

            struct v2f{
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _Color;

            v2f vert(appdata_t v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target{
                return tex2D(_MainTex, i.uv) * _Color;
            }
            ENDCG
        }
    }
}
