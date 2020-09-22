Shader "Unlit/shaderTry2"
{
    Properties
    {
        _MainTex ("Texture", 3D) = "white" {}
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

            #define MAX_STEPS 100
            #define MAX_DIST 100
            #define SURF_DIST 0.001 //1e-3

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 ro : TEXCOORD1;
                float3 hitPos : TEXCOORD2;
            };

            sampler3D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.ro = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
                o.hitPos = v.vertex;
                return o;
            }

            float GetDist(float3 p)
            {
                float d = length(p) - .5; //sphere
                d = length(float2(length(p.xz) - .5, p.y)) - .1;

                return d;
            }

            float Raymarch(float3 ro, float3 rd) 
            {
                float dO = 0;
                float dS;

                for(int i = 0; i < MAX_STEPS; i++)
                {
                    float3 p = ro + dO * rd;
                    dS = GetDist(p);
                    dO += dS;
                    if(dS<SURF_DIST || dO>MAX_DIST) break;                
                }

                return dO;
            }

            float3 GetNormal(float3 p)
            {
                float2 e = float2(0.01, 0);
                float3 n = GetDist(p) - float3(GetDist(p-e.xyy), GetDist(p-e.yxy), GetDist(p-e.yyx));
                return normalize(n);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv-.5;
                float3 ro = i.ro;//float3(0,0,-3 );
                float3 rd = normalize(i.hitPos - ro);//normalize(float3(uv.x, uv.y, 1));
                //loat3 rd


                float d = Raymarch(ro, rd);
                float4 col = 0;
                

                if(d < MAX_DIST)
                {
                    float3 p = ro + rd * d;
                    float3 n = GetNormal(p);
                    float4 color = (tex3Dlod(_MainTex, (p,0)));
                    //col.rgb = n;
                }
                else
                {
                    discard;
                }

                //col.rgb = rd;
                return col;
            }
            ENDCG
        }
    }
}
