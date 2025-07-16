// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TidalFlask/Two Texture Alpha Blend"
{
	Properties
	{
		_BaseTexture("Base Texture", 2D) = "white" {}
		_BaseTextureLuminance("Base Texture Luminance", Range( 0.5 , 2)) = 1
		_BlendTexture("Blend Texture", 2D) = "white" {}
		_BlendTextureOpacity("Blend Texture Opacity", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _BaseTexture;
		uniform float4 _BaseTexture_ST;
		uniform float _BaseTextureLuminance;
		uniform sampler2D _BlendTexture;
		uniform float4 _BlendTexture_ST;
		uniform float _BlendTextureOpacity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BaseTexture = i.uv_texcoord * _BaseTexture_ST.xy + _BaseTexture_ST.zw;
			float4 temp_cast_0 = (_BaseTextureLuminance).xxxx;
			float2 uv_BlendTexture = i.uv_texcoord * _BlendTexture_ST.xy + _BlendTexture_ST.zw;
			float4 tex2DNode1 = tex2D( _BlendTexture, uv_BlendTexture );
			float4 lerpResult3 = lerp( pow( tex2D( _BaseTexture, uv_BaseTexture ) , temp_cast_0 ) , tex2DNode1 , ( tex2DNode1.a * _BlendTextureOpacity ));
			o.Albedo = lerpResult3.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18912
518;566;1906;786;2209.521;-27.36628;1.167778;True;False
Node;AmplifyShaderEditor.SamplerNode;1;-1163.522,212.5363;Inherit;True;Property;_BlendTexture;Blend Texture;2;0;Create;True;0;0;0;False;0;False;-1;46dd717523e407e4a979ddfbfb696739;46dd717523e407e4a979ddfbfb696739;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-1167.572,-0.7767795;Inherit;True;Property;_BaseTexture;Base Texture;0;0;Create;True;0;0;0;False;0;False;-1;7ba61d2305b467948b2205945a0be3b3;7ba61d2305b467948b2205945a0be3b3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;7;-1154.344,-178.7353;Inherit;False;Property;_BaseTextureLuminance;Base Texture Luminance;1;0;Create;True;0;0;0;False;0;False;1;1;0.5;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-1149.413,536.6818;Inherit;False;Property;_BlendTextureOpacity;Blend Texture Opacity;3;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-808.3611,521.8682;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;6;-689.8671,2.508074;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;3;-376.4882,0.9890743;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;;0;0;Standard;TidalFlask/Two Texture Alpha Blend;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;16;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;1;4
WireConnection;4;1;5;0
WireConnection;6;0;2;0
WireConnection;6;1;7;0
WireConnection;3;0;6;0
WireConnection;3;1;1;0
WireConnection;3;2;4;0
WireConnection;0;0;3;0
ASEEND*/
//CHKSM=1C90C9E57F31AFA8A31049709D934D26958AAC6D