
using System;
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;

public class BulletDestroyCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == Layers.EnemyBullet || collision.gameObject.layer == Layers.PlayerBullet)
        {
            var bullet = collision.gameObject.GetComponent<Bullet>();
            if(bullet != null && !bullet.InCache && bullet.AutoDestroy)
            {
                BulletFactory.DestroyBullet(bullet);
            }
        }
    }
}