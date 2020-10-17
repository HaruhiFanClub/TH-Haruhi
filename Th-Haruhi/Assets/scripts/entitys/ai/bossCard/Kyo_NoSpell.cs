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
            Master.Wander(new Vector2(-96, 96), new Vector2(112, 144), new Vector2(16, 32), new Vector2(8, 16), 1);
        }
        else if (_task2Idx == 180)
        {
            ShootBigBullet(1, Random.Range(0f, 360f));
        }
        else if(_task2Idx == 240)
        {
            _task2Idx = 0;
            Master.Wander(new Vector2(-96, 96), new Vector2(112, 144), new Vector2(16, 32), new Vector2(8, 16), 1);
        }
    }


    private void ShootBigBullet(int sign, float sss)
    {
        Master.PlayShootSound(EShootSound.Tan02);
        for (int i = 0; i < 12; i++)
        {
            var ang = i * 30 * sign;
            var shootForward = Quaternion.Euler(0, 0, ang) * Master.transform.up;
            var moveData = MoveData.New(Master.transform.position, shootForward, 3f.ToLuaStgSpeed());

            List<EventData> eventList = new List<EventData>();
            eventList.Add(EventData.NewFrame_ChangeForward(60, shootForward, sign < 0 ? 
                MoveData.EHelixToward.Left : MoveData.EHelixToward.Right, 1));

            eventList.Add(EventData.NewFrame_ChangeForward(180, null, MoveData.EHelixToward.None));
            eventList.Add(EventData.NewFrame_Destroy(400));

            BulletFactory.CreateBulletShoot(BigBulletId, Master.transform, Layers.EnemyBullet, moveData, eventList, boundDestroy: false, 
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
                            bullet.transform.position = bigBullet.transform.position + Vector2Fight.New(posX, posY) - Vector2Fight.Center;

                            //rotation
                            var eulurZ = angservant2 - 120 * sign;
                            var eulur = bullet.transform.eulerAngles;
                            eulur.z = eulurZ;
                            bullet.transform.eulerAngles = eulur;

                            angservant2 += 1 * sign;
                            wuhu += 0.5f;
                        })
                    };
                    
                    BulletFactory.CreateBulletShoot(RedBulletId, Master.transform, Layers.EnemyBullet, m2, e, boundDestroy: false, onCreate: bullet=>
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
        Master.PlayShootEffect(EColor.BlueLight, 2.5f);

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
            var moveData = MoveData.New(Master.transform.position, shootForward, speed.ToLuaStgSpeed());
            BulletFactory.CreateBulletShoot(BulletId, Master.transform, Layers.EnemyBullet, moveData, eventList, onDestroy: bullet=>
            {
                var pos = bullet.transform.position;
                var fwd = bullet.transform.forward;
                Master.PlayShootEffect(EColor.BlueLight, 1.5f, pos);
                var sf = Quaternion.Euler(0, 0, huhu * turn + turn * 30) * Master.transform.up;
                for (int j = 0; j < 5; j++)
                {
                    var d = MoveData.New(pos, sf, 2f.ToLuaStgSpeed(), 0.2f.ToLuaStgSpeed() * j);
                    BulletFactory.CreateBulletShoot(BulletId, Master.transform, Layers.EnemyBullet, d);
                }
            });

        }
        _curAngel += 12; 
        _sing += 12;
    }

}