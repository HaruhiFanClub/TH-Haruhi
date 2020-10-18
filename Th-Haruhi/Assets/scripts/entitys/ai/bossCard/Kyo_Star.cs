using UnityEngine;
using System.Collections.Generic;

//boss 符卡2
public class Kyo_Star : BossCardBase
{
    public override string CardName => "「市井牛郎与奇幻织女星」";

    public override float TotalTime => 30f;

    public override Vector3 StartPos => Boss.BossMidCenter;

    private int _curIdx;
    private bool _inWait = true;
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        
        if (!CanShoot || Master.IsDead) return;

        //每3秒转向一次，每次休息1秒
        _curIdx++;
        if (_inWait)
        {
            if(_curIdx == 80)
            {
                FireBigStar();
                Sound.PlayUiAudioOneShot(108);
            }
            if (_curIdx >= 90)
            {
                _inWait = false;
                _curIdx = 0;
                _shootedRedStarCount = 0;
            }
            return;
        }

        if (_curIdx >= 180)
        {
            _inTurnLeft = !_inTurnLeft;
            _inWait = true;
            _curIdx = 0;
        }

        FireStar(_inTurnLeft);
        FireRedStar();
    }

    private bool _inTurnLeft;

   
    private int BigStarId = 1192;
    private int MiniStartId = 1460;
    private int StarCount = 17;
    private int StarFrame = 8;
    private int StarSpeed = 10;

    private int RedStarId = 1189;
    private int RedStarFrame = 20;
    private int RedStarCount = 9;
    private int RedStarSpeed = 3;
    private float TurnSpeed = 0.5f;


    protected override void InitDifficult(ELevelDifficult diff)
    {
        switch (diff)
        {
            case ELevelDifficult.Easy:
                StarCount = 11;
                RedStarCount = 1;
                TurnSpeed = 0.2f;
                break;
            case ELevelDifficult.Normal:
                StarCount = 13;
                RedStarCount = 5;
                TurnSpeed = 0.4f;
                break;
            case ELevelDifficult.Hard:
                StarCount = 17;
                RedStarCount = 9;
                TurnSpeed = 0.6f;
                break;
            case ELevelDifficult.Lunatic:
            case ELevelDifficult.Extra:
                StarCount = 19;
                RedStarCount = 11;
                TurnSpeed = 0.75f;
                break;
        }
    }


    private float _starAngel;
    private int _shootedRedStarCount = 0;

    private void FireRedStar()
    {
        if (_shootedRedStarCount < RedStarCount)
        {
            if (ShootIdx % RedStarFrame == 0)
            {
                _shootedRedStarCount++;
                Master.PlayShootSound(EShootSound.Tan01);

                var z = _starAngel + _shootedRedStarCount * 60;
                if (z > 360) z -= 360;
                z += Random.Range(-30, 30);

                var shootForward = Quaternion.Euler(0, 0, z) * Master.transform.up;
                var moveData = MoveData.New(Master.transform.position, shootForward, RedStarSpeed);
                BulletFactory.CreateEnemyBullet(RedStarId,  moveData);
            }
        }
    }
    private void FireBigStar()
    {
        for (int i = 0; i < 6; i++)
        {
            var shootForward = Quaternion.Euler(0, 0, _starAngel + i * 60) * Master.transform.up;
            var moveData = MoveData.New(Master.transform.position, shootForward, StarSpeed - 2f);
            BulletFactory.CreateEnemyBullet(BigStarId,moveData);
        }
    }

    private void FireStar(bool bTurnLeft)
    {
        _starAngel += bTurnLeft ? -TurnSpeed : TurnSpeed;

        if (_starAngel > 360)
            _starAngel -= 360;

        //int 
        if (ShootIdx  % StarFrame == 0)
        {
            Master.PlayShootSound(EShootSound.Tan00);

            for (int i = 0; i < 6; i++)
            {
                int mid = (StarCount + 1) / 2;
                for (int j = 1; j <= StarCount; j++)
                {
                    //从最外面渲染到最里面
                    int rowCount = Mathf.CeilToInt((StarCount - j) / 2f);
                    float forZ;
                    float speed = StarSpeed;
                    bool inSide = false;
                    //mid
                    if (j == StarCount)
                    {
                        forZ = _starAngel + i * 60;
                    }
                    else
                    {
                        var isLeft = j % 2 == 0;
                        if(isLeft)
                        {
                            forZ = _starAngel + i * 60 - rowCount * 1.8f;
                            if(bTurnLeft) inSide = true;
                        }
                        else
                        {
                            forZ = _starAngel + i * 60 + rowCount * 1.8f;
                            if (!bTurnLeft) inSide = true;
                        }


                        speed = StarSpeed - rowCount * 0.2f - (rowCount - 1) * 0.1f;
                    }

                    
                    var shootForward =  Quaternion.Euler(0, 0, forZ) * Master.transform.up;
                    var shootPos = Master.transform.position;

                    var delayTurnForward = Quaternion.Euler(0, 0, forZ + (bTurnLeft ? 90f : -90f)) * Master.transform.up;
                    var moveData = MoveData.New(shootPos, shootForward, speed); 

                    List<EventData> eventList = new List<EventData>();

                    var t2 = 10 + (mid - rowCount) * 4;
                    var t = 80 + (mid - rowCount) * 10;
                    if (inSide)
                    {
                        eventList.Add(EventData.NewFrame_ChangeSpeed(t2, speed, -5f, 2f));
                        eventList.Add(EventData.NewFrame_ChangeForward(t, delayTurnForward));
                        eventList.Add(EventData.NewFrame_ChangeSpeed(t, -1, 3f, speed + 3f));
                    }

                    BulletFactory.CreateEnemyBullet(MiniStartId, moveData, eventList);
                }
            }
        }
    }
}