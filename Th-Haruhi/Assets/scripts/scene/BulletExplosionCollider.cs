using UnityEngine;

public class BulletExplosionCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == Layers.EnemyBullet)
        {
            var bullet = collision.gameObject.GetComponent<Bullet>();
            if(bullet != null && !bullet.InCache && bullet.AutoDestroy)
            {
                var pos = collision.gameObject.transform.position;
                BulletFactory.DestroyBullet(bullet);

                TextureEffectFactroy.CreateEffect(501, SortingOrder.ShootEffect, effect =>
                {
                    effect.transform.position = pos;
                    effect.AutoDestroy();
                });
            }
        }
    }
}