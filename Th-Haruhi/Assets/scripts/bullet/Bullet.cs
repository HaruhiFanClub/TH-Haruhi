using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : EntityBase
{
    public class ColliderInfo
    {
        public bool IsBox;
        public float Radius;
        public float BoxWidth;
        public float BoxHeight;
        public Vector3 Center;
    }

    //擦弹判定距离
    public const float GrizeDistance = 0.25f;

    //碰撞信息
    public ColliderInfo CollisionInfo;

    public List<Sprite> AniList { set; get; }
    public override EEntityType EntityType => EEntityType.Bullet;
    public BulletDeploy Deploy { private set; get; }
    protected Transform Master { private set; get; }
    protected bool Shooted { private set; get; }

    public int Atk { protected set; get; }

    private int _totalFrame;

    private bool _isAni;

    public bool BoundDestroy = true;

    public Action<Bullet> OnDestroyCallBack;

    private int DefaultSortOrder;

    public static int TotalBulletCount;


    private void OnEnable()
    {
        TotalBulletCount++;
    }

    private void OnDisable()
    {
        TotalBulletCount--;
    }

    public virtual void Init(BulletDeploy deploy)
    {
        Deploy = deploy;
        _isAni = deploy.isAni;
        DefaultSortOrder = Renderer.sortingOrder;
        ReInit();
    }

    public void SetAtk(int atk)
    {
        Atk = atk;
    }

    public void SetMaster(Transform master)
    {
        Master = master;
    }

    public virtual void ReInit()
    {
    }
 
    public virtual void Shoot(Vector3 realPos)
    {
        _totalFrame = 0;
        CacheTransform.position = realPos;
        Shooted = true;
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (InCache) return;
        if (!Shooted) return;
        _totalFrame++;
        UpdateAnimation();
    }

    protected bool CheckBulletOutSide(Vector3 bulletCenter)
    {
        //超出边界销毁
        if(BoundDestroy)
        {
            if (bulletCenter.x < -15f || bulletCenter.x > 5f || bulletCenter.y < -12f || bulletCenter.y > 12f)
            {
                BulletFactory.DestroyBullet(this);
                return true;
            }
        }
        return false;
    }

    private int _currAniIdx;
    private float _lastAniFrame;

    private void UpdateAnimation()
    {
        if (_isAni && AniList != null)
        {
            if (_totalFrame - _lastAniFrame > 6)
            {
                _currAniIdx++;
                if (_currAniIdx >= AniList.Count)
                {
                    _currAniIdx = 0;
                }
                Renderer.material.mainTexture = AniList[_currAniIdx].texture;
                _lastAniFrame = _totalFrame;
            }
        }
    }


    public void SetBoundDestroy(bool b)
    {
        BoundDestroy = b;
    }

    public override void OnRecycle()
    {
        base.OnRecycle();

        SetBoundDestroy(true);

        OnDestroyCallBack?.Invoke(this);
        OnDestroyCallBack = null;

        _currAniIdx = 0;
        _totalFrame = 0;
        Shooted = false;
        Renderer.sortingOrder = DefaultSortOrder;
    }
}


public class BulletDeploy : Conditionable
{
    public int id;
    public string classType;
    public int resourceId;
    public float rota = 90f;
    public float scale;
    public float alpha;
    public float radius;
    public int spriteIdx;
    public int bombEffectId;
    public float bombEffectSpeed;
    public int centerPivot;
    public bool isAni;
    public bool isBoxCollider;
    public float sizeX;
    public float sizeY;
    public float brightness;
    public bool delayDestroy;
    public EColor EColor;
}