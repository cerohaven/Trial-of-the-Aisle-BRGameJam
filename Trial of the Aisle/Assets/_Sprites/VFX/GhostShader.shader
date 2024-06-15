Shader "Custom/GhostShader"
{
    Properties
    {
        // Define the main texture property with a default white texture
        _MainTex ("Main Texture", 2D) = "white" {}

        // Define the tint color property with a default semi-transparent white color
        _Color ("Tint Color", Color) = (1,1,1,0.5)

        // Define a float property to determine if a single color should be used
        _UseSingleColor ("Use Single Color", Float) = 0

        // Define a vector property to store the player's position
        _PlayerPos ("Player Position", Vector) = (0,0,0)

        // Define a float property for the maximum distance used in the fade calculation
        _MaxDistance ("Max Distance", Float) = 10.0
    }
    SubShader
    {
        // Set the render queue and type for transparent rendering
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        // Enable alpha blending, disable depth writing, and backface culling
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            // Start of the CG program block
            CGPROGRAM
            #pragma vertex vert // Define the vertex shader function
            #pragma fragment frag // Define the fragment shader function
            #include "UnityCG.cginc" // Include common Unity shader functions

            // Structure to define the input data for the vertex shader
            struct appdata_t
            {
                float4 vertex : POSITION; // Vertex position
                float2 uv : TEXCOORD0; // Texture coordinates
            };

            // Structure to define the output data from the vertex shader to the fragment shader
            struct v2f
            {
                float2 uv : TEXCOORD0; // Texture coordinates
                float4 vertex : SV_POSITION; // Clip space position
                float3 worldPos : TEXCOORD1; // World space position
            };

            // Declare shader properties
            sampler2D _MainTex; // Main texture sampler
            float4 _MainTex_ST; // Main texture scaling and offset
            float4 _Color; // Tint color
            float _UseSingleColor; // Use single color flag
            float3 _PlayerPos; // Player position
            float _MaxDistance; // Maximum distance for fade calculation

            // Vertex shader function
            v2f vert (appdata_t v)
            {
                v2f o;
                // Transform the vertex position from object space to clip space
                o.vertex = UnityObjectToClipPos(v.vertex);

                // Transform the texture coordinates
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // Calculate the world position of the vertex
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            // Fragment shader function
            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture color at the given UV coordinates
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // Calculate the distance from the vertex to the player position
                float dist = distance(i.worldPos, _PlayerPos);

                // Calculate the fade factor based on the distance
                float fadeFactor = saturate(1.0 - pow(dist / _MaxDistance, 2));

                fixed4 finalColor;

                // Check if single color mode is enabled
                if (_UseSingleColor == 1)
                {
                    // Use the single color with the fade factor applied to the alpha
                    finalColor = fixed4(_Color.rgb, texColor.a * fadeFactor);
                }
                else
                {
                    // Tint the texture color with the specified color and apply the fade factor to the alpha
                    finalColor = texColor * _Color;
                    finalColor.a *= fadeFactor;
                }

                // Return the final color to be rendered
                return finalColor;
            }
            ENDCG // End of the CG program block
        }
    }
}
