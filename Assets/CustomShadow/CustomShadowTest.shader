﻿Shader "Hidden/CustomShadowTest"
{
    Properties
    {
        _MainTex("", 2D) = ""{}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            Blend Zero SrcAlpha
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma target 3.5
            #include "CustomShadowTest.cginc"
            ENDCG
        }
    }
}
