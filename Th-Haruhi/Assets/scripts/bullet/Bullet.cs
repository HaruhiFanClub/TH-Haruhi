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
    public const float GrizeDistance = 6;

    //碰撞信息
    public ColliderInfo CollisionInfo;

    public List<Sprite> AniList { set; get; }
    public override EEntityType EntityType => EEntityType.Bullet;
    public BulletDeploy Deploy { private set; get; }
    protected Transform Master { private set; get; }
    
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
    }

    public void SetAtk(int atk)
    {
        Atk = atk;
    }

    public void SetMaster(Transform master)
    {
        Master = master;
    }

    public virtual void OnCreate(Vector3 pos)
    {
        _totalFrame = 0;
        CacheTransform.position = pos;
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if (InCache) return;
        _totalFrame++;
        UpdateAnimation();
    }

    protected bool CheckBulletOutSide(Vector3 bulletCenter)
    {
        //超出边界销毁
        if(BoundDestroy)
        {
            var factor = 1.5f;
            if (bulletCenter.x < Vector2Fight.Left * factor || 
                bulletCenter.x > Vector2Fight.Right * factor || 
                bulletCenter.y < Vector2Fight.Down * factor || 
                bulletCenter.y > Vector2Fight.Up * factor)
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


    public void SetBoxSize(float width, float length)
    {
        var size = Renderer.transform.localScale;
        size.x = length;
        size.y = width;
        Renderer.transform.localScale = size;
        
        //中心点位置
        if (Deploy.centerPivot == 1)
        {
            Renderer.transform.localPosition = new Vector3(0, length / 2f, 0);
        }
        else if (Deploy.centerPivot == 2)
        {
            Renderer.transform.localPosition = new Vector3(0, -length / 2f, 0);
        }

        CollisionInfo.IsBox = true;
        CollisionInfo.BoxWidth = width * Deploy.radius;
        CollisionInfo.BoxHeight = length * Deploy.radius;
    }

    public override void OnRecycle()
    {
        base.OnRecycle();

        SetBoundDestroy(true);

        OnDestroyCallBack?.Invoke(this);
        OnDestroyCallBack = null;

        _currAniIdx = 0;
        _totalFrame = 0;
        Renderer.sortingOrder = DefaultSortOrder;
    }
}


public class BulletDeploy : Conditionable
{
    public int id;
    public string classType;
    public int resourceId;
    public float scale;
    public float alpha;
    public float radius;
    public int spriteIdx;
    public int bombEffectId;
    public float bombEffectSpeed;
    public int centerPivot;
    public bool isAni;
    public float brightness;
    public bool delayDestroy;
    public EColor EColor;
}