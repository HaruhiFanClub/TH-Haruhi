﻿
//子弹工厂，缓存池
using System;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using DG.Tweening;

public static class BulletFactory
{
    public delegate void CreatedNotify(Bullet bullet);

    public class Holder
    {
        public BulletDeploy Deploy;
        public Sprite Resource;
        public List<Sprite> SpriteList;
        public Queue<Bullet> Queue;
        public float BackToPoolTime;
    }

    private const int OneCacheMax = 100;              //每个种类最多缓存数量

    private static readonly TableT<BulletDeploy> BulletTab;
    private static readonly Dictionary<int, Holder> CachePool;

    public static Transform CacheRoot
    {
        get
        {
            if (!_cacheRoot)
            {
                GameObject factory = GameObjectTools.CreateGameObject("BulletFactory");
                Object.DontDestroyOnLoad(factory);
                _cacheRoot = factory.transform;
                factory.SetActiveSafe(false);
            }
            return _cacheRoot;
        }
    }

    private static Transform _cacheRoot;
    private static Dictionary<int, Material> BulletMaterialCache = new Dictionary<int, Material>();

    static BulletFactory()
    {
        CachePool = new Dictionary<int, Holder>();
        BulletTab = TableUtility.GetTable<BulletDeploy>();
    }

    public static void CreateBullet(int id, Vector3 pos, int layer, CreatedNotify notify)
    {
        var deploy = BulletTab[id];
        if (!deploy)
        {
            Debug.LogError("bullet id : " + id + " not exist");
            notify(null);
        }
        else
        {
            Holder holder;
            if (CachePool.TryGetValue(id, out holder))
            {
                Bullet bullet;
                if (holder.Queue.Count > 0)
                {
                    bullet = holder.Queue.Dequeue();
                    if (bullet)
                    {
                        bullet.gameObject.layer = layer;
                        bullet.SetInCache(false);
                        bullet.transform.SetParent(null, false);
                        bullet.OnCreate(pos);
                    }
                    notify(bullet);
                }
                else
                {
                    CreateBulletDirect(holder.SpriteList, holder.Resource,deploy, pos, layer, bulletObj =>
                    {
                        bullet = bulletObj;
                        notify(bullet);
                    });
                }
            }
            else
            {
                CreateNewBullet(id, layer, deploy, pos, notify);
            }
        }
    }

    private static void CreateNewBullet(int id, int layer, BulletDeploy deploy, Vector3 pos, CreatedNotify notify)
    {
        GameSystem.Start(TextureUtility.LoadResourceById(deploy.resourceId, spriteList =>
        {
            int spriteIdx = deploy.spriteIdx;
            if(deploy.spriteIdx > spriteList.Count)
            {
                spriteIdx = 0;
            }
            var sprite = spriteList[spriteIdx];
            Holder holder;
            if (!CachePool.TryGetValue(id, out holder))
            {
                holder = new Holder
                {
                    Deploy = deploy,
                    Resource = sprite,
                    SpriteList = spriteList,
                    Queue = new Queue<Bullet>()
                };
                CachePool.Add(id, holder);
            }

            CreateBulletDirect(spriteList, sprite, deploy, pos, layer, bullet =>
            {
                notify(bullet);
            });
        }));
    }

    public static void DestroyBullet(Bullet bullet)
    {
        if (!bullet) return;

        //Debug.LogError("destory bullet:" + bullet.Deploy.id);

        if (bullet.InCache)
        {
            Debug.LogError("invalidate to destroy bullet in cache, " + bullet.Deploy.id); 
        }
        else
        {
            Holder holder;
            if (!CachePool.TryGetValue(bullet.Deploy.id, out holder) || holder.Queue.Count >= OneCacheMax)
                EntityBase.DestroyEntity(bullet);
            else
            {
                bullet.OnRecycle();
                bullet.SetInCache(true);
                bullet.transform.SetParent(CacheRoot, false);
                holder.Queue.Enqueue(bullet);
                holder.BackToPoolTime = Time.realtimeSinceStartup;
            }
        }
    }

    
    private static void CreateBulletDirect(List<Sprite> spriteList, Sprite resource, BulletDeploy deploy, Vector3 pos, int layer,  Action<Bullet> notify)
    {
        Type type = null;

        var bulletClass = deploy.classType;
        if (!string.IsNullOrEmpty(bulletClass))
            if ((type = Common.GetType(bulletClass)) == null)
                Debug.LogError("bullet class : " + bulletClass + " not exist");

        if (type == null) type = typeof(Bullet);

        var _object = new GameObject("bullet_" + deploy.id);
        _object.transform.localScale = deploy.scale * Vector3.one;

        var model = new GameObject("model");
        model.transform.SetParent(_object.transform, false);
        model.AddComponent<MeshFilter>().sharedMesh = GameSystem.DefaultRes.QuadMesh;

        Material material;
        if (!BulletMaterialCache.TryGetValue(deploy.id, out material))
        {
            material = new Material(GameSystem.DefaultRes.AlphaBlended);
            material.mainTexture = resource.texture;
            material.SetFloat("_AlphaScale", deploy.alpha);
            material.SetFloat("_Brightness", deploy.brightness);
            

            BulletMaterialCache[deploy.id] = material;
        }

        var mr = model.AddComponent<MeshRenderer>();
        mr.sharedMaterial = material;

        switch (layer)
        {
            case Layers.PlayerBullet:
                mr.sortingOrder = SortingOrder.PlayerBullet;
                break;
            case Layers.EnemyBullet:
                mr.sortingOrder = SortingOrder.EnemyBullet;
                break;
            case Layers.Ui:
                mr.sortingOrder = SortingOrder.Top;
                break;
        }

        //图片要默认转90度
        model.transform.localEulerAngles = new Vector3(0, 0, 90f);

        var sizeX = resource.bounds.size.x;
        var sizeY = resource.bounds.size.y;

        model.transform.localScale = new Vector3(sizeX, sizeY, 1);

        //中心点位置
        if(deploy.centerPivot == 1)
        {
            model.transform.localPosition = new Vector3(0, sizeX / 2f, 0);
        }
        else if(deploy.centerPivot == 2)
        {
            model.transform.localPosition = new Vector3(0, -sizeX / 2f, 0);
        }

        var collisionInfo = new Bullet.ColliderInfo
        {
            IsBox = false,
            Radius = deploy.radius,
            Center = model.transform.localPosition
        };

        var bullet = _object.AddComponent(type) as Bullet;
        if (!bullet)
        {
            Debug.LogError("CreateBulletDirect, Object is null 222");
            Object.Destroy(_object);
        }
        else
        {
            bullet.CollisionInfo = collisionInfo;
            bullet.gameObject.layer = layer;
            bullet.SetRenderer(mr);
            bullet.Init(deploy);
            bullet.OnCreate(pos);
        }

        //ani
        if (deploy.isAni)
        {
            bullet.AniList = spriteList;
        }

        notify(bullet);
    }


    private static readonly List<Holder> HolderList = new List<Holder>();

    private static void ReleasePool(int removeCount)
    {
        HolderList.Clear();
        var e = CachePool.GetEnumerator();
        using (e)
        {
            while (e.MoveNext())
            {
                HolderList.Add(e.Current.Value);
            }
        }
        HolderList.Sort(HolderComparison);

        if (removeCount > HolderList.Count)
            removeCount = HolderList.Count;

        for (int i = 0; i < removeCount; i++)
        {
            var queue = HolderList[i].Queue.GetEnumerator();
            using (queue)
            {
                while (queue.MoveNext())
                {
                    var prop = queue.Current;
                    EntityBase.DestroyEntity(prop);
                }
            }
            HolderList[i].Queue.Clear();
            CachePool.Remove(HolderList[i].Deploy.id);
        }
    }

    private static int HolderComparison(Holder x, Holder y)
    {
        if (x.BackToPoolTime < y.BackToPoolTime)
            return -1;
        return x.BackToPoolTime > y.BackToPoolTime ? 1 : 0;
    }

    public static void BackAllToPool()
    {
        var bullets = Resources.FindObjectsOfTypeAll<Bullet>();
        for (int i = 0; i < bullets.Length; i++)
        {
            if (bullets[i].transform.parent != CacheRoot)
                DestroyBullet(bullets[i]);
        }
    }
}
