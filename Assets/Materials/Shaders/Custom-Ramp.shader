Shader "Custom/Ramp" {

	Properties {
	    _Color ("Main Color", Color) = (1,1,1,1)
	    _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_SpecSize ("Specular Size", Range (1.0, 1.5)) = 1.0
		_Shininess ("Shininess", Range (0.01, 1)) = 0.09
		_FresnelBias("Fresnel Bias", float) = 0
		_Fresnel ("Fresnel Power", float) = 3
	    _MainTex ("Base (RGB)", 2D) = "white" {}
	    _BumpMap ("Bumpmap", 2D) = "bump" {}
	    _Ramp ("Toon Ramp (RGBA)", 2D) = "black" {}	    
	}
	
	SubShader {
	    Tags { "RenderType" = "Opaque" }
		
		CGPROGRAM
		#pragma surface surf Ramp
		#pragma target 3.0
				
		float4 _Color;
		half _SpecSize;
		half _Shininess;
		half _Fresnel;
		half _FresnelBias;
		half _Translucency;
		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _Ramp;
		
		inline half3 Overlay (half3 a, half3 b) {
		    half3 r = half3(0,0,0);
		    if (a.r > 0.5) { r.r = 1-(1-2*(a.r-0.5))*(1-b.r); }
		    else { r.r = (2*a.r)*b.r; }
		    if (a.g > 0.5) { r.g = 1-(1-2*(a.g-0.5))*(1-b.g); }
		    else { r.g = (2*a.g)*b.g; }
		    if (a.b > 0.5) { r.b = 1-(1-2*(a.b-0.5))*(1-b.b); }
		    else { r.b = (2*a.b)*b.b; }
		    //r = lerp(a,r,b.a);
		    //r = lerp(b,r,a.a);
		    return r;
		}
		
		struct Input {
		    float2 uv_MainTex;
		    float2 uv_BumpMap;
		};
		
		void surf (Input IN, inout SurfaceOutput o) {
		
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			//o.Albedo = .5;
			o.Albedo = tex.rgb * _Color.rgb;
			o.Gloss = tex.a;
			o.Alpha = tex.a * _Color.a;
			o.Specular = _Shininess;
			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));

		}
		
		half4 LightingRamp (SurfaceOutput s, half3 lightDir,  half3 viewDir, half atten) {
		    
		    half3 halfDir = normalize (lightDir + viewDir);
	 		
	 		half NdotL = dot (s.Normal, lightDir);
	 		half NdotV = dot (s.Normal, viewDir);
	 		float NdotH = dot (s.Normal, halfDir);
	 		
			fixed diff = (NdotL * 0.5 + 0.5);

	 		//fixed diff = pow((NdotL * 0.5 + 0.5), 2 - atten) * atten;
	 		
	 		//half3 brdf = tex2D (_Ramp, float2(diff, 1.0)).rgb;
	 		
	 		half3 fresnel = min (1, max (0, pow (1.0 - NdotV + _FresnelBias, _Fresnel))) * s.Albedo;// * max(0, NdotL);
	 		float spec = min (1, pow (saturate (NdotH) * _SpecSize, s.Specular * 128.0) * s.Gloss);
			//float spec = pow (NdotH * _SpecSize, s.Specular * 128.0);  // * s.Gloss;
	 		
			//half3 ramp = tex2D(_Ramp, float2(diff, 1.0)).rgb;
			half3 brdf = tex2D (_Ramp, float2(diff, NdotV)).rgb;
			
			half4 c;

			//c.rgb = s.Albedo;
			//c.rgb = _LightColor0.rgb * _Color.rgb * brdf  * (atten * 2);
            //c.rgb += _LightColor0.rgb * _SpecColor.rgb * spec * (atten);

            //c.rgb += fresnel;// * _Emission.rgb;

			//c.rgb = ((s.Albedo + spec * (_SpecColor.rgb + s.Albedo) + fresnel * 0.225) * brdf * _LightColor0.rgb + fresnel * 0.025) * atten;// _FresnelColor.rgb;

			c.rgb = ((s.Albedo + spec * Overlay(_SpecColor.rgb + s.Albedo,s.Albedo) + fresnel * 0.225) * brdf * _LightColor0.rgb + fresnel * 0.025) * atten * 2;// _FresnelColor.rgb;
            c.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec * atten;

			return c;
		}
		
		ENDCG
		
	}
	
	Fallback " Glossy", 0

}