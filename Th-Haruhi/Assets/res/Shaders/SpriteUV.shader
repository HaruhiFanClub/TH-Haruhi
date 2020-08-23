//////////////////////////////////////////////
/// 2DxFX v3 - by VETASOFT 2018 //
//////////////////////////////////////////////


//////////////////////////////////////////////

Shader "Haruhi/SpriteUV"
{
    Properties
    {
        //��������
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        //UV�ƶ�����_X��
        AnimatedMouvementUV_X_1("AnimatedMouvementUV_X_1", Range(-1, 1)) = 0.268
        //UV�ƶ�����_Y��
        AnimatedMouvementUV_Y_1("AnimatedMouvementUV_Y_1", Range(-1, 1)) = 0.554
        //UV�ƶ�����_�ٶ�
        AnimatedMouvementUV_Speed_1("AnimatedMouvementUV_Speed_1", Range(-1, 1)) = 0.239
        //UV��ֵ
        _LerpUV_Fade_1("_LerpUV_Fade_1", Range(0, 1)) = 1
        //UV����
        _SpriteFade("SpriteFade", Range(0, 1)) = 1.0

        // required for UI.Mask
        [HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector]_Stencil("Stencil ID", Float) = 0
        [HideInInspector]_StencilOp("Stencil Operation", Float) = 0
        [HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
        [HideInInspector]_ColorMask("Color Mask", Float) = 15

    }

        SubShader
    {
        //��Ⱦ����=͸��ͨ��             ����ͶӰ                    ��Ⱦ����                    Ԥ������=ƽ��     ����ʹ�þ���ͼ��
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }

        ZWrite Off//��ȼ��ر�
        Blend SrcAlpha OneMinusSrcAlpha//���Ч����͸�����
        Cull Off//�����޳��ر�

        // required for UI.Mask
        Stencil
        {
        Ref[_Stencil]
        Comp[_StencilComp]
        Pass[_StencilOp]
        ReadMask[_StencilReadMask]
        WriteMask[_StencilWriteMask]
        }

        Pass
        {

            CGPROGRAM
            //����������ɫ������
            #pragma vertex vert
            //����ƬԪ��ɫ������
            #pragma fragment frag
            //ʹ�õ;���
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            //������ɫ������ṹ��
            struct appdata_t {
                float4 vertex   : POSITION;//����λ����Ϣ
                float4 color    : COLOR;//��ɫ��Ϣ
                float2 texcoord : TEXCOORD0;//�������꼯
            };
    //ƬԪ��ɫ������ṹ��
    struct v2f
    {
        float2 texcoord  : TEXCOORD0;//�������꼯
        float4 vertex   : SV_POSITION;//��Ļ����ϵ��λ����Ϣ
        float4 color    : COLOR;//��ɫ��Ϣ
    };

    //��Ӧ�����������������
    sampler2D _MainTex;
    float _SpriteFade;
    float AnimatedMouvementUV_X_1;
    float AnimatedMouvementUV_Y_1;
    float AnimatedMouvementUV_Speed_1;
    float _LerpUV_Fade_1;

    //������ɫ������
    v2f vert(appdata_t IN)
    {
        v2f OUT;
        OUT.vertex = UnityObjectToClipPos(IN.vertex);//������ת�����ü��ռ䲢���
        OUT.texcoord = IN.texcoord;//����������꼯
        OUT.color = IN.color;//�����ɫ��Ϣ
        return OUT;
    }

    //New!  UV�ƶ���������            UV          X���ϵ�ƫ�ƶ� Y��ƫ�ƶ�   �ƶ��ٶ�
    float2 AnimatedMouvementUV(float2 uv, float offsetx, float offsety, float speed)
    {
        speed *= _Time * 50;//�ٶȳ���ʱ�亯��
        uv += float2(offsetx, offsety) * speed;//�����ؽ���ƫ�ƣ�(������ԭ�е�λ����λ�Ƶ��µ�����λ��)

        //����������Ҫʵ�ʲٿ��޸Ĳ����������
        uv = fmod(uv,1);//������ȡ��(1/1=1����ȡ����������ͼ��UV�����1/0.25=4����UV��Ϊ4*4��)
        return uv;//�����һ����UV���ֳ���4*4�ݣ�������Խ�UV/0.25�������������������
    }

    //ƬԪ��ɫ������
    float4 frag(v2f i) : COLOR
    {
        //UV�ƶ���������
        float2 AnimatedMouvementUV_1 = AnimatedMouvementUV(i.texcoord,AnimatedMouvementUV_X_1,AnimatedMouvementUV_Y_1,AnimatedMouvementUV_Speed_1);

        //����Ҫ��ֵ�Ŀ���ֱ�����������������
        //i.texcoord=AnimatedMouvementUV_1;

        //New! ��ֵ����(��ֵ��Ч��������B�ӽ���A������Ҳ�����õڶ���UV�ӽ��ڵ�һ��UV)
        i.texcoord = lerp(i.texcoord,AnimatedMouvementUV_1,_LerpUV_Fade_1);

        //��������в���
        float4 _MainTex_1 = tex2D(_MainTex,i.texcoord);

        //���յ�������
        float4 FinalResult = _MainTex_1;
        FinalResult.rgb *= i.color.rgb;
        //��͸��ͨ�����н�������(���Խ�������)
        FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;

        return FinalResult;
    }

ENDCG
}
    }
        Fallback "Sprites/Default"
}
