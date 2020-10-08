Shader "MediVR/Old/Old3"
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

            #define MAX_STEPS 256
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
                //o.ro = v.vertex - o.vertex;
                o.hitPos = v.vertex;
                //o.hitPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float GetDist(float3 p)
            {
                float d = length(p) - 5; //sphere
                //d = length(float2(length(p.xz) - .5, p.y)) - .1;

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

            // gets data value at a given position
			float4 get_data(float3 pos) {
				// sample texture (pos is normalized in [0,1])
				float3 pos_righthanded = float3(pos.x,pos.z,pos.y);
				//float data = tex3D(_Data, pos_righthanded).a;
				float4 data = tex3Dlod(_MainTex, float4(pos_righthanded,0));
				// slice and threshold
				//data.a *= step(_SliceAxis1Min, pos.x);
				//data.a *= step(_SliceAxis2Min, pos.y);
				//data.a *= step(_SliceAxis3Min, pos.z);
				//data.a *= step(pos.x, _SliceAxis1Max);
				//data.a *= step(pos.y, _SliceAxis2Max);
				//data.a *= step(pos.z, _SliceAxis3Max);
				//data.a *= step(0, data);
				//data.a *= step(data.a, _DataMax);
				// colourize
				float4 col = float4(data.a, data.a, data.a, data.a);
				return data;
				//return col;
			}

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv-.5;
                float3 ro = i.ro;//float3(0,0,-3 );
                float3 rd = normalize(i.hitPos - ro);//normalize(float3(uv.x, uv.y, 1));
                //loat3 rd


                float d = Raymarch(ro, rd);
                float4 ray_col = 0;
                

                if(d < MAX_DIST)
                {
                    float3 p = ro + rd * d;
                    float4 voxel_col = tex3Dlod(_MainTex, float4(p,0));
                    //float4 color = (tex3Dlod(_MainTex, (p,0)));
                    //col.rgb = n;
					ray_col.rgb = ray_col.rgb + (1 - ray_col.a) * voxel_col.a * voxel_col.rgb;
					ray_col.a   = ray_col.a   + (1 - ray_col.a) * voxel_col.a;

                    //if(ray_col.r < 0.08 && ray_col.g < 0.08 && ray_col.b < 0.08)
					//discard;
                }
                else
                {
                    discard;
                }

                //col.rgb = rd;
                return ray_col;
            }
            ENDCG
        }
    }
}
