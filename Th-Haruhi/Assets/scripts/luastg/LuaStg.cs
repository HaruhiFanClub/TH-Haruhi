
using LitJson;
using System;
using System.Collections.Generic;
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
    public const float LuaStgSpeedChange = 2.2f;

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
        return UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
    }

    public static float AnglePlayer(this Transform trans)
    {
        var playerPos = Player.CurrPos;
        var targetPos = trans.position;
        var f = playerPos - targetPos;
        var a = Vector3.Angle(f, Vector3.down);

        //判断左右，右侧 180 + a   左侧 180 - a
        if(playerPos.x > targetPos.x)
        {
            a = 180f + a;
        }
        else
        {
            a = 180f - a;
        }
        return a;
    }

    public static LuaStgTask CreateTask(this EntityBase master)
    {
        var luaStgTask = master.gameObject.AddComponent<LuaStgTask>();
        return luaStgTask;
    }

    public static void RemoveAllTask(this EntityBase master)
    {
        var tasks = master.GetComponents<LuaStgTask>();
        for(int i = 0; i < tasks.Length; i++)
        {
            UnityEngine.Object.Destroy(tasks[i]);
        }
    }

    public static void ShootEnemyBullet(int id, float x, float y,
        Action<Bullet> onCreate = null, Action<Bullet> onDestroy = null, float shootEffectScale = 1.5f)
    {
        if (BulletExplosion.InExplosion) return;

        var pos = Vector2Fight.NewWorld(x, y);
        BulletFactory.CreateBullet(id, pos, Layers.EnemyBullet, bullet =>
        {
            bullet.OnDestroyCallBack = onDestroy;

            if (shootEffectScale > 0) bullet.PlayShootEffect(shootEffectScale);
            onCreate?.Invoke(bullet);
        });
    }

    public static void ShootLaser(int id, float length, float width, int turnOnFrame, float x, float y, Action<Laser> onCreate = null, Action<Bullet> onDestroy = null)
    {
        if (BulletExplosion.InExplosion) return;

        var pos = Vector2Fight.NewWorld(x, y);

        BulletFactory.CreateBullet(id, pos, Layers.EnemyBullet, bullet =>
        {
            var laser = (Laser)bullet;
            laser.transform.localScale = Vector2Fight.NewLocal(width, length);
            laser.OnDestroyCallBack = onDestroy;
            laser.TurnOn(turnOnFrame);
            onCreate?.Invoke(laser);
        });
    }
}