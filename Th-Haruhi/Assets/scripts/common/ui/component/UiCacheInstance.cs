
//////////////////////////////////////////////////////////////////////////
//
//   FileName : XUI_Instance.cs
//     Author : Felon
// CreateTime : 2017-05-02
//       Desc :
//
//////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using UnityEngine;



public abstract class UiCacheInstance : UiInstance
{
    private static readonly Dictionary<Type, List<UiCacheInstance>> CachePool;
    private static readonly Dictionary<Type, List<UiCacheInstance>> ActiveList;

    static UiCacheInstance()
    {
        CachePool = new Dictionary<Type, List<UiCacheInstance>>();
        ActiveList = new Dictionary<Type, List<UiCacheInstance>>();
    }

    public bool InCache;

    private static Transform _cacheRoot;

    private static Transform CacheRoot
    {
        get
        {
            if (_cacheRoot) return _cacheRoot;
            var factory = GameObjectTools.CreateGameObject("XUIFactroy");
            DontDestroyOnLoad(factory);
            _cacheRoot = factory.transform;
            factory.SetActiveSafe(false);
            return _cacheRoot;
        }
    }

    private static List<UiCacheInstance> GetCachePool(Type t)
    {
        List<UiCacheInstance> caches;
        if (!CachePool.TryGetValue(t, out caches))
        {
            caches = new List<UiCacheInstance>();
            CachePool.Add(t, caches);
        }
        return caches;
    }

    private static List<UiCacheInstance> GetActiveInstances(Type t)
    {
        List<UiCacheInstance> caches;
        if (!ActiveList.TryGetValue(t, out caches))
        {
            caches = new List<UiCacheInstance>();
            ActiveList.Add(t, caches);
        }
        return caches;
    }

    protected static void Recycle(UiCacheInstance inst, Type t)
    {
        if (inst.InCache) return;

        var actives = GetActiveInstances(t);
        actives.Remove(inst);

        var caches = GetCachePool(t);

        //缓存满了，直接destroy
        if (caches.Count >= inst.GetMaxCacheCount())
        {
            inst.Close();
        }
        else
        {
            inst.InCache = true;
            inst.SetActiveByCanvasGroup(false);
            inst.OnRecycle();

            if (inst.TransForm.parent != CacheRoot)
            {
                inst.InCache = true;
                inst.TransForm.SetParent(CacheRoot, false);
            }

            if (!caches.Contains(inst))
                caches.Add(inst);
        }
    }

    protected static T CreateInstFromPool<T>(Transform parent = null) where T : UiCacheInstance
    {
        var caches = GetCachePool(typeof(T));
        var actives = GetActiveInstances(typeof(T));

        if (caches.Count == 0)
        {
            CacheNew<T>();
        }
        return Create<T>(caches, actives, parent);
    }

    private static T Create<T>(List<UiCacheInstance> caches, List<UiCacheInstance> actives, Transform parent = null) where T : UiCacheInstance
    {
        UiCacheInstance inst = null;
        if (caches.Count == 0)
        {
            if (actives.Count > 0)
            {
                Recycle(actives[0], actives[0].GetType());
            }
        }

        if (caches.Count > 0)
        {
            inst = caches[0];
            inst.InCache = false;
            inst.SetActiveByCanvasGroup(true);
            caches.RemoveAt(0);
        }


        if (inst == null)
        {
            Debug.LogError("GetInstFromPool == null, 池内没有可用的UIInst，Type:" + typeof(T));
        }
        else
        {
            actives.Add(inst);

            //parent
            if (parent == null)
            {
                var t = UiManager.GetCanvas(inst.UiInfo.Layer);
                inst.TransForm.SetParent(t, false);
                inst.TransForm.SetAsLastSibling();
            }
            else
            {
                inst.transform.SetParent(parent, false);
                inst.TransForm.SetAsLastSibling();
            }
        }

        return inst as T;
    }

    private static void CacheNew<T>() where T : UiCacheInstance
    {
        var caches = GetCachePool(typeof(T));
        var v = UiManager.ImmediatelyShow<T>();
        v.InCache = true;
        v.SetActiveByCanvasGroup(false);
        caches.Add(v);
    }

    public static void BackAllToPool()
    {
        var cache = Resources.FindObjectsOfTypeAll<UiCacheInstance>();
        for (int i = 0; i < cache.Length; i++)
        {
            Recycle(cache[i], cache[i].GetType());
        }
    }

    protected abstract int GetMaxCacheCount();

    protected virtual void OnRecycle()
    {

    }
}
