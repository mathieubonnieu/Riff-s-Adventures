Shader "Unlit/SolidColor_WithWorldPos"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CameraDepthTexture ("Camera Depth Texture", 2D) = "blue" {}
        _Zone ("Zone Points", Vector) = (4.500000,4.500000,-4.500000,-4.500000)
        _Fade ("Fade Distance", Float) = 2.000000
        _Color ("Color", Color) = (0.000000,0.000000,0.000000,1.000000)
        _CenterOpacity ("Center Opacity", Float) = 1.000000
        _NoiseStrength ("Noise Strength", Float) = 0.5
        _NoiseSpeed ("Noise Speed", Float) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
        
        Pass
        {
            Name "BlitPass"
            ZTest Always Cull Off ZWrite Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile __ ORTHOGRAPHIC_CAMERA

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // The DeclareDepthTexture.hlsl file contains utilities for sampling the Camera
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
            // Textures et leurs samplers
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float4 _Zone;
            float _Fade;
            float4 _Color;
            float _CenterOpacity;
            float _NoiseStrength;
            float _NoiseSpeed;
            
            // Pour la reconstruction, URP fournit dans le constant buffer UnityPerCamera
            CBUFFER_START(UnityPerCamera)
                float4x4 _InverseCameraProjection; // Inverse de la matrice de projection de la caméra
                float4x4 _CameraToWorld;           // Matrice de passage de la caméra au monde
            CBUFFER_END
            
            struct appdata 
            { 
                uint vertexID : SV_VertexID;
            };
            
            struct v2f 
            { 
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0; 
            };

            v2f vert(appdata input)
            {
                v2f o;
                // Création d'un full-screen triangle
                float2 positions[3] = {
                    float2(-1, -1),
                    float2( 3, -1),
                    float2(-1,  3)
                };
                
                float2 uvs[3] = {
                    float2(0, 0),
                    float2(2, 0),
                    float2(0, 2)
                };
                
                o.vertex = float4(positions[input.vertexID], 0, 1);
                o.uv = uvs[input.vertexID] * _MainTex_ST.xy + _MainTex_ST.zw;
                // Inverser l'axe Y si nécessaire (dépend de l'espace UV utilisé)
                o.uv.y = 1 - o.uv.y;
                
                return o;
            }

            // --- Simplex/Perlin-like noise function (value noise) ---
            float hash(float2 p) {
                p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
                return frac(sin(p) * 43758.5453);
            }
            float noise(float2 p) {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float2 UV = i.vertex.xy / _ScaledScreenParams.xy;

                #if UNITY_REVERSED_Z
                    real depth = SampleSceneDepth(UV);
                #else
                    // Adjust z to match NDC for OpenGL
                    real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(UV));
                #endif
                
                float3 worldPos = ComputeWorldSpacePosition(i.uv, depth, UNITY_MATRIX_I_VP);


                float maxX = _Zone.x;
                float maxY = _Zone.y;
                float minX = _Zone.z;
                float minY = _Zone.w;

                maxX = max(worldPos.x - maxX, 0);
                minX = max(minX - worldPos.x, 0);
                maxY = max(worldPos.z - maxY, 0);
                minY = max(minY - worldPos.z, 0);

                float2 xy = float2(maxX + minX, maxY + minY);
                float dist = length(xy);

                // Ajout du bruit basé sur la position monde et le temps
                float2 noiseInput = worldPos.xz * 3 + _Time.y * _NoiseSpeed;
                float2 noiseInput2 = worldPos.xz * 3 - _Time.y * _NoiseSpeed;
                float n = noise(noiseInput) * noise(noiseInput2);
                dist += (n - 0.5) * 2.0 * _NoiseStrength; // bruit centré sur 0, amplitude contrôlée

                dist = smoothstep(_Fade, 0, dist);
                dist = smoothstep(0, _Fade, dist);
                
                // Ajoute un noise pour ajouter de la variation à la couleur
                float noiseScale = 5;
                float noiseValue = noise(UV * noiseScale + _Time.y * _NoiseSpeed) * noise(UV * noiseScale - _Time.y * _NoiseSpeed); // Valeur de bruit entre 0 et 1
                noiseValue = noiseValue * 0.5 + 0.5;
                _Color.rgb *= noiseValue; // Applique le bruit à la couleur

                texColor = lerp(_Color, texColor, _CenterOpacity);
            
                return lerp(texColor, _Color, 1 - dist);
            }
            ENDHLSL
        }
    }
    FallBack "Unlit/Color"
}
