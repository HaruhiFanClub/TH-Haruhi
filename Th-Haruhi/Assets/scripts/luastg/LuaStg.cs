
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
    public const float LuaStgSpeedChange = 2.3f;

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

    public static Vector3 AngleToForward(this float angle)
    {
        return Quaternion.Euler(0, 0, angle) * Vector3.up;
    }

    public static float RandomSign()
    {
        return Random.Range(0, 2) == 0 ? -1 : 1;
    }

    public static float AnglePlayer(this Transform trans)
    {
        var f = Player.CurrPos - trans.position;
        return Vector3.Angle(f, Vector3.up);
    }

    public static LuaStgTask CreateTask(this EntityBase master)
    {
        var luaStgTask = master.gameObject.AddComponent<LuaStgTask>();
        return luaStgTask;
    }
}