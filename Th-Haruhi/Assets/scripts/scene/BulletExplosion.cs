using UnityEngine;
using DG.Tweening;
using System.Collections;

public class BulletExplosion
{
    public static bool InExplosion;
    public static Vector3 Center;
    public static float Radius;

    public static void Reset()
    {
        InExplosion = false;
    }
    private static IEnumerator DoCreate(Vector3 pos, float delay)
    {
        yield return new WaitForSeconds(delay);
        var gameObj = new GameObject("BulletExplosionObj");
        InExplosion = true;
        Center = pos;
        Radius = 0;

        gameObj.transform.position = pos;
        var t = gameObj.transform.DOScale(600f, 1.2f);
        t.onUpdate = () =>
        {
            Radius = gameObj.transform.localScale.x;
        };

        t.onComplete = ()=>
        {
            InExplosion = false;
            Object.Destroy(gameObj);
        };
    }

    public static void Create(Vector3 pos, float delay)
    {
        GameSystem.Start(DoCreate(pos, delay));
    }
}