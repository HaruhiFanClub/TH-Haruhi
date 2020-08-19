

using System;
using System.Collections.Generic;

public static class Pool
{
    class ObjectPool
    {
        internal int capacity;
        internal Queue<IPool> objectQueue;
    }

    static int defaultCapacity;
    static Dictionary<Type, ObjectPool> poolDict;

    static Pool()
    {
        poolDict =
            new Dictionary<Type, ObjectPool>();
        defaultCapacity = 1024;
    }

    private static void SetCapacity(Type type, int capacity)
    {
        ObjectPool objectPool;
        if (!poolDict.TryGetValue(type, out objectPool))
        {
            objectPool = new ObjectPool();
            objectPool.objectQueue = new Queue<IPool>();
            poolDict.Add(type, objectPool);
        }
        objectPool.capacity = capacity;
    }

    public static void SetCapacity<T>(int capacity) where T : IPool
    {
        SetCapacity(typeof(T), capacity);
    }

    public static void Cache<T>(int count) where T : IPool, new()
    {
        for (int i = 0; i < count; i++)
        {
            IPool o = new T();
            o.Init();
            o.Reset();
            o.SetInPool(true);
        }
    }

    public static IPool New<T>(Type type, Action<object> beforeInit) where T : IPool, new()
    {
        ObjectPool objectPool;
        IPool o = null;
        if (poolDict.TryGetValue(type, out objectPool))
        {
            if (objectPool.objectQueue.Count > 0)
            {
                o = objectPool.objectQueue.Dequeue();
            }
        }

        if (o == null)
        {
            o = Common.CreateInstance(type) as T;
            if (o != null)
            {
                beforeInit(o);
                o.Init();
            }
        }
        else
        {
            beforeInit(o);
            o.Reset();
            o.SetInPool(false);
        }

        return o;
    }

    public static IPool New<T>() where T : IPool, new()
    {
        ObjectPool objectPool;
        IPool o = null;
        if (poolDict.TryGetValue(typeof(T), out objectPool))
        {
            if (objectPool.objectQueue.Count > 0)
            {
                o = objectPool.objectQueue.Dequeue();
            }
        }

        if (o == null)
        {
            o = new T();
            o.Init();
        }
        
        o.Reset();
        o.SetInPool(false);
        return o;
    }

    public static void Free(IPool _object)
    {
        if (_object == null)
            return;

        ObjectPool objectPool;
        Type type = _object.GetType();

        if (!poolDict.TryGetValue(type, out objectPool))
        {
            objectPool = new ObjectPool();
            objectPool.capacity = defaultCapacity;
            objectPool.objectQueue = new Queue<IPool>();
            poolDict.Add(type, objectPool);
        }

        if (objectPool.objectQueue.Count < objectPool.capacity && !_object.GetInPool())
        {
            objectPool.objectQueue.Enqueue(_object);
            _object.Recycle();
            _object.SetInPool(true);
        }
        else
        {
            _object.OnDestroy();
        }
    }

    public static void Clear(Type type = null)
    {
        if (type == null)
            poolDict.Clear();
        else
        {
            ObjectPool objectPool;
            if (poolDict.TryGetValue(type, out objectPool))
                objectPool.objectQueue.Clear();
        }
    }
}