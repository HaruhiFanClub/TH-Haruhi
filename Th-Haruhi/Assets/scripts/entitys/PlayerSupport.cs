using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//僚机
public class PlayerSupport : MonoBehaviour
{
    public float NextShootTime;

    public PlayerSupportDeploy Deploy { private set; get; }

    public Effect _slowEffect;
    public Effect _fastEffect;

    public void Init(PlayerSupportDeploy deploy, GameObject renderObj)
    {
        Deploy = deploy;

        if(deploy.slowShootEffectId > 0)
        {
            EffectFactory.CreateEffect(deploy.slowShootEffectId, SortingOrder.Effect, obj =>
            {
                _slowEffect = obj;
                _slowEffect.transform.Bind(renderObj.transform);
                _slowEffect.SetActiveSafe(false);

            });
        }

        if (deploy.fastShootEffectId > 0)
        {
            EffectFactory.CreateEffect(deploy.fastShootEffectId, SortingOrder.Effect, obj =>
            {
                _fastEffect = obj;
                _fastEffect.transform.Bind(renderObj.transform);
                _fastEffect.SetActiveSafe(false);

            });
        }
    }

    private bool _prevInLoopShoot;
    private bool _prevInSlow;
    private Bullet _currBullet;
   

    public void UpdateShoot(bool isSlow, int layer, bool inShoot)
    { 
        var shootFrame = isSlow ? Deploy.slowFrame : Deploy.fastFrame;
        var bulletId = isSlow ? Deploy.slowBulletId : Deploy.fastBulletId;

        if(_prevInSlow != isSlow)
        {
            _prevInSlow = isSlow;
            if (_currBullet != null)
            {
                BulletFactory.DestroyBullet(_currBullet);
                _currBullet = null;
            }
            _prevInLoopShoot = false;
            NextShootTime = Time.time + shootFrame * GameSystem.FrameTime;
        }

        _slowEffect?.SetActiveSafe(inShoot && isSlow);
        _fastEffect?.SetActiveSafe(inShoot && !isSlow);

        if (shootFrame > 0)
        {
            //正常类型，持续射击
            if(inShoot)
            {
                if (Time.time > NextShootTime)
                {
                    NextShootTime = Time.time + shootFrame * GameSystem.FrameTime;
                    BulletFactory.CreateBullet(bulletId, transform, layer, bullet =>
                    {
                        bullet.Shoot(transform.position, MathUtility.SwapYZ(transform.forward));
                    });
                }
            }
        }
        else
        {
            //射击间隔为0的，表示激光类型，射击状态下持续显示，非射击销毁
            if(!_prevInLoopShoot && inShoot)
            {
                BulletFactory.CreateBullet(bulletId, transform, layer, bullet =>
                {
                    _currBullet = bullet;
                    bullet.Shoot(transform.position, transform.up);
                });
            }
            if(_prevInLoopShoot && !inShoot)
            {
                if (_currBullet != null)
                {
                    BulletFactory.DestroyBullet(_currBullet);
                    _currBullet = null;
                }
            }
            _prevInLoopShoot = inShoot;
        }
    }
}

