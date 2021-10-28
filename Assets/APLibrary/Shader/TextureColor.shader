Shader "TearableCloth/DiceShader"
 {
 	Properties {
 		_MainTex ("Font Texture", 2D) = "white" {}
 		_Color ("Text Color", Color) = (1,1,1,1)
 	}
 	
 	SubShader {
 		Lighting Off 		
        LOD 200
        Cull Off
 		Zwrite off
 		Fog { Mode Off }
 		Tags {"Queue" = "Transparent" }
 		Pass {
 			Blend SrcAlpha OneMinusSrcAlpha
 			SetTexture [_MainTex] {
 				constantColor [_Color]
 				Combine texture * constant, texture * constant
 			}
 		}
 	}
 }