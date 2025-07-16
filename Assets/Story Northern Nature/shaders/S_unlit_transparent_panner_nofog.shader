// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TidalFlask/Unlit Transparent Panner NoFog"
{
	Properties
	{
		[NoScaleOffset]_BaseTexture("Base Texture", 2D) = "white" {}
		_BaseTextureTint("Base Texture Tint", Color) = (1,1,1,0)
		_BaseTextureStartOffset("Base Texture Start Offset", Range( 0 , 1)) = 0
		_BaseTextureTilingX("Base Texture Tiling X", Range( 0 , 1)) = 1
		_BaseTextureTilingY("Base Texture Tiling Y", Range( 0 , 1)) = 1
		[NoScaleOffset]_GradientMaskL("Gradient Mask L", 2D) = "white" {}
		[NoScaleOffset]_GradientMaskR("Gradient Mask R", 2D) = "white" {}
		_PannerSpeed("Panner Speed", Float) = 0.02
		_TextureVSNoiseSpeedRatio("Texture VS Noise Speed Ratio", Range( -2 , 2)) = 1.2
		_NoiseFalloff("Noise Falloff", Range( 0 , 5)) = 3
		_NoiseScale("Noise Scale", Float) = 2
		_NoiseTilingX("Noise Tiling X", Range( 0 , 1)) = 0.5
		_NoiseTilingY("Noise Tiling Y", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow exclude_path:deferred nofog 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _BaseTextureTint;
		uniform sampler2D _BaseTexture;
		uniform float _PannerSpeed;
		uniform float _BaseTextureTilingX;
		uniform float _BaseTextureTilingY;
		uniform float _BaseTextureStartOffset;
		uniform sampler2D _GradientMaskL;
		uniform sampler2D _GradientMaskR;
		uniform float _TextureVSNoiseSpeedRatio;
		uniform float _NoiseTilingX;
		uniform float _NoiseTilingY;
		uniform float _NoiseScale;
		uniform float _NoiseFalloff;


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


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 temp_cast_0 = (_PannerSpeed).xx;
			float2 appendResult16 = (float2(_BaseTextureTilingX , _BaseTextureTilingY));
			float2 appendResult13 = (float2(_BaseTextureStartOffset , 0.0));
			float2 uv_TexCoord8 = i.uv_texcoord * appendResult16 + appendResult13;
			float2 panner10 = ( 1.0 * _Time.y * temp_cast_0 + uv_TexCoord8);
			float2 appendResult57 = (float2(panner10.x , uv_TexCoord8.y));
			float4 tex2DNode1 = tex2D( _BaseTexture, appendResult57 );
			o.Emission = ( _BaseTextureTint * tex2DNode1 ).rgb;
			float2 uv_GradientMaskL2 = i.uv_texcoord;
			float2 uv_GradientMaskR3 = i.uv_texcoord;
			float2 temp_cast_2 = (( _PannerSpeed * _TextureVSNoiseSpeedRatio )).xx;
			float2 appendResult23 = (float2(_NoiseTilingX , _NoiseTilingY));
			float2 uv_TexCoord21 = i.uv_texcoord * appendResult23;
			float2 panner44 = ( 1.0 * _Time.y * temp_cast_2 + uv_TexCoord21);
			float simplePerlin2D19 = snoise( panner44*_NoiseScale );
			simplePerlin2D19 = simplePerlin2D19*0.5 + 0.5;
			float temp_output_41_0 = ( simplePerlin2D19 + simplePerlin2D19 );
			o.Alpha = ( tex2DNode1.a * ( ( tex2D( _GradientMaskL, uv_GradientMaskL2 ).r * tex2D( _GradientMaskR, uv_GradientMaskR3 ).r ) * saturate( pow( saturate( ( temp_output_41_0 * temp_output_41_0 ) ) , ( _NoiseFalloff * 10.0 ) ) ) ) );
		}

		ENDCG
	}
}
/*ASEBEGIN
Version=18912
620;393;1906;989;4478.489;343.447;2.434285;True;False
Node;AmplifyShaderEditor.RangedFloatNode;25;-3113.724,1151.606;Inherit;False;Property;_NoiseTilingY;Noise Tiling Y;12;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-3112.43,1042.792;Inherit;False;Property;_NoiseTilingX;Noise Tiling X;11;0;Create;True;0;0;0;False;0;False;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;23;-2790.411,1050.321;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-3107.392,573.4102;Inherit;False;Property;_PannerSpeed;Panner Speed;7;0;Create;True;0;0;0;False;0;False;0.02;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-3106.27,675.5057;Inherit;False;Property;_TextureVSNoiseSpeedRatio;Texture VS Noise Speed Ratio;8;0;Create;True;0;0;0;False;0;False;1.2;0.9;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-2534.17,1025.779;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-2442.063,898.1778;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;44;-2220.393,1023.568;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-2207.533,1462.478;Inherit;False;Property;_NoiseScale;Noise Scale;10;0;Create;True;0;0;0;False;0;False;2;3.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-3117.112,221.4981;Inherit;False;Property;_BaseTextureStartOffset;Base Texture Start Offset;2;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-3116.801,66.08688;Inherit;False;Property;_BaseTextureTilingY;Base Texture Tiling Y;4;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;19;-1970.314,1020.282;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-2814.349,330.8126;Inherit;False;Constant;_Float1;Float 1;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-3118.941,-23.92518;Inherit;False;Property;_BaseTextureTilingX;Base Texture Tiling X;3;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;16;-2810.576,48.23737;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;41;-1661.124,1024.622;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;13;-2810.097,222.7595;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-1453.012,1023.706;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-1544.364,1456.002;Inherit;False;Property;_NoiseFalloff;Noise Falloff;9;0;Create;True;0;0;0;False;0;False;3;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-1206.95,1258.171;Inherit;False;Constant;_Float0;Float 0;12;0;Create;True;0;0;0;False;0;False;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-2547.023,42.40313;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-1206.75,1142.471;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;28;-1210.817,1023.86;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;10;-2220.109,43.54508;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PowerNode;29;-1008.655,1023.86;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-1011.87,579.2657;Inherit;True;Property;_GradientMaskR;Gradient Mask R;6;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;3f569e13e42a44e4cb93da7ad23df2c5;3f569e13e42a44e4cb93da7ad23df2c5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;58;-1963.881,35.86735;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.BreakToComponentsNode;62;-1963.774,157.5216;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SamplerNode;2;-1013.17,345.2651;Inherit;True;Property;_GradientMaskL;Gradient Mask L;5;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;981a34570f7fce740ba2a6775b4551bd;981a34570f7fce740ba2a6775b4551bd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;57;-1627.123,32.96845;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-551.0675,344.672;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;31;-520.7668,1027.57;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;53;-941.2541,-225.186;Inherit;False;Property;_BaseTextureTint;Base Texture Tint;1;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.1033731,0.9528302,0.6986516,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-245.0248,342.228;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1020.735,8.053627;Inherit;True;Property;_BaseTexture;Base Texture;0;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;32d3bf100dda5b542ae288563f72d995;c85b399f958d70e43bcd3fe2e0c7326a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-559.7324,-2.656698;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;61.7928,343.8524;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;434.8884,-44.25785;Float;False;True;-1;2;;0;0;Unlit;TidalFlask/Unlit Transparent Panner NoFog;False;False;False;False;False;False;False;False;False;True;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;16;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;23;0;24;0
WireConnection;23;1;25;0
WireConnection;21;0;23;0
WireConnection;45;0;11;0
WireConnection;45;1;46;0
WireConnection;44;0;21;0
WireConnection;44;2;45;0
WireConnection;19;0;44;0
WireConnection;19;1;22;0
WireConnection;16;0;17;0
WireConnection;16;1;18;0
WireConnection;41;0;19;0
WireConnection;41;1;19;0
WireConnection;13;0;14;0
WireConnection;13;1;15;0
WireConnection;47;0;41;0
WireConnection;47;1;41;0
WireConnection;8;0;16;0
WireConnection;8;1;13;0
WireConnection;48;0;33;0
WireConnection;48;1;49;0
WireConnection;28;0;47;0
WireConnection;10;0;8;0
WireConnection;10;2;11;0
WireConnection;29;0;28;0
WireConnection;29;1;48;0
WireConnection;58;0;10;0
WireConnection;62;0;8;0
WireConnection;57;0;58;0
WireConnection;57;1;62;1
WireConnection;4;0;2;1
WireConnection;4;1;3;1
WireConnection;31;0;29;0
WireConnection;26;0;4;0
WireConnection;26;1;31;0
WireConnection;1;1;57;0
WireConnection;50;0;53;0
WireConnection;50;1;1;0
WireConnection;5;0;1;4
WireConnection;5;1;26;0
WireConnection;0;2;50;0
WireConnection;0;9;5;0
ASEEND*/
//CHKSM=5DF30D4DE5A5F6B88D1238FF7F846A8C52E8DDEB