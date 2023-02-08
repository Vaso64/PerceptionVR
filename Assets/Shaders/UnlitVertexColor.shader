Shader "PerceptionVR/UnlitVertexColorTexture"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
		_ClipPlane ("Clip plane", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Lighting Off
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            
            struct appdata
            {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 color : COLOR;
            };

            sampler2D _MainTex;

            v2f vert (appdata v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord0.xy;
                o.color = v.color;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }

			float4 _ClipPlane;

            half4 frag (v2f i) : COLOR
            {
                const float distanceFromPlane = dot(i.worldPos, _ClipPlane.xyz) + _ClipPlane.w;
				clip(-distanceFromPlane);
                return tex2D(_MainTex, i.uv) * i.color;
            }

            ENDCG
        }
    }
}
