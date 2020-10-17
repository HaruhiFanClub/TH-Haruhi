using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

//boss 符卡2
public class BossCard2 : BossCardBase
{
    public override string CardName => "有希符「资讯爆炸」";

    public override float TotalTime => 30f;

    //blueBullet
    private int BlueBulletId = 1154;
    private int BlueBulletFrame = 10;
    private float BlueBulletSpeed = 5f;
    private int BlueBulletCount = 8;

 
    //redBullet
    private int RedBulletId = 1134;
    private int RedBulletFrame = 10;
    private int RedBulletCount = 5;
    private float RedBulletSpeed = 4f;


    protected override void InitDifficult(ELevelDifficult diff)
    {
        switch (diff)
        {
            case ELevelDifficult.Easy:
                BlueBulletCount = 4;
                RedBulletCount = 3;
                break;
            case ELevelDifficult.Normal:
                BlueBulletCount = 6;
                RedBulletCount = 4;
                break;
            case ELevelDifficult.Hard:
                BlueBulletCount = 8;
                RedBulletCount = 5;
                break;
            case ELevelDifficult.Lunatic:
            case ELevelDifficult.Extra:
                BlueBulletCount = 12;
                RedBulletCount = 7;
                break;
        }

    }

    private float _lastMoveTime;
    private bool _moveLeft;

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if (!CanShoot || Master.IsDead) return;

        FireBlueBullet();
        FireBulletRed();

        //左右移动
        if (Time.time - _lastMoveTime > Random.Range(8, 15)) 
        {
            _moveLeft = !_moveLeft;

            Master.MoveToTarget(Vector2Fight.New(_moveLeft ? -60f : 60f, 144f), 0.4f);
            _lastMoveTime = Time.time;
        }
    }

    private int _redShootIndex = 1;
    private int _redBulletAngel;
    private void FireBulletRed()
    {
        if (ShootIdx % RedBulletFrame == 0)
        {
          
            var fwd = Quaternion.Euler(0, 0, _redBulletAngel) * Master.transform.up;
            var shootPos = Master.transform.position + fwd.normalized * 3f;

            Master.PlayShootSound(EShootSound.Noraml);
            Master.PlayShootEffect(EColor.Red, 2f, shootPos);


            //2组子弹，往反方向
            for (int i = 0; i < RedBulletCount; i++)
            {
                var f1 = Quaternion.Euler(0, 0, _redBulletAngel + 120 + i * 5) * Master.transform.up;
                var moveData = MoveData.New(shootPos, f1, RedBulletSpeed - i * 0.1f);
                BulletFactory.CreateBulletShoot(RedBulletId, Master.transform, Layers.EnemyBullet, moveData);
            }

            for (int i = 0; i < RedBulletCount; i++)
            {
                var f1 = Quaternion.Euler(0, 0, _redBulletAngel - 120 - i * 5) * Master.transform.up;
                var moveData = MoveData.New(shootPos, f1, RedBulletSpeed - i * 0.1f);
                BulletFactory.CreateBulletShoot(RedBulletId, Master.transform, Layers.EnemyBullet, moveData);
            }

            _redShootIndex++;

            if (_redShootIndex % Random.Range(4, 6) == 0)
                _redBulletAngel += 120;
            else
                _redBulletAngel += 60;


            if (_redBulletAngel > 360)
                _redBulletAngel -= 360;
        }
    }

    private int _blueBulletAngel;
    private int _blueShootIndex = 1;
    private void FireBlueBullet()
    {
        //蓝色子弹
        if (ShootIdx % BlueBulletFrame == 0)
        {
            Master.PlayShootSound(EShootSound.Tan00);

            //6发换角度
            for (int i = 0; i < BlueBulletCount; i++)
            {
                var f1 = Quaternion.Euler(0, 0, _blueBulletAngel + i * 10) * Master.transform.up;
                var data = MoveData.New(Master.transform.position, f1, BlueBulletSpeed);
                BulletFactory.CreateBulletShoot(BlueBulletId, Master.transform, Layers.EnemyBullet, data);
            }
            _blueShootIndex++;

            Master.PlayShootEffect(EColor.BlueLight, 2f);

            if(_blueShootIndex % Random.Range(4, 6) == 0) 
                _blueBulletAngel += Random.Range(90, 110);
            else
                _blueBulletAngel += Random.Range(25, 35);


            if (_blueBulletAngel > 360)
                _blueBulletAngel -= 360;
        }
    }
}