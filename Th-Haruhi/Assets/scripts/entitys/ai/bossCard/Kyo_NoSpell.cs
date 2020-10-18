using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//boss 符卡2
public class Kyo_NoSpell: BossCardBase
{
    public override string CardName => "Kyo_NoSpell";

    public override float TotalTime => 30f;

    public override Vector3 StartPos => Boss.BossUpCenter;

    
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        
        if (!CanShoot || Master.IsDead) return;

        TaskBullet1();
        Task2();
    }

    public int BulletId = 1008;
    public int BulletSpeed = 8;
    public int BulletInterval = 2;
    public int BigBulletId = 2001;
    private int RedBulletId = 2002;

    protected override void InitDifficult(ELevelDifficult diff)
    {

    }

    private int _task2Idx;
    private void Task2()
    {
        _task2Idx++;

        if(_task2Idx == 60)
        {
            ShootBigBullet(-1, Random.Range(0f, 360f));
        }
        else if(_task2Idx == 120)
        {
            
            Master.MoveToPlayer(60, Vector2Fight.NewLocal(-96, 96), Vector2Fight.NewLocal(112, 144), Vector2Fight.NewLocal(16, 32), Vector2Fight.NewLocal(8, 16), MovementMode.MOVE_NORMAL, DirectionMode.MOVE_X_TOWARDS_PLAYER);
        }
        else if (_task2Idx == 180)
        {
            ShootBigBullet(1, Random.Range(0f, 360f));
        }
        else if(_task2Idx == 240)
        {
            _task2Idx = 0;
            Master.MoveToPlayer(60, Vector2Fight.NewLocal(-96, 96), Vector2Fight.NewLocal(112, 144), Vector2Fight.NewLocal(16, 32), Vector2Fight.NewLocal(8, 16), MovementMode.MOVE_NORMAL, DirectionMode.MOVE_X_TOWARDS_PLAYER);
        }
    }


    private void ShootBigBullet(int sign, float sss)
    {
        Master.PlayShootSound(EShootSound.Tan02);
        for (int i = 0; i < 12; i++)
        {
            var ang = i * 30 * sign;
            var shootForward = Quaternion.Euler(0, 0, ang) * Master.transform.up;
            var moveData = MoveData.New(Master.transform.position, shootForward, 3f);

            List<EventData> eventList = new List<EventData>();
            eventList.Add(EventData.NewFrame_ChangeForward(60, shootForward, sign < 0 ? 
                MoveData.EHelixToward.Left : MoveData.EHelixToward.Right, 1));

            eventList.Add(EventData.NewFrame_ChangeForward(180, null, MoveData.EHelixToward.None));
            eventList.Add(EventData.NewFrame_Destroy(400));

            BulletFactory.CreateEnemyBullet(BigBulletId,  moveData, eventList, boundDestroy: false, 
            onCreate : bigBullet =>
            {
                //环绕子弹（红）
                var angServent = 0;
                for (int j = 0; j < 10; j++)
                {
                    var m2 = MoveData.New(Master.transform.position, Vector3.zero, 0f);
                    var angservant2 = angServent * sign;
                    var wuhu = sss;
                    List<EventData> e = new List<EventData>
                    {
                        EventData.NewFrame_Update(0, 1, bullet =>
                        {
                            //position
                            var posX = LuaStg.Cos(angservant2) * 40 * LuaStg.Sin(wuhu);
                            var posY = LuaStg.Sin(angservant2) * 40 * LuaStg.Cos(wuhu);
                            bullet.transform.position = bigBullet.transform.position + Vector2Fight.NewWorld(posX, posY) - Vector2Fight.Center;

                            //rotation
                            var eulurZ = angservant2 - 120 * sign;
                            var eulur = bullet.transform.eulerAngles;
                            eulur.z = eulurZ;
                            bullet.transform.eulerAngles = eulur;

                            angservant2 += 1 * sign;
                            wuhu += 0.5f;
                        })
                    };
                    
                    BulletFactory.CreateEnemyBullet(RedBulletId, m2, e, boundDestroy: false, onCreate: bullet=>
                    {
                        bigBullet.SonBullets.Add(bullet);
                    });


                    angServent += sign * 36;
                }
            });
        }
    }

    private int _curIdx;
    private int _curAngel = 0;
    private bool _inTurnLeft;
    private float _sing;
    private void TaskBullet1()
    {
        if (_curIdx == 0)
        {
            _curAngel = 0;
            _inTurnLeft = !_inTurnLeft;
            _sing = Random.Range(0f, 120f);

        }
        if (_curIdx <= 180)
        {
            if (_curIdx % BulletInterval == 0)
            {
                FireSmallBullet();
            }
        }

        _curIdx++;
        if (_curIdx > 240)
        {
            _curIdx = 0;
        }
    }

    private void FireSmallBullet()
    {

        var sin = LuaStg.Sin(Mathf.Abs(_curAngel));
        var turn = _inTurnLeft ? 1f : -1f;

        for (int i = 0; i < 3; i++)
        {
            var ang = i * 120f;
            var huhu = (_sing + ang - sin * 10);
            var shootForward = Quaternion.Euler(0, 0, huhu * turn) * Master.transform.up;

            List<EventData> eventList = new List<EventData>();
            eventList.Add(EventData.NewFrame_Destroy(40));

            var speed = 2.5f + sin;
            var moveData = MoveData.New(Master.transform.position, shootForward, speed);
            BulletFactory.CreateEnemyBullet(BulletId, moveData, eventList, onDestroy: bullet=>
            {
                var pos = bullet.transform.position;
                var fwd = bullet.transform.forward;
                var sf = Quaternion.Euler(0, 0, huhu * turn + turn * 30) * Master.transform.up;
                for (int j = 0; j < 5; j++)
                {
                    var d = MoveData.New(pos, sf, 2f, 0.2f * j);
                    BulletFactory.CreateEnemyBullet(BulletId, d);
                }
            });

        }
        _curAngel += 12; 
        _sing += 12;
    }

}