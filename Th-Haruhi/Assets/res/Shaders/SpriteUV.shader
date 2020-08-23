//////////////////////////////////////////////
/// 2DxFX v3 - by VETASOFT 2018 //
//////////////////////////////////////////////


//////////////////////////////////////////////

Shader "Haruhi/SpriteUV"
{
    Properties
    {
        //精灵纹理
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        //UV移动动画_X轴
        AnimatedMouvementUV_X_1("AnimatedMouvementUV_X_1", Range(-1, 1)) = 0.268
        //UV移动动画_Y轴
        AnimatedMouvementUV_Y_1("AnimatedMouvementUV_Y_1", Range(-1, 1)) = 0.554
        //UV移动动画_速度
        AnimatedMouvementUV_Speed_1("AnimatedMouvementUV_Speed_1", Range(-1, 1)) = 0.239
        //UV插值
        _LerpUV_Fade_1("_LerpUV_Fade_1", Range(0, 1)) = 1
        //UV渐淡
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
        //渲染队列=透明通道             忽略投影                    渲染类型                    预览类型=平面     可以使用精灵图吗
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }

        ZWrite Off//深度检测关闭
        Blend SrcAlpha OneMinusSrcAlpha//混合效果：透明混合
        Cull Off//背面剔除关闭

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
            //声明顶点着色器代码
            #pragma vertex vert
            //声明片元着色器代码
            #pragma fragment frag
            //使用低精度
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            //顶点着色器输入结构体
            struct appdata_t {
                float4 vertex   : POSITION;//顶点位置信息
                float4 color    : COLOR;//颜色信息
                float2 texcoord : TEXCOORD0;//纹理坐标集
            };
    //片元着色器输入结构体
    struct v2f
    {
        float2 texcoord  : TEXCOORD0;//纹理坐标集
        float4 vertex   : SV_POSITION;//屏幕坐标系中位置信息
        float4 color    : COLOR;//颜色信息
    };

    //对应最上面材质属性声明
    sampler2D _MainTex;
    float _SpriteFade;
    float AnimatedMouvementUV_X_1;
    float AnimatedMouvementUV_Y_1;
    float AnimatedMouvementUV_Speed_1;
    float _LerpUV_Fade_1;

    //顶点着色器代码
    v2f vert(appdata_t IN)
    {
        v2f OUT;
        OUT.vertex = UnityObjectToClipPos(IN.vertex);//将坐标转换到裁剪空间并输出
        OUT.texcoord = IN.texcoord;//输出纹理坐标集
        OUT.color = IN.color;//输出颜色信息
        return OUT;
    }

    //New!  UV移动动画处理            UV          X轴上的偏移度 Y轴偏移度   移动速度
    float2 AnimatedMouvementUV(float2 uv, float offsetx, float offsety, float speed)
    {
        speed *= _Time * 50;//速度乘以时间函数
        uv += float2(offsetx, offsety) * speed;//对像素进行偏移，(像素在原有的位置上位移到新的像素位置)

        //下面两步需要实际操控修改才能理解作用
        uv = fmod(uv,1);//浮点数取余(1/1=1，获取到的是整张图的UV，如果1/0.25=4，则将UV分为4*4份)
        return uv;//如果上一步将UV划分乘了4*4份，这里可以将UV/0.25，都后续纹理进行缩放
    }

    //片元着色器代码
    float4 frag(v2f i) : COLOR
    {
        //UV移动动画处理
        float2 AnimatedMouvementUV_1 = AnimatedMouvementUV(i.texcoord,AnimatedMouvementUV_X_1,AnimatedMouvementUV_Y_1,AnimatedMouvementUV_Speed_1);

        //不需要插值的可以直接跳过进行纹理采样
        //i.texcoord=AnimatedMouvementUV_1;

        //New! 插值计算(插值的效果就是让B接近于A，这里也就是让第二张UV接近于第一张UV)
        i.texcoord = lerp(i.texcoord,AnimatedMouvementUV_1,_LerpUV_Fade_1);

        //对纹理进行采样
        float4 _MainTex_1 = tex2D(_MainTex,i.texcoord);

        //最终的输出结果
        float4 FinalResult = _MainTex_1;
        FinalResult.rgb *= i.color.rgb;
        //对透明通道进行渐淡处理(乘以渐淡参数)
        FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;

        return FinalResult;
    }

ENDCG
}
    }
        Fallback "Sprites/Default"
}
