// ---------------------------【均值模糊】---------------------------
using UnityEngine;

[ExecuteInEditMode]
public class CameraBlur : MonoBehaviour
{
    private Shader _shader;
    private Material _material = null;

    public Material Material
    {
        get
        {
            if(_material == null)
                _material = new Material(_shader);
            return _material;
        }
    }

    //模糊半径  
    [Header("模糊半径")]
    [Range(0.2f, 10.0f)]
    public float BlurRadius = 1.0f;
    //降采样次数
    [Header("降采样次数")]
    [Range(1, 8)]
    public int downSample = 2;
    //迭代次数  
    [Header("迭代次数")]
    [Range(0, 4)]
    public int iteration = 1;

    void Start()
    {
        _shader = Shader.Find("Haruhi/CameraBlur");
    }

    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if (Material)
        {
            //申请RenderTexture，RT的分辨率按照downSample降低
            RenderTexture rt = RenderTexture.GetTemporary(sourceTexture.width >> downSample, sourceTexture.height >> downSample, 0, sourceTexture.format);

            //直接将原图拷贝到降分辨率的RT上
            Graphics.Blit(sourceTexture, rt);

            //进行迭代
            for (int i = 0; i < iteration; i++)
            {
                Material.SetFloat("_BlurRadius", BlurRadius);
                Graphics.Blit(rt, sourceTexture, Material);
                Graphics.Blit(sourceTexture, rt, Material);
            }
            //将结果输出  
            Graphics.Blit(rt, destTexture);

            //释放RenderBuffer
            RenderTexture.ReleaseTemporary(rt);
        }
    }
}
