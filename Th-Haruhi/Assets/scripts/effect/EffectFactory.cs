
using System;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public static class EffectFactory
{
    public delegate void CreatedNotify(Effect effect);

    public class Holder
    {
        public EffectDeploy Deploy;
        public List<Sprite> Resource;
        public Queue<Effect> Queue;
        public float BackToPoolTime;
    }

    private const int OneCacheMax = 100;              //每个种类最多缓存数量

    private static readonly TableT<EffectDeploy> EffectTab;
    private static readonly Dictionary<int, Holder> CachePool;

    public static Transform CacheRoot
    {
        get
        {
            if (!_cacheRoot)
            {
                GameObject factory = GameObjectTools.CreateGameObject("EffectFactory");
                Object.DontDestroyOnLoad(factory);
                _cacheRoot = factory.transform;
                factory.SetActiveSafe(false);
            }
            return _cacheRoot;
        }
    }

    private static Transform _cacheRoot;
    private static Dictionary<int, Material> _materialCache = new Dictionary<int, Material>();

    static EffectFactory()
    {
        CachePool = new Dictionary<int, Holder>();
        EffectTab = TableUtility.GetTable<EffectDeploy>();
    }

    public static void CreateEffect(int id, int sortingOrder, CreatedNotify notify)
    {
        var deploy = EffectTab[id];
        if (!deploy)
        {
            Debug.LogError("effect id : " + id + " not exist");
            notify(null);
        }
        else
        {
            Holder holder;
            if (CachePool.TryGetValue(id, out holder))
            {
                Effect effect;
                if (holder.Queue.Count > 0)
                {
                    effect = holder.Queue.Dequeue();
                    if (effect)
                    {
                        effect.Renderer.sortingOrder = sortingOrder;
                        effect.SetInCache(false);
                        effect.transform.SetParent(null, false);
                        effect.ReInit();
                    }
                    notify(effect);
                }
                else
                {
                    CreateEffectDirect(holder.Resource, sortingOrder, deploy, effectObj =>
                    {
                        effect = effectObj;
                        notify(effect);
                    });
                }
            }
            else
            {
                CreateNewEffect(id, sortingOrder, deploy, notify);
            }
        }
    }

    private static void CreateNewEffect(int id, int sortingOrder, EffectDeploy deploy, CreatedNotify notify)
    {
        GameSystem.CoroutineStart(TextureUtility.LoadResourceById(deploy.resourceId, spriteList =>
        {
            Holder holder;
            if (!CachePool.TryGetValue(id, out holder))
            {
                holder = new Holder
                {
                    Deploy = deploy,
                    Resource = spriteList,
                    Queue = new Queue<Effect>()
                };
                CachePool.Add(id, holder);
            }

            CreateEffectDirect(spriteList, sortingOrder, deploy, effect =>
            {
                notify(effect);
            });
        }));
    }

    public static void DestroyEffect(Effect effect)
    {
        if (!effect) return;

        //Debug.LogError("destory effect:" + effect.Deploy.id);

        if (effect.InCache)
        {
            Debug.LogError("invalidate to destroy effect in cache, " + effect.Deploy.id);
        }
        else
        {
            Holder holder;
            if (!CachePool.TryGetValue(effect.Deploy.id, out holder) || holder.Queue.Count >= OneCacheMax)
                EntityBase.DestroyEntity(effect);
            else
            {
                effect.OnRecycle();
                effect.SetInCache(true);
                effect.transform.SetParent(CacheRoot, false);
                holder.Queue.Enqueue(effect);
                holder.BackToPoolTime = Time.realtimeSinceStartup;
            }
        }
    }


    private static void CreateEffectDirect(List<Sprite> spriteList, int sortingOrder, EffectDeploy deploy,  Action<Effect> notify)
    {
        var type = typeof(Effect);
        var resource = spriteList[0];

        var _object = new GameObject("effect_" + deploy.id);
        _object.transform.localScale = deploy.scale * Vector3.one;

        var model = new GameObject("model");
        model.transform.SetParent(_object.transform, false);
        model.AddComponent<MeshFilter>().sharedMesh = GameSystem.DefaultRes.QuadMesh;

        Material material;
        if (!_materialCache.TryGetValue(deploy.id, out material))
        {
            material = new Material(GameSystem.DefaultRes.BulletShader)
            {
                mainTexture = resource.texture
            };
            material.SetFloat("_AlphaScale", deploy.alpha);

            _materialCache[deploy.id] = material;
        }

        var mr = model.AddComponent<MeshRenderer>();
        mr.sharedMaterial = material;
        

        model.transform.localEulerAngles = new Vector3(0, 0, deploy.rota);

        var sizeX = resource.bounds.size.x;
        var sizeY = resource.bounds.size.y;

        model.transform.localScale = new Vector3(sizeX, sizeY, 1);

        //添加自动效果
        AddAutoEffect(deploy, model);

        var effect = _object.AddComponent(type) as Effect;
        if (!effect)
        {
            Debug.LogError("CreateEffectDirect, Object is null 222");
            Object.Destroy(_object);
        }
        else
        {
            effect.Renderer = mr;
            effect.Renderer.sortingOrder = sortingOrder;
            effect.Init(deploy, spriteList);
        }

        notify(effect);
    }

    private static void AddAutoEffect(EffectDeploy deploy, GameObject model)
    {
        //添加效果
        if (deploy.AutoRotation != null && deploy.AutoRotation.Length > 0)
        {
            //[方向,速度]
            var autoRota = model.AddComponent<AutoRotation>();
            autoRota.RotaFoward = deploy.AutoRotation[0];
            autoRota.TurnSpeed = deploy.AutoRotation[1];
        }

        if (deploy.AutoScale != null && deploy.AutoScale.Length > 0)
        {
            //[最小,最大,速度]
            var autoScale = model.AddComponent<AutoScale>();
            autoScale.MinScale = deploy.AutoScale[0];
            autoScale.MaxScale = deploy.AutoScale[1];
            autoScale.Speed = deploy.AutoScale[2];
        }


        if (deploy.AutoGamma != null && deploy.AutoGamma.Length > 0)
        {
            //[最小,最大,速度]
            var autoGamma = model.AddComponent<AutoGamma>();
            autoGamma.MinGamma = deploy.AutoGamma[0];
            autoGamma.MaxGamma = deploy.AutoGamma[1];
            autoGamma.Speed = deploy.AutoGamma[2];
        }
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
        var effects = Resources.FindObjectsOfTypeAll<Effect>();
        for (int i = 0; i < effects.Length; i++)
        {
            if (effects[i].transform.parent != CacheRoot)
                DestroyEffect(effects[i]);
        }
    }
}
