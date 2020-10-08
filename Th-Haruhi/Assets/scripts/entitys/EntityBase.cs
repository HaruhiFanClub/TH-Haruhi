using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public enum EEntityType
{
    Player,
    Enemy,
    Effect,
    Bullet
}

public enum EShootSound
{
    Laser = 2006,
    Tan00 = 2003,
    Tan01 = 2004,
    Tan02 = 2005,
    Noraml = 2007,
}


public abstract class EntityBase : MonoBehaviour
{
    public abstract EEntityType EntityType { get; }

    private Rigidbody2D _rigidBody2d;

    public void InitRigid()
    {
        if (_rigidBody2d == null)
        {
            var r = GetComponent<Rigidbody2D>();
            if (r != null)
            {
                _rigidBody2d = r;
            }
            else
            {
                _rigidBody2d = gameObject.AddComponent<Rigidbody2D>();

                if (EntityType == EEntityType.Player)
                {
                    _rigidBody2d.bodyType = RigidbodyType2D.Dynamic;
                    _rigidBody2d.simulated = true;
                    _rigidBody2d.useAutoMass = false;
                    _rigidBody2d.mass = 10;
                    _rigidBody2d.drag = 30;
                    _rigidBody2d.gravityScale = 0f;
                    _rigidBody2d.angularDrag = 0f;
                }
                else
                {
                    _rigidBody2d.bodyType = RigidbodyType2D.Kinematic;
                }
                _rigidBody2d.freezeRotation = true;
            }
        }
    }

    public void PlayShootEffect(EColor color, float startScale = 1f, Vector3? startPos = null)
    {
        var effectId = 0;
        switch (color)
        {
            case EColor.Red:
                effectId = 901;
                break;
            case EColor.Purple:
                effectId = 902;
                break;
            case EColor.Blue:
                effectId = 903;
                break;
            case EColor.BlueLight:
                effectId = 904;
                break;
            case EColor.Green:
                effectId = 905;
                break;
            case EColor.Yellow:
                effectId = 906;
                break;
            case EColor.Orange:
                effectId = 907;
                break;
            case EColor.White:
                effectId = 908;
                break;
        }

        if (effectId > 0)
        {
            var pos = startPos == null ? transform.position : (Vector3)startPos;
            TextureEffectFactroy.CreateEffect(effectId, SortingOrder.ShootEffect, effect =>
            {
                effect.transform.position = pos;
                effect.transform.localScale = Vector3.one * startScale;
                effect.transform.DOScale(0f, 0.4f).onComplete = () =>
                {
                    TextureEffectFactroy.DestroyEffect(effect);
                };
            });
        }
    }



    private Dictionary<EShootSound, int> ShootSoundCd = new Dictionary<EShootSound, int>();

    private int GetShootSoundCdFrame(EShootSound e)
    {
        switch (e)
        {
            case EShootSound.Laser:
                return 20;
            case EShootSound.Tan00:
            case EShootSound.Tan01:
            case EShootSound.Tan02:
                return 5;
            case EShootSound.Noraml:
                return 3;
        }
        return 0;
    }

    public void PlayShootSound(EShootSound sound)
    {
        if(ShootSoundCd.TryGetValue(sound, out int lastframe))
        {
            var cdFrame = GetShootSoundCdFrame(sound);
            if(GameSystem.FixedFrameCount - lastframe < cdFrame)
            {
                return;
            }
        }

        ShootSoundCd[sound] = GameSystem.FixedFrameCount;
        Sound.PlayUiAudioOneShot((int)sound, true);
    }

    public Rigidbody2D Rigid2D
    { 
        get 
        {
            InitRigid();
            return _rigidBody2d;
        }
    }

    public bool InCache { private set; get; }
    public void SetInCache(bool b)
    {
        InCache = b;
    }
    protected virtual void Awake() {

        
    }
    protected virtual void OnDestroy() { }
    protected virtual void Update() { }
    protected virtual void FixedUpdate() { }
    public virtual void OnRecycle() { }

    public static void DestroyEntity(EntityBase b)
    {
        if(b != null && b.gameObject != null)
        {
            Destroy(b.gameObject);
        }
    }
}