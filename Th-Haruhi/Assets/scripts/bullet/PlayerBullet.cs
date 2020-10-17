using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerBullet : Bullet
{
    private bool _bHitEnemy;
    private float _hitEnemyFrame;

    public override void Shoot(MoveData moveData, List<EventData> eventList = null, int atk = 1, bool boundDestroy = true, Action<Bullet> onDestroy = null)
    {
        base.Shoot(moveData, eventList, atk, boundDestroy, onDestroy);
        _bHitEnemy = false;
    }

    protected override void Update()
    {
        base.Update();
        CheckHitEnemy();
    }

    //Update 检测子弹是否命中敌人
    protected virtual void CheckHitEnemy()
    {
        if (InCache) return;
        if (!Shooted) return;

        //已经击中敌人的不处理，且0.1s后销毁子弹
        if (_bHitEnemy)
        {
            if(Deploy.delayDestroy && GameSystem.FixedFrameCount - _hitEnemyFrame > 6)
            {
                DoBulletBomb();
            }
            return;
        }

        var bulletCenter = CacheTransform.position + CollisionInfo.Center;

        //超出边界，自动销毁
        if (CheckBulletOutSide(bulletCenter))
        {
            return;
        }

        //检测玩家子弹是否击中敌人
        var hit = Physics2D.Raycast(bulletCenter, CacheTransform.up, CollisionInfo.Radius, LayersMask.Enemy);
        if (hit.collider != null)
        {
            var gameObj = hit.collider.gameObject;
            if (gameObj.layer == Layers.Enemy)
            {
                var enemy = gameObj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    //扣血
                    enemy.OnEnemyHit(Atk);

                    //命中效果
                    OnBulletHitEnemy(enemy);
                }
            }
        }
    }

    //子弹命中敌人时处理
    protected virtual void OnBulletHitEnemy(Enemy enemy)
    {
        _bHitEnemy = true;
        _hitEnemyFrame = GameSystem.FixedFrameCount;

        //击中敌人后速度减慢
        if (Deploy.delayDestroy)
        {
            MoveData.Speed = 8f;
            Renderer.material.SetFloat("_Brightness", 3f);
        }
        else
        {
            DoBulletBomb();
        }
    }

    //子弹爆炸，销毁子弹
    protected virtual void DoBulletBomb()
    {
        if (InCache)
        {
            return;
        }

        //播放爆炸特效
        if (Deploy.bombEffectId > 0)
        {
            var pos = Renderer.transform.position;
            var forward = MoveData.Forward;
            var speed = Deploy.bombEffectSpeed;
            TextureEffectFactroy.CreateEffect(Deploy.bombEffectId, SortingOrder.EnemyBullet, effect =>
            {
                effect.transform.position = pos;
                effect.AutoDestroy();
                if(speed > 0)
                {
                    effect.SetAutoMove(forward, speed);
                }
            });
        }

        //销毁子弹
        BulletFactory.DestroyBullet(this);
    }

    public override void OnRecycle()
    {
        base.OnRecycle();
        _bHitEnemy = false;
        Renderer.material.SetFloat("_Brightness", 1f);
    }
}
