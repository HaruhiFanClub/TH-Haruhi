using System;

public class Singleton<T> where T : class, new()
{
    private static T _instance;

    protected Singleton()
    {
    }

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                CreateInstance();
            }
            return _instance;
        }
    }

    private static void CreateInstance()
    {
        if (_instance != null) return;
        _instance = Activator.CreateInstance<T>();
        var ins = (_instance as Singleton<T>);
        if (ins != null) ins.Init();
    }

    public void DestroyInstance()
    {
        if (_instance == null) return;
        var ins = (_instance as Singleton<T>);
        if (ins != null) ins.Destroy();
        _instance = null;
    }

    public static T GetInstance()
    {
        if (_instance == null)
        {
            CreateInstance();
        }
        return _instance;
    }

    public static bool HasInstance()
    {
        return (_instance != null);
    }

    protected virtual void Init()
    {
    }

    protected virtual void Destroy()
    {
    }
}

