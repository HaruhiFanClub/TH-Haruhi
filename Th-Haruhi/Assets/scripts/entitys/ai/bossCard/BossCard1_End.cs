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
    }

}