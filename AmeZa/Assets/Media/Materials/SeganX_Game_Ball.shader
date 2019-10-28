Shader "SeganX/Game/Ball" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_VinylTex ("Vinyl (RGB)", 2D) = "white" {}
		
		[Enum(ON,1,OFF,0)]	_ZWrite ("Z Write", Int) = 1
		[Enum(BACK,2,FRONT,1,OFF,0)]	_Cull ("Cull", Int) = 2
		[Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendSrc ("SrcFactor", Int) = 5
		[Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendDest ("DstFactor", Int) = 10
		
	}

	SubShader 
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100
		
		Cull [_Cull]
		ZWrite [_ZWrite]
		Blend [_BlendSrc] [_BlendDest] 
			
		Pass 
		{ 
			Name "FORWARD" 
			Tags { "LightMode" = "ForwardBase" }
		
			CGPROGRAM
				
				#pragma vertex vert
				#pragma fragment frag
			
				#include "UnityCG.cginc"

				struct VertexInput
				{
					float4 pos : POSITION;
					float2 uv0 : TEXCOORD0;
                    float4 col : COLOR;
				};
				
				struct VertexOutput 
				{
					float4 pos : SV_POSITION;
					float2 uv0 : TEXCOORD0;
                    float4 col : COLOR;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _VinylTex;
				
				VertexOutput vert (VertexInput v)
				{
					VertexOutput o;
					o.pos = UnityObjectToClipPos( v.pos );
					o.uv0 = TRANSFORM_TEX( v.uv0, _MainTex );
                    o.col = v.col;
					return o;
				}
				
				fixed4 frag (VertexOutput i) : SV_Target
				{
					fixed4 c = tex2D( _MainTex, i.uv0 );
                    fixed4 m = tex2D( _VinylTex, i.uv0 ) * i.col;
                    float gray = clamp(c.r + c.g + c.b, 0, 3) / 3;
                    gray = pow(gray, 0.1f);
                    //return float4(gray, gray, gray, 1);
                    return fixed4(c.rgb * (1 - m.a) + m.rgb * m.a * gray, c.a);
				}
				
			ENDCG
		}
	}

}
