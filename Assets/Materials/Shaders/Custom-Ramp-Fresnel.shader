Shader "Custom/Ramp + Fresnel" {

	Properties {
	    _Color ("Main Color", Color) = (1,1,1,1)
	    //_DiffuseScale("Diffuse Scale", Float) = 1
        //_DiffuseBias("Diffuse Bias", Float) = 0
        //_DiffuseExponent("Diffuse Exponent", float) = 1
		_Fresnel ("Fresnel Power", float) = 2.5
		_FresnelBias("Fresnel Bias", float) = 2.5
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.09
	    _FresnelColor ("Rim Color", Color) = (.25,.25,.25,1)
	    _FresnelDodge ("Fresnel Dodge", float) = .25
	    _MainTex ("Base (RGB)", 2D) = "white" {}
	    _BumpMap ("Bumpmap", 2D) = "bump" {}
	    _Ramp ("Toon Ramp (RGBA)", 2D) = "black" {}	    
	}
	
	SubShader {
	    Tags { "RenderType" = "Opaque" }
		
		CGPROGRAM
		// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
		#pragma exclude_renderers gles
		//#pragma surface surf BlinnPhong
		#pragma surface surf Ramp
		#pragma target 3.0
				
		float4 _Color;
		float4 _FresnelColor;
		float3 _RimDirection;
		float _FresnelDodge;
		float _FresnelBias;
		half _Shininess;
		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _Ramp;
		float _Fresnel;
		
		half4 LightingRamp (SurfaceOutput s, half3 lightDir,  half3 viewDir, half atten) {
		    
		    half3 h = normalize (lightDir + viewDir);
	 		
	 		half NdotL = dot (s.Normal, lightDir);
	 		half NdotV = dot (s.Normal, viewDir);
	 		
			float diff = NdotL * 0.5 + 0.5;
			
			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, s.Specular*128.0) * s.Gloss;
			
			float2 uv = float2(diff - 0.1, NdotV);
			float3 brdf = tex2D (_Ramp, uv.xy).rgb;
            
            float4 c;
            
            //c.rgb = (s.Albedo * _LightColor0.rgb + _SpecColor.rgb * spec) * brdf * (atten * 2);// + NdotL * NdotV;
            c.rgb = (s.Albedo * _LightColor0.rgb + _SpecColor.rgb * spec) * brdf * (atten * 2);// + NdotL * NdotV;
			//c.rgb = brdf;
			c.a = s.Alpha; // + _LightColor0.a * _SpecColor.a * spec * atten;
			return c;
		}
		
		struct Input {
		    float2 uv_MainTex;
		    float2 uv_BumpMap;
		    float3 viewDir;
		    float3 lightDir;
		    float3 worldNormal;
		    INTERNAL_DATA
		};
		
		void surf (Input IN, inout SurfaceOutput o) {
		
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.rgb * _Color.rgb;
			o.Gloss = tex.a;
			o.Alpha = tex.a * _Color.a;
			o.Specular = _Shininess;
		
			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
		 
		 	half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal)) + _FresnelBias;
			half sunlight = max(0,dot(float3(0, 1, 0), WorldNormalVector(IN, o.Normal) * (1 + _FresnelBias)));

			//half light = saturate(dot(normalize(IN.lightDir), o.Normal));

			//half sunlight = dot(normalize(IN.lightDir)*-1, WorldNormalVector(IN, o.Normal));
			
		    //o.Emission = sunlight * (o.Albedo * _FresnelDodge + _FresnelColor);
		    o.Emission = min(1,pow(rim,_Fresnel))  * sunlight * (o.Albedo * _FresnelDodge + _FresnelColor);

			//o.Emission = min(1,pow(rim,_Fresnel * sunlight)) * (o.Albedo * _FresnelDodge + _FresnelColor);
		    //o.Emission = pow (min(1,max(rim,sunlight)), _Fresnel) * (o.Albedo * _FresnelDodge + _FresnelColor);
		    //o.Alpha = tex2D ( _MainTex, IN.uv_MainTex).a;
		    //o.Specular = _Shininess;

		}
		
		ENDCG
		
	}
	
	Fallback " Glossy", 0

}