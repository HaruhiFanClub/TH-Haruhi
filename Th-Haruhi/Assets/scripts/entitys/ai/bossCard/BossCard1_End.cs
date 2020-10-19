using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

//boss 符卡1
public class BossCard1_End : BossCardBase
{
    public override string CardName => "有希符「统合思念体」";
    public override float TotalTime => 30f;

    private bool _isBuletBulletState = true;
    private const int SwitchFrame = 400;

    //blueBullet
    private int BlueBulletId = 1244;
    private int BlueBulletFrame = 6;
    private float BlueBulletSpeed = 3f;
    private int BlueBulletCount = 3;

    //laser
    private int LaserBulletId = 1455;
    private int LaserFrame = 10;
    private float LaserFastSpeed = 10;
    private float LaserSlowSpeed = 1;

    //redBullet
    private int RedBulletId = 1238;
    private int RedBulletFrame = 3;
    private int RedBulletCount = 4;
    private float RedBulletSpeed = 8f;

    protected override void InitDifficult(ELevelDifficult diff)
    {
        switch (diff)
        {
            case ELevelDifficult.Easy:
                BlueBulletCount = 1;
                LaserFrame = 20;
                RedBulletSpeed = 6f;
                RedBulletFrame = 8;
                break;
            case ELevelDifficult.Normal:
                BlueBulletCount = 2;
                LaserFrame = 15;
                RedBulletSpeed = 6f;
                RedBulletFrame = 8;
                break;
            case ELevelDifficult.Hard:
                BlueBulletCount = 3;
                LaserFrame = 10;
                RedBulletSpeed = 8f;
                RedBulletFrame = 4;
                break;
            case ELevelDifficult.Lunatic:
            case ELevelDifficult.Extra:
                BlueBulletCount = 4;
                LaserFrame = 7;
                RedBulletSpeed = 8f;
                RedBulletFrame = 2;
                break;
        }
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if (!CanShoot || Master.IsDead) return;

        if (ShootIdx >= SwitchFrame && ShootIdx % SwitchFrame == 0)
        {
            _isBuletBulletState = !_isBuletBulletState;
        }

        FireBlueBullet();
        FireLaser();
        FireBulletRed();
    }

    private void FireBulletRed()
    {
        if (ShootIdx % RedBulletFrame == 0)
        {
            Sound.PlayTHSound("plst00", true, 0.2f);

            for(int i = 0; i < RedBulletCount; i++)
            {
                var fwd = Quaternion.Euler(0, 0, i * (360f / RedBulletCount) + Random.Range(-30f, 30f)) * Master.transform.up;
                var moveData = MoveData.New(Master.transform.position, fwd, RedBulletSpeed, -1, RedBulletSpeed - 2);
                BulletFactory.CreateEnemyBullet(RedBulletId, moveData);
            }
        }
    }

    private int _laserAngle;
    private void FireLaser()
    {
        if (ShootIdx % LaserFrame == 0)
        {
            var fwd = Quaternion.Euler(0, 0, _laserAngle) * Master.transform.up;
            var moveData = MoveData.New(Master.transform.position, fwd, LaserFastSpeed);

            List<EventData> eventList = new List<EventData>();
            eventList.Add(EventData.NewFrame_ChangeSpeed(20, LaserSlowSpeed));
            eventList.Add(EventData.NewFrame_ChangeSpeed(48, LaserSlowSpeed, 4, LaserFastSpeed));

            BulletFactory.CreateEnemyBullet(LaserBulletId,  moveData, eventList);

            _laserAngle += Random.Range(45, 75);
            if (_laserAngle > 360) _laserAngle -= 360;
        }
    }

    private int _blueBulletAngle;
    private void FireBlueBullet()
    {
        //蓝色子弹
        if (ShootIdx % BlueBulletFrame == 0)
        {
            Sound.PlayTHSound("tan00", true, 0.5f);

            for (int i = 0; i < BlueBulletCount; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var f1 = Quaternion.Euler(0, 0, _blueBulletAngle + i * -12 + j * 120) * Master.transform.up;
                    var data = MoveData.New(Master.transform.position, f1, BlueBulletSpeed + i * 0.05f, -0.2f, BlueBulletSpeed - 1f);
                    BulletFactory.CreateEnemyBullet(BlueBulletId, data);
                }
            }

            
            _blueBulletAngle += 15;
            if (_blueBulletAngle > 360)
                _blueBulletAngle -= 360;
        }
    }
}