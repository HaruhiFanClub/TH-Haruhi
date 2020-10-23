using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//僚机
public class PlayerSupport : MonoBehaviour
{
    public int NextShootFrame;

    public PlayerSupportDeploy Deploy { private set; get; }

    private TextureEffect _slowEffect;
    private TextureEffect _fastEffect;
    private Bullet _currBullet;

    private bool _prevInLoopShoot;
    private bool _prevInSlow;

    public void Init(PlayerSupportDeploy deploy, GameObject renderObj)
    {
        Deploy = deploy;

        if(deploy.slowShootEffectId > 0)
        {
            TextureEffectFactroy.CreateEffect(deploy.slowShootEffectId, SortingOrder.Effect, obj =>
            {
                _slowEffect = obj;
                _slowEffect.transform.Bind(renderObj.transform);
                _slowEffect.SetActiveSafe(false);

            });
        }

        if (deploy.fastShootEffectId > 0)
        {
            TextureEffectFactroy.CreateEffect(deploy.fastShootEffectId, SortingOrder.Effect, obj =>
            {
                _fastEffect = obj;
                _fastEffect.transform.Bind(renderObj.transform);
                _fastEffect.SetActiveSafe(false);

            });
        }
    }

    public void FixUpdateShoot(bool isSlow, int layer, bool inShoot)
    { 
        var shootFrame = isSlow ? Deploy.slowFrame : Deploy.fastFrame;
        var bulletId = isSlow ? Deploy.slowBulletId : Deploy.fastBulletId;
        var atk = isSlow ? Deploy.slowAtk : Deploy.fastAtk;
        var bulletSpeed = isSlow ? Deploy.slowSpeed : Deploy.fastSpeed;

        if(_prevInSlow != isSlow)
        {
            _prevInSlow = isSlow;
            if (_currBullet != null)
            {
                BulletFactory.DestroyBullet(_currBullet);
                _currBullet = null;
            }
            _prevInLoopShoot = false;
            NextShootFrame = GameSystem.FixedFrameCount + shootFrame;
        }

        _slowEffect?.SetActiveSafe(inShoot && isSlow);
        _fastEffect?.SetActiveSafe(inShoot && !isSlow);

        if (shootFrame > 0)
        {
            //正常类型，持续射击
            if(inShoot)
            {
                if (GameSystem.FixedFrameCount > NextShootFrame)
                {
                    NextShootFrame = GameSystem.FixedFrameCount + shootFrame;
                    BulletFactory.CreateBullet(bulletId, transform.position, layer, bullet =>
                    {
                        bullet.SetMaster(transform);
                        bullet.SetAtk(atk);
                        bullet.SetVelocity(bulletSpeed, transform.eulerAngles.z, false, true);
                    });
                }
            }
        }
        else
        {
            //射击间隔为0的，表示激光类型，射击状态下持续显示，非射击销毁
            if(!_prevInLoopShoot && inShoot)
            {
                BulletFactory.CreateBullet(bulletId, transform.position, layer,  bullet =>
                {
                    _currBullet = bullet;
                    bullet.SetMaster(transform);
                    bullet.SetAtk(atk);
                    bullet.SetVelocity(bulletSpeed, transform.eulerAngles.z, false, true);
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

    public void Destroy()
    {
        if (_currBullet != null)
        {
            BulletFactory.DestroyBullet(_currBullet);
            _currBullet = null;
        }
        if (_slowEffect != null)
        {
            TextureEffectFactroy.DestroyEffect(_slowEffect);
            _slowEffect = null;
        }
        if (_fastEffect != null)
        {
            TextureEffectFactroy.DestroyEffect(_fastEffect);
            _fastEffect = null;
        }
        Destroy(gameObject);
    }
}

