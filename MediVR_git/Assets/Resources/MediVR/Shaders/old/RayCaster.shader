// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MediVR/RayCastingRendering" {
	
	Properties {
		// the data cube
		[NoScaleOffset] _Data ("Data Texture", 3D) = "" {}
		// data slicing and thresholding
		_SliceAxis1Min ("Slice along axis 1: min", Range(0,1)) = 0
		_SliceAxis1Max ("Slice along axis 1: max", Range(0,1)) = 1
		_SliceAxis2Min ("Slice along axis 2: min", Range(0,1)) = 0
		_SliceAxis2Max ("Slice along axis 2: max", Range(0,1)) = 1
		_SliceAxis3Min ("Slice along axis 3: min", Range(0,1)) = 0
		_SliceAxis3Max ("Slice along axis 3: max", Range(0,1)) = 1
		_DataMin ("Data threshold: min", Range(0,1)) = 0
		_DataMax ("Data threshold: max", Range(0,1)) = 1
		_Iterations ("RayMarching Iterations", int) = 2048
		// normalization of data intensity (has to be adjusted for each data set, also depends on the number of steps)
		_Normalization ("Intensity normalization", Float) = 1 
	}

	SubShader {

		Pass {

			CGPROGRAM
	        #pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler3D _Data;
			float _SliceAxis1Min, _SliceAxis1Max;
			float _SliceAxis2Min, _SliceAxis2Max;
			float _SliceAxis3Min, _SliceAxis3Max;
			float _DataMin, _DataMax;
			int _Iterations;
			float _Normalization;

			// calculates intersection between a ray and a box
			// http://www.siggraph.org/education/materials/HyperGraph/raytrace/rtinter3.htm
			bool IntersectBox(float3 ray_o, float3 ray_d, float3 boxMin, float3 boxMax, out float tNear, out float tFar)
			{
			    // compute intersection of ray with all six bbox planes
			    float3 invR = 1.0 / ray_d;
			    float3 tBot = invR * (boxMin.xyz - ray_o);
			    float3 tTop = invR * (boxMax.xyz - ray_o);
			    // re-order intersections to find smallest and largest on each axis
			    float3 tMin = min (tTop, tBot);
			    float3 tMax = max (tTop, tBot);
			    // find the largest tMin and the smallest tMax
			    float2 t0 = max (tMin.xx, tMin.yz);
			    float largest_tMin = max (t0.x, t0.y);
			    t0 = min (tMax.xx, tMax.yz);
			    float smallest_tMax = min (t0.x, t0.y);
			    // check for hit
			    bool hit = (largest_tMin <= smallest_tMax);
			    tNear = largest_tMin;
			    tFar = smallest_tMax;
			    return hit;
			}

			struct vert_input {
			    float4 pos : POSITION;
			};

			struct frag_input {
			    float4 pos : SV_POSITION;
			    float3 ray_o : TEXCOORD1; // ray origin
			    float3 ray_d : TEXCOORD2; // ray direction
			};

			// vertex program
			frag_input vert(vert_input i)
			{
				frag_input o;

			    // calculate eye ray in object space
				o.ray_d = -ObjSpaceViewDir(i.pos);
				o.ray_o = i.pos.xyz - o.ray_d;
				//o.ray_o = _WorldSpaceCameraPos;
				// calculate position on screen (unused)
				o.pos = UnityObjectToClipPos(i.pos);
				//o.pos = mul(unity_WorldToObject, i.pos);

				return o;
			}

			// gets data value at a given position
			float4 get_data(float3 pos) {
				// sample texture (pos is normalized in [0,1])
				float3 pos_righthanded = float3(pos.x,pos.z,pos.y);
				//float data = tex3D(_Data, pos_righthanded).a;
				float4 data = tex3Dlod(_Data, float4(pos_righthanded,0));
				// slice and threshold
				data.a *= step(_SliceAxis1Min, pos.x);
				data.a *= step(_SliceAxis2Min, pos.y);
				data.a *= step(_SliceAxis3Min, pos.z);
				data.a *= step(pos.x, _SliceAxis1Max);
				data.a *= step(pos.y, _SliceAxis2Max);
				data.a *= step(pos.z, _SliceAxis3Max);
				data.a *= step(_DataMin, data);
				data.a *= step(data.a, _DataMax);
				// colourize
				float4 col = float4(data.a, data.a, data.a, data.a);
				return data;
				//return col;
			}

#define STEP_CNT _Iterations // should ideally be at least as large as data resolution, but strongly affects frame rate
			
			// fragment program
			float4 frag(frag_input i) : COLOR
			{
			    i.ray_d = normalize(i.ray_d);
			    // calculate eye ray intersection with cube bounding box
				float3 boxMin = { -0.5, -0.5, -0.5 };
				float3 boxMax = {  0.5,  0.5,  0.5 };
			    float tNear, tFar;
			    bool hit = IntersectBox(i.ray_o, i.ray_d, boxMin, boxMax, tNear, tFar);
			    if (!hit) discard;
			    if (tNear < 0.0) tNear = 0.0;
			    // calculate intersection points
			    float3 pNear = i.ray_o + i.ray_d*tNear;
			    float3 pFar  = i.ray_o + i.ray_d*tFar;
			    // convert to texture space
				pNear = pNear + 0.5;
				pFar  = pFar  + 0.5;
				
			    // march along ray inside the cube, accumulating color
				float3 ray_pos = pNear;
				float3 ray_dir = pFar - pNear;

				float3 ray_step = normalize(ray_dir) * sqrt(3) / STEP_CNT;
				float4 ray_col = 0;

				for(int k = 0; k < STEP_CNT; k++)
				{
					float4 voxel_col = get_data(ray_pos);

					ray_col.rgb = ray_col.rgb + (1 - ray_col.a) * voxel_col.a * voxel_col.rgb;
					ray_col.a   = ray_col.a   + (1 - ray_col.a) * voxel_col.a;

					ray_pos += ray_step;

					if (ray_pos.x < 0 || ray_pos.y < 0 || ray_pos.z < 0) break;
					if (ray_pos.x > 1 || ray_pos.y > 1 || ray_pos.z > 1) break;
				}

				//if(ray_col.r < 0.08 && ray_col.g < 0.08 && ray_col.b < 0.08)
				//	discard;

					//NEW


				if(ray_col.r < 0.08 && ray_col.g < 0.08 && ray_col.b < 0.08)
				discard;
              
                ray_col = (ray_col * 4095) - 1024;

                if(ray_col.r > 400 && ray_col.g > 400 && ray_col.b > 400)
                {
                    ray_col = 400;
                }
                else if(ray_col.r < -300 && ray_col.g < -300 && ray_col.b < -300)
                {
                    ray_col = -300;
                }
                else if((ray_col.r > -300 && ray_col.r < 400) && (ray_col.g > -300 && ray_col.g < 400) && (ray_col.b > -300 && ray_col.b < 400))
                {
                    ray_col = ray_col;
                }

                ray_col = (ray_col + 300) / 350;

					//END OF NEW

		    	return ray_col*_Normalization;
			}

			ENDCG

		}

	}

	FallBack Off
}