
using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Object = UnityEngine.Object;

public class EffectFactory : MonoBehaviour
{
    public delegate void CreatedNotify(Effect effect);

    public class Holder
    {
        public string EffectName;
        public Object Resource;
        public Queue<Effect> Queue;
        public float BackToPoolTime;
    }

    private const int TypeMaxCount = 50;            //不同种类(Id)的最多缓存数量s
    private const int OneCacheMax = 5;              //每个种类最多缓存数量
    private const int OnceReleaseTypeCount = 5;     //当种类超出时，每次释放几个种类
    private const int RetainTypeCount = 25;         //游戏结束后，留几个种类在池中

    public static Dictionary<string, Holder> CachePool;

    private static Transform CacheRoot
    {
        get
        {
            if(_cacheRoot == null)
            {
                GameObject pool = GameObjectTools.CreateGameObject("EffectPool");
                DontDestroyOnLoad(pool);
                _cacheRoot = pool.transform;
                pool.SetActiveSafe(false);
            }
            return _cacheRoot;


        }
    }
    private static Transform _cacheRoot;

    static EffectFactory()
    {
        CachePool = new Dictionary<string, Holder>();
    }

    public static void PlayEffectOnce(string effectName, Vector3 pos)
    {
        CreateEffect(effectName, effect =>
        {
            effect.AutoDestory();
            effect.transform.position = pos;
            effect.Play();
        });
    }
    public static void CreateEffect(string effectName, CreatedNotify notify, bool isImport = false)
    {
        DoCreateEffect(effectName, isImport, notify);
    }

    public static void DoCreateEffect(string effectName, bool isImport, CreatedNotify notify)
    {
        if (string.IsNullOrEmpty(effectName))
        {
            notify(null);
        }
        else
        {
            Holder holder;
            if (CachePool.TryGetValue(effectName, out holder))
            {
                Queue<Effect> queue = holder.Queue;
                Effect effect;
                if (queue.Count > 0)
                {
                    effect = queue.Dequeue();
                    if (!effect)
                    {
                        Debug.LogError("invalidate effect from cache, " + effectName);
                    }
                    else
                    {
                        effect.InCache = false;
                        effect.transform.SetParent(null, false);
                        effect.SetActiveSafe(true);
                    }
                    notify(effect);
                }
                else
                {
                    CreateEffectDirect(holder.Resource, isImport, obj =>
                    {
                        effect = obj;
                        effect.CacheName = effectName;
                        notify(effect);
                    });
                }

            }
            else
            {
                CreateEffectLoad(effectName, isImport, notify);
            }
        }
    }

    private static void CreateEffectLoad(string effectName, bool isImport, CreatedNotify notify)
    {
        ResourceMgr.Load(effectName, resource =>
        {
            Holder holder;
            if (!CachePool.TryGetValue(effectName, out holder))
            {
                holder = new Holder
                {
                    EffectName = effectName,
                    Resource = resource,
                    Queue = new Queue<Effect>()
                };
                CachePool.Add(effectName, holder);
            }

            CreateEffectDirect(holder.Resource, isImport, effect =>
            {
                effect.CacheName = effectName;
                notify(effect);
            });
        });
    }

    public static void DestroyEffect(Effect effect, float delay = 0f, bool cacheInPool = true)
    {
        if (!effect) return;
        if (delay <= 0f)
            DestroyEffectImpl(effect);
        else
        {
            DOVirtual.DelayedCall(delay, () =>
            {
                DestroyEffectImpl(effect);
            });
        }
    }

    private static void DestroyEffectImpl(Effect effect)
    {
        if (!effect)
            return;

        if (effect.InCache)
        {
            //Debug.LogWarning("invalidate to destroy effect in cache, " + effect.name);
        }
        else
        {
            Holder holder;
            if (string.IsNullOrEmpty(effect.CacheName) || !CachePool.TryGetValue(effect.CacheName, out holder) ||
                holder.Queue.Count >= OneCacheMax)
            {
                GameObjectTools.DestroyGameObject(effect);
            }
            else
            {
                //超出数量，则释放一些
                if (CachePool.Count > TypeMaxCount)
                {
                    ReleasePool(OnceReleaseTypeCount);
                }

                effect.InCache = true;
                effect.OnPreDestroy();
                effect.AutoDestory(false);
                effect.transform.SetParent(CacheRoot, false);
                holder.BackToPoolTime = Time.realtimeSinceStartup;
                holder.Queue.Enqueue(effect);
            }
        }
    }

    public static void CreateEffectDirect(Object resource, bool isImport, Action<Effect> notify)
    {
        if (resource == null)
        {
            notify(null);
            return;
        }

        if (isImport)
        {
            var gameObj = ResourceMgr.Instantiate(resource);
            if (gameObj != null)
            {
                var effect = gameObj.AddComponent<Effect>();
                effect.Init();
                notify(effect);
            }
            else
            {
                notify(null);
            }
        }
        else
        {
            ResourceMgr.InstantiateX(resource, gameObj =>
            {
                if (gameObj != null)
                {
                    var effect = gameObj.AddComponent<Effect>();
                    effect.Init();
                    notify(effect);
                }
                else
                {
                    notify(null);
                }
            });
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
                    var effect = queue.Current;
                    GameObjectTools.DestroyGameObject(effect);
                }
            }
            HolderList[i].Queue.Clear();
            CachePool.Remove(HolderList[i].EffectName);
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
        //所有存在的特效进池
        var effects = Resources.FindObjectsOfTypeAll<Effect>();
        for (int i = 0; i < effects.Length; i++)
        {
            if (effects[i].transform.parent != CacheRoot)
                DestroyEffect(effects[i]);
        }

        //池子里只保留一半
        if (CachePool.Count > RetainTypeCount)
        {
            var releaseCount = CachePool.Count - RetainTypeCount;
            ReleasePool(releaseCount);
        }
    }
}
