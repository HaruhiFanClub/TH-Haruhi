using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet
{

    //是否被擦过弹
    private bool _isGrazed;

    public override void Shoot(MoveData moveData, List<EventData> eventList = null, int atk = 1,Action<Bullet> onDestroy = null)
    {
        base.Shoot(moveData, eventList, atk, onDestroy);
        _isGrazed = false;
    }

    protected virtual void LateUpdate()
    {
        if (InCache) return;
        if (!Shooted) return;

        var bulletCenter = CacheTransform.position + CollisionInfo.Center;

        //超出边界，自动销毁
        if (CheckBulletOutSide(bulletCenter))
        {
            return;
        }
       
        //检测死亡销毁销毁
        if (!CheckBulletExplosion())
        {
            //方形子弹
            if (CollisionInfo.IsBox)
            {
                CheckBoxColliderBullet(bulletCenter);
            }
            //圆形子弹
            else
            {
                CheckCircleColliderHit(bulletCenter);
            }
        }
    }

    //检测玩家死亡时，是否需要自我销毁并播放特效
    private bool CheckBulletExplosion()
    {
        if (!BulletExplosion.InExplosion)
        {
            return false;
        }
        var pos = CacheTransform.position;
        var radius = BulletExplosion.Radius;
        var sqrDist = MathUtility.SqrDistanceXY(pos, BulletExplosion.Center);
        if (sqrDist < radius * radius)
        {
            this.PlayEffectAndDestroy();
            return true;
        }
        return false;
    }

    //circleCollider类型子弹检测命中
    private void CheckCircleColliderHit(Vector3 bulletCenter)
    {
        //无碰撞子弹不处理
        if (CollisionInfo.Radius <= 0 || InHidden || InBanCollision) return;

        var mainPlayer = StageMgr.MainPlayer;
        if (mainPlayer != null)
        {
            var hitDist = CollisionInfo.Radius + mainPlayer.Deploy.radius;
            var sqrDist = MathUtility.SqrDistance(bulletCenter, mainPlayer.transform.position);

            //伤害判定
            var bHit = false;
            if (sqrDist < hitDist * hitDist)
            {
                if (mainPlayer.OnPlayerHit())
                {
                    bHit = true;
                }
            }

            //擦弹判定
            if (!bHit && !_isGrazed)
            {
                var grazeDist = CollisionInfo.Radius + mainPlayer.Deploy.radius + GrizeDistance;
                if (sqrDist < grazeDist * grazeDist)
                {
                    mainPlayer.OnGraze();
                    _isGrazed = true;
                }
            }
        }
    }

    //boxCollider类型子弹检测命中
    private void CheckBoxColliderBullet(Vector3 bulletCenter)
    {
        //无碰撞子弹不处理
        if (CollisionInfo.BoxHeight <= 0 && CollisionInfo.BoxWidth <= 0 || InHidden || InBanCollision) return;

        var distUD = CollisionInfo.BoxHeight / 2f;
        var distLR = CollisionInfo.BoxWidth / 2f;

        var hitUp = Physics2D.Raycast(bulletCenter, CacheTransform.up, distUD + GrizeDistance, LayersMask.Player | LayersMask.BulletDestroy);
        if (CheckBoxHitResult(hitUp, bulletCenter, distUD))
            return;

        var hitDown = Physics2D.Raycast(bulletCenter, -CacheTransform.up, distUD + GrizeDistance, LayersMask.Player | LayersMask.BulletDestroy);
        if (CheckBoxHitResult(hitDown, bulletCenter, distUD))
            return;

        var hitLeft = Physics2D.Raycast(bulletCenter, -CacheTransform.right, distLR + GrizeDistance, LayersMask.Player | LayersMask.BulletDestroy);
        if (CheckBoxHitResult(hitLeft, bulletCenter, distLR))
            return;

        var hitRight = Physics2D.Raycast(bulletCenter, CacheTransform.right, distLR + GrizeDistance, LayersMask.Player | LayersMask.BulletDestroy);
        if (CheckBoxHitResult(hitRight, bulletCenter, distLR))
            return;
    }

    //处理方形子弹是否命中敌人
    private bool CheckBoxHitResult(RaycastHit2D hit, Vector3 startPos, float deadDist)
    {
        if (hit.collider == null) return false;
        if (hit.collider.gameObject.layer == Layers.Player)
        {
            var sqrDist = MathUtility.SqrDistanceXY(hit.point, startPos);
            var player = hit.collider.gameObject.GetComponent<Player>();
            if (player != null)
            {
                //伤害判定
                if (sqrDist < deadDist * deadDist)
                {
                    if (player.OnPlayerHit())
                    {
                        return true;
                    }
                }

                //擦弹判定
                if(!_isGrazed)
                {
                    player.OnGraze();
                    _isGrazed = true;
                }
                
            }
        }
        return false;
    }
}
