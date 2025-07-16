// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TidalFlask/Top Projection Noise Standard"
{
	Properties
	{
		[NoScaleOffset]_BaseTexture("Base Texture", 2D) = "white" {}
		_BaseTextureTint("Base Texture Tint", Color) = (1,1,1,0)
		_BaseTextureSaturation("Base Texture Saturation", Range( 0 , 2)) = 1
		[NoScaleOffset]_TopTexture("Top Texture", 2D) = "white" {}
		_TopTextureTint("Top Texture Tint", Color) = (1,1,1,0)
		_TopTextureTiling("Top Texture Tiling", Float) = 1
		_CoverageAmount("Coverage Amount", Range( -2 , 1)) = -1
		_CoverageFalloff("Coverage Falloff", Range( 0.01 , 2)) = 0.15
		_CoverageNoiseScale("Coverage Noise Scale", Range( 0 , 5)) = 0.85
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float4 _BaseTextureTint;
		uniform sampler2D _BaseTexture;
		uniform float _BaseTextureSaturation;
		uniform float4 _TopTextureTint;
		uniform sampler2D _TopTexture;
		uniform float _TopTextureTiling;
		uniform float _CoverageNoiseScale;
		uniform float _CoverageAmount;
		uniform float _CoverageFalloff;
		uniform float _Smoothness;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BaseTexture1 = i.uv_texcoord;
			float3 desaturateInitialColor131 = ( _BaseTextureTint * tex2D( _BaseTexture, uv_BaseTexture1 ) ).rgb;
			float desaturateDot131 = dot( desaturateInitialColor131, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar131 = lerp( desaturateInitialColor131, desaturateDot131.xxx, ( 1.0 - _BaseTextureSaturation ) );
			float3 ase_worldPos = i.worldPos;
			float3 break109 = ase_worldPos;
			float2 appendResult113 = (float2(break109.x , break109.z));
			float3 break152 = ase_worldPos;
			float2 appendResult154 = (float2(break152.x , break152.z));
			float simplePerlin2D148 = snoise( (appendResult154*_CoverageNoiseScale + 0.0) );
			simplePerlin2D148 = simplePerlin2D148*0.5 + 0.5;
			float3 ase_worldNormal = i.worldNormal;
			float temp_output_96_0 = ( ase_worldNormal.y + _CoverageAmount );
			float4 lerpResult62 = lerp( float4( desaturateVar131 , 0.0 ) , ( _TopTextureTint * tex2D( _TopTexture, (appendResult113*_TopTextureTiling + 0.0) ) ) , saturate( ( pow( saturate( ( simplePerlin2D148 + temp_output_96_0 ) ) , _CoverageFalloff ) + pow( saturate( ( simplePerlin2D148 + ( temp_output_96_0 * 0.85 ) ) ) , _CoverageFalloff ) ) ));
			o.Albedo = lerpResult62.rgb;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18912
645;538;1906;786;2389.767;1109.108;1.352628;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;151;-3207.71,820.9293;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BreakToComponentsNode;152;-2961.877,817.5952;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;156;-2933.018,1334.458;Inherit;False;Property;_CoverageNoiseScale;Coverage Noise Scale;8;0;Create;True;0;0;0;False;0;False;0.85;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-2570.435,1333.624;Float;False;Property;_CoverageAmount;Coverage Amount;6;0;Create;True;0;0;0;False;0;False;-1;-0.18;-2;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;124;-2544.395,1090.82;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;154;-2807.821,817.2133;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;155;-2590.488,817.2813;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;185;-2419.333,1679.278;Inherit;False;Constant;_Float1;Float 1;13;0;Create;True;0;0;0;False;0;False;0.85;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;96;-2209.842,1099.329;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;148;-2315.895,810.8533;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;184;-2207.982,1660.944;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;91;-2297.292,152.0532;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;183;-1983.496,1649.919;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;162;-1990.376,816.3273;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;109;-2051.46,148.7199;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SaturateNode;192;-1706.589,1649.677;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;113;-1897.404,148.3374;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;190;-1825.252,1327.56;Float;False;Property;_CoverageFalloff;Coverage Falloff;7;0;Create;True;0;0;0;False;0;False;0.15;0.65;0.01;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;189;-1720.791,817.3512;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;135;-1970.71,300.1143;Inherit;False;Property;_TopTextureTiling;Top Texture Tiling;5;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1394.053,-665.731;Inherit;True;Property;_BaseTexture;Base Texture;0;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;1519e3b659b5fc248abd68be06f3429e;1519e3b659b5fc248abd68be06f3429e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;193;-1485.502,1647.625;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;191;-1499.704,815.2997;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;4;-1326.402,-844.3707;Inherit;False;Property;_BaseTextureTint;Base Texture Tint;1;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleAndOffsetNode;134;-1688.072,151.4054;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;132;-1334.563,-1001.888;Inherit;False;Property;_BaseTextureSaturation;Base Texture Saturation;2;0;Create;True;0;0;0;False;0;False;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;188;-1105.804,819.2704;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;136;-1028.7,-997.9599;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;128;-1305.033,-53.33077;Inherit;False;Property;_TopTextureTint;Top Texture Tint;4;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-1014.091,-683.6935;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;117;-1390.874,124.3689;Inherit;True;Property;_TopTexture;Top Texture;3;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;a36684cfc68dfce44af56f21e48ee77f;a36684cfc68dfce44af56f21e48ee77f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;129;-995.1082,26.77634;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DesaturateOpNode;131;-713.2202,-683.1989;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;194;-909.4329,817.6506;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;607.6833,337.3765;Float;False;Property;_Smoothness;Smoothness;9;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;62;-448.394,2.672567;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;921.8478,2.716273;Float;False;True;-1;6;;0;0;Standard;TidalFlask/Top Projection Noise Standard;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;16;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;152;0;151;0
WireConnection;154;0;152;0
WireConnection;154;1;152;2
WireConnection;155;0;154;0
WireConnection;155;1;156;0
WireConnection;96;0;124;2
WireConnection;96;1;71;0
WireConnection;148;0;155;0
WireConnection;184;0;96;0
WireConnection;184;1;185;0
WireConnection;183;0;148;0
WireConnection;183;1;184;0
WireConnection;162;0;148;0
WireConnection;162;1;96;0
WireConnection;109;0;91;0
WireConnection;192;0;183;0
WireConnection;113;0;109;0
WireConnection;113;1;109;2
WireConnection;189;0;162;0
WireConnection;193;0;192;0
WireConnection;193;1;190;0
WireConnection;191;0;189;0
WireConnection;191;1;190;0
WireConnection;134;0;113;0
WireConnection;134;1;135;0
WireConnection;188;0;191;0
WireConnection;188;1;193;0
WireConnection;136;0;132;0
WireConnection;2;0;4;0
WireConnection;2;1;1;0
WireConnection;117;1;134;0
WireConnection;129;0;128;0
WireConnection;129;1;117;0
WireConnection;131;0;2;0
WireConnection;131;1;136;0
WireConnection;194;0;188;0
WireConnection;62;0;131;0
WireConnection;62;1;129;0
WireConnection;62;2;194;0
WireConnection;0;0;62;0
WireConnection;0;4;61;0
ASEEND*/
//CHKSM=F42CA39008678D4BB53538E41944AE2F90EF11C4