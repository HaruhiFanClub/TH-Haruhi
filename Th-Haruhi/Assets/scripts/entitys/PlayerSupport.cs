using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//僚机
public class PlayerSupport : MonoBehaviour
{
    public float NextShootTime;

    public PlayerSupportDeploy Deploy { private set; get; }

    public void Init(PlayerSupportDeploy deploy)
    {
        Deploy = deploy;
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
        }


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
                    //Sound.PlayUiAudioOneShot(Deploy.shootSound);
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
                    bullet.Shoot(transform.position, MathUtility.SwapYZ(transform.forward), true);
                });
            }
            if(_prevInLoopShoot && !inShoot)
            {
                if(_currBullet != null)
                {
                    BulletFactory.DestroyBullet(_currBullet);
                    _currBullet = null;
                }
            }
            _prevInLoopShoot = inShoot;
        }
    }
}

