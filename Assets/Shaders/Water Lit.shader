Shader "Water/Ocean Lit"
{
		Properties
		{
			// color of the water
			_Color("Color", Color) = (1, 1, 1, 1)
			_MainTex("Texture", 2D) = "white" {}
			// color of the edge effect
			_EdgeColor("Edge Color", Color) = (1, 1, 1, 1)
			// width of the edge effect
			_DepthFactor("Depth Factor", float) = 1.0

			_WaveSpeed("Wave Speed", float) = 1.0
			_WaveAmp("Wave Amp", float) = 0.2
			_NoiseTex("Noise Texture", 2D) = "white" {}
			_ExtraHeight("Extra Height", float) = 0.0
		}
		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
			}
			Pass
			{
				Tags{ "LightMode" = "ForwardBase" }
				Blend SrcAlpha OneMinusSrcAlpha
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "UnityStandardBRDF.cginc"

				// compile shader into multiple variants, with and without shadows
				// (we don't care about any lightmaps yet, so skip these variants)
				#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
				// shadow helper functions and macros
				#include "AutoLight.cginc"

				// Unity built-in - NOT required in Properties
				sampler2D _CameraDepthTexture;

				sampler2D _NoiseTex;
				sampler2D _MainTex;
				float4 _NoiseTex_ST;
				float4 _Color;
				float4 _EdgeColor;
				float _DepthFactor;
				float _WaveSpeed;
				float _WaveAmp;
				float _ExtraHeight;

				struct vertexInput
				{
					float4 vertex : POSITION;
					float3 texCoord : TEXCOORD0;
					float3 normal : NORMAL;

				};

				struct vertexOutput
				{
					float2 uv : TEXCOORD0;
					SHADOW_COORDS(1) // put shadows data into TEXCOORD1
					fixed3 diff : COLOR0;
					fixed3 ambient : COLOR1;
					float4 pos : SV_POSITION;
					float4 screenPos : TEXCOORD3;
					float3 normal : TEXCOORD2;
				};

				vertexOutput vert(vertexInput input)
				{
					vertexOutput output;

					//Waves
					// convert obj-space position to camera clip space
					output.pos = UnityObjectToClipPos(input.vertex);
					//float2 noiseUV = TRANSFORM_TEX(input.texCoord, _NoiseTex);

					float noiseSample = tex2Dlod(_NoiseTex, float4(input.texCoord.xy, 0, 0));
					output.pos.y += sin(_Time*_WaveSpeed*noiseSample)*_WaveAmp + _ExtraHeight;
					output.pos.x += cos(_Time*_WaveSpeed*noiseSample)*_WaveAmp;

					// compute depth (screenPos is a float4)

					output.normal = UnityObjectToWorldNormal(input.normal);
					//Lighting and Shadows (No fucking clue what this does)
					//output.pos = UnityObjectToClipPos(input.vertex);
					output.uv = input.texCoord;
					half3 worldNormal = UnityObjectToWorldNormal(input.normal);
					half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
					output.diff = nl * _LightColor0.rgb;
					output.ambient = ShadeSH9(half4(worldNormal,1));

					output.screenPos = ComputeScreenPos(output.pos);

					// compute shadows data
					TRANSFER_SHADOW(output)

					return output;
				}


				float4 frag(vertexOutput input) : COLOR
				{
					//Foam and color
					input.normal = normalize(input.normal);

					float4 depthSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, input.screenPos);
					float depth = LinearEyeDepth(depthSample).r;

					// apply the DepthFactor to be able to tune at what depth values
					// the foam line actually starts
					float foamLine = 1 - saturate(_DepthFactor * (depth - input.screenPos.w));

					// multiply the edge color by the foam factor to get the edge,
					// then add that to the color of the water
					//float4 foamRamp = float4(tex2D(_DepthRampTex, float2(foamLine, 0.5)).rgb, 1.0);
					float4 col = _Color + foamLine * _EdgeColor;

					
					// compute shadow attenuation (1.0 = fully lit, 0.0 = fully shadowed)
					fixed shadow = SHADOW_ATTENUATION(input);
					// darken light's illumination with shadow, keep ambient intact
					fixed3 lighting = input.diff * shadow + input.ambient;
					col.rgb *= lighting;

					return col;
				}
				ENDCG
			}

				// shadow casting support
				//UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
			}
	}
