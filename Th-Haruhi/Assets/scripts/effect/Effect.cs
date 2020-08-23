
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Effect : EntityBase
{
    public override EEntityType EntityType => EEntityType.Effect;

    public MeshRenderer Renderer { set; get; }
    public EffectDeploy Deploy;
    public virtual void Init(EffectDeploy deploy)
    {
        Deploy = deploy;
    }
    public virtual void ReInit()
    {

    }
}

public class EffectDeploy : Conditionable
{
    public int id;
    public int resourceId;
    public float scale;
    public float alpha;
    public int rota;
    public float[] AutoScale;
    public float[] AutoRotation;
    public float[] AutoGamma;
}