using UnityEngine;
using DG.Tweening;
using System.Collections;

public class BulletExplosion
{
    private static IEnumerator DoCreate(Vector3 pos, float delay)
    {
        yield return new WaitForSeconds(delay);
        var gameObj = new GameObject("BulletExplosionObj");
        gameObj.transform.position = pos;
        gameObj.layer = Layers.BulletDestroy;

        var circleCollider = gameObj.AddComponent<CircleCollider2D>();
        circleCollider.radius = 1f;

        var rigid = gameObj.AddComponent<Rigidbody2D>();
        rigid.bodyType = RigidbodyType2D.Kinematic;
        rigid.simulated = true;

        gameObj.AddComponent<BulletExplosionCollider>();
        gameObj.transform.DOScale(20f, 1.2f).onComplete = ()=>
        {
            Object.Destroy(gameObj);
        };
    }

    public static void Create(Vector3 pos, float delay)
    {
        GameSystem.CoroutineStart(DoCreate(pos, delay));
    }
}