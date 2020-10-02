
using System;
using System.Collections.Generic;
using UnityEngine;

public class TextureEffect : EntityBase
{
    public override EEntityType EntityType => EEntityType.Effect;

    public MeshRenderer Renderer { set; get; }
    public TextureEffectDeploy Deploy { private set; get; }
    public List<Sprite> SpriteList { private set; get; }


    private float _lastChangeSpriteTime;
    private int _currSpriteIndex;

    public virtual void Init(TextureEffectDeploy deploy, List<Sprite> spriteList)
    {
        Deploy = deploy;
        SpriteList = spriteList;
        ReInit();
    }

    public virtual void ReInit()
    {
        _lastChangeSpriteTime = Time.time;
        _currSpriteIndex = 0;
        _bAutoDestroy = false;
    }

    private bool _bAutoDestroy;
    public void AutoDestroy()
    {
        _bAutoDestroy = true;
    }


    protected override void Update()
    {
        base.Update();
        if (SpriteList.Count <= 1) return;
        if (Deploy.frame <= 0) return;

        if(Time.time - _lastChangeSpriteTime > GameSystem.FrameTime * Deploy.frame)
        {
            _lastChangeSpriteTime = Time.time;
            _currSpriteIndex++;
            if (_currSpriteIndex >= SpriteList.Count)
            {
                if(_bAutoDestroy)
                {
                    TextureEffectFactroy.DestroyEffect(this);
                    return;
                }
                _currSpriteIndex = 0;
            }
            Renderer.material.mainTexture = SpriteList[_currSpriteIndex].texture;
        }
    }

}

public class TextureEffectDeploy : Conditionable
{
    public int id;
    public int resourceId;
    public float scale;
    public float alpha;
    public int frame;
    public int rota;
    public float[] Color;
    public float[] AutoScale;
    public float[] AutoRotation;
    public float[] AutoGamma;
}