
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ProjectorShadow : MonoBehaviour
{
    public float mProjectorSize = 23;

    public int mRenderTexSize = 2048;

    public LayerMask mLayerCaster;

    public LayerMask mLayerIgnoreReceiver;

    private bool mUseCommandBuf = false; 

    private Projector mProjector;

    private Camera mShadowCam;

    private RenderTexture mShadowRT;

    private CommandBuffer mCommandBuf;

    private Material mReplaceMat;


    void Start ()
    {
        // 创建render texture
        mShadowRT = new RenderTexture(mRenderTexSize, mRenderTexSize, 0, RenderTextureFormat.R8);
        mShadowRT.name = "ShadowRT";
        mShadowRT.antiAliasing = 1;
        mShadowRT.filterMode = FilterMode.Bilinear;
        mShadowRT.wrapMode = TextureWrapMode.Clamp;

        //projector初始化
        mProjector = GetComponent<Projector>();
        mProjector.orthographic = true;
        mProjector.orthographicSize = mProjectorSize;
        mProjector.ignoreLayers = mLayerIgnoreReceiver;
        mProjector.material.SetTexture("_ShadowTex", mShadowRT);

        //camera初始化
        mShadowCam = gameObject.AddComponent<Camera>();
        mShadowCam.clearFlags = CameraClearFlags.Color;
        mShadowCam.backgroundColor = Color.black;
        mShadowCam.orthographic = true;
        mShadowCam.orthographicSize = mProjectorSize;
        mShadowCam.depth = -100.0f;
        mShadowCam.nearClipPlane = mProjector.nearClipPlane;
        mShadowCam.farClipPlane = mProjector.farClipPlane;
        mShadowCam.targetTexture = mShadowRT;
        mShadowCam.depthTextureMode = DepthTextureMode.None;
        mShadowCam.renderingPath = RenderingPath.Forward;

        SwitchCommandBuffer();
    }

    private void LateUpdate()
    {
        //transform.position = chrPos - transform.forward * 50f;
    }

    private Transform _lightTrans;
    public void Init(Transform lightTrans)
    {
        transform.forward = lightTrans.forward;
    }


    private void SwitchCommandBuffer()
    {
        Shader replaceshader = GameSystem.DefaultRes.ShadowCaster;

        if (!mUseCommandBuf)
        {
            mShadowCam.cullingMask = mLayerCaster;
            mShadowCam.SetReplacementShader(replaceshader, "RenderType");
        }
        else
        {
            mShadowCam.cullingMask = 0;
            mShadowCam.RemoveAllCommandBuffers();
            if (mCommandBuf != null)
            {
                mCommandBuf.Dispose();
                mCommandBuf = null;
            }
            
            mCommandBuf = new CommandBuffer();
            mShadowCam.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, mCommandBuf);

            if (mReplaceMat == null)
            {
                mReplaceMat = new Material(replaceshader);
                mReplaceMat.hideFlags = HideFlags.HideAndDontSave;
            }
        }
    }

    private void FillCommandBuffer()
    {
        mCommandBuf.Clear();

        Plane[] camfrustum = GeometryUtility.CalculateFrustumPlanes(mShadowCam);

        /*
        foreach (var render in ShadowScript.ShadowRenderers)
        {
            if (render == null)
                continue;

            mCommandBuf.DrawRenderer(render, mReplaceMat);
        }*/
    }
}
