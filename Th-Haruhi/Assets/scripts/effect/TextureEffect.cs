
using System;
using System.Collections.Generic;
using UnityEngine;

public class TextureEffect : EntityBase
{
    public override EEntityType EntityType => EEntityType.Effect;

    public TextureEffectDeploy Deploy { private set; get; }
    public List<Sprite> SpriteList { private set; get; }


    private int _lastChangedFrame;
    private int _currSpriteIndex;

    public virtual void Init(TextureEffectDeploy deploy, List<Sprite> spriteList)
    {
        Deploy = deploy;
        SpriteList = spriteList;
        ReInit();
    }

    public virtual void ReInit()
    {
        _lastChangedFrame = GameSystem.FixedFrameCount;
        _currSpriteIndex = 0;
        _bAutoDestroy = false;
        Renderer.material.mainTexture = SpriteList[0].texture;

    }

    private bool _bAutoDestroy;
    public void AutoDestroy()
    {
        _bAutoDestroy = true;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateAutoMove();
    }

    protected override void Update()
    {
        base.Update();
       
        if (SpriteList.Count <= 1) return;
        if (Deploy.frame <= 0) return;

        if (GameSystem.FixedFrameCount - _lastChangedFrame >  Deploy.frame)
        {
            _lastChangedFrame = GameSystem.FixedFrameCount;
            _currSpriteIndex++;
            if (_currSpriteIndex >= SpriteList.Count)
            {
                if (_bAutoDestroy)
                {
                    TextureEffectFactroy.DestroyEffect(this);
                    return;
                }
                _currSpriteIndex = 0;
            }
            Renderer.material.mainTexture = SpriteList[_currSpriteIndex].texture;
        }
    }

    private bool _bAutoMove;
    private Vector3 _autoMoveForward;
    public float _autoMoveSpeed;
    public void SetAutoMove(Vector3 forward, float speed)
    {
        _bAutoMove = true;
        _autoMoveForward = forward;
        _autoMoveSpeed = speed;
    }

    private void UpdateAutoMove()
    {
        if (_bAutoMove)
        {
            var dist = Time.deltaTime * _autoMoveSpeed;
            transform.position += _autoMoveForward * dist;
        }
    }

    public override void OnRecycle()
    {
        base.OnRecycle();
        _bAutoMove = false;
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
    public float brightNess;
    public float[] AutoScale;
    public float[] AutoRotation;
    public float[] AutoGamma;
}