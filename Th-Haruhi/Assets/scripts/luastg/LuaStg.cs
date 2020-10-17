
using System.Collections.Generic;
using System.IO;
using InControl;
using LitJson;
using UnityEngine;

public class LuaStgCfg
{
    public List<LuaStgInfo> Main;
}

public class LuaStgInfo
{
    public List<string> attr;
    public string type;
    public bool expand;
    public List<LuaStgInfo> child;
}

public static class LuaStg 
{
    public static void LoadLuaSTG(string name)
    {
        var source = LocalStorage.Read<string>(name, LocalStorage.EStorageType.BulletCfg);
        var data = JsonMapper.ToObject<LuaStgCfg>(source);
        Debug.LogError("1");
    }

    public static float Sin(float degree)
    {
        return Mathf.Sin(Mathf.Deg2Rad * degree);
    }
    public static float Cos(float degree)
    {
        return Mathf.Cos(Mathf.Deg2Rad * degree);
    }

    public static float ToLuaStgSpeed(this float unitySpeed)
    {
        return unitySpeed * 2.3f;
    }
}