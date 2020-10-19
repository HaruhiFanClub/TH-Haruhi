using UnityEngine;
using System.Collections.Generic;

//boss 符卡2
public class Kyo_Star : BossCardBase
{
    public override string CardName => "『诺维科夫所见的宇宙』";

    public override float TotalTime => 303f;

    public override Vector3 StartPos => Boss.BossMidCenter;

    protected override void InitDifficult(ELevelDifficult diff)
    {
    }

    public int WarnStarId = 2003;
    public int HiddenBulletId = 1001;
    public int MiniStarId = 1460;
    public int RedHugeBulletId = 1113;
    public int WeiDaoBulletId = 1244;
    public int LaserId = 1418;

    protected override void Start()
    {
        base.Start();

        //Boss 移动逻辑
        var taskWander = Master.NewTask(0, 1, 0, MainTask);
        taskWander.AddWait(120);
        taskWander.AddWander(60, -96, 96, 112, 144, 16, 32, 8, 16, MovementMode.MOVE_NORMAL, DirectionMode.MOVE_RANDOM);
        taskWander.AddWait(260);

        taskWander.AddRepeat(0, 240, son =>
        {
            taskWander.AddMoveTo(60, 0, 120, MovementMode.MOVE_NORMAL);
            taskWander.AddWait(240);
            taskWander.AddMoveTo(60, Random.Range(-30f, 30f), 0, MovementMode.MOVE_NORMAL);
            taskWander.AddWait(360);
            taskWander.AddWander(60, -96, 96, 112, 144, 16, 32, 8, 16, MovementMode.MOVE_NORMAL, DirectionMode.MOVE_RANDOM);
        });


        //射击逻辑
        var taskShoot = Master.NewTask(60, 500, 0, MainTask);
        taskShoot.Execuse = () =>
        {

            //正星星
            ShootStarGroup(taskShoot, 1);
            
            //wait 36
            Master.NewTask(36, 0, 1, taskShoot);

            //反星星
            ShootStarGroup(taskShoot, -1);

            //激光球
            var t1 = Master.NewTask(0, 0, 1, taskShoot);
            t1.Execuse = () =>
            {
                var angle = Master.transform.AnglePlayer();
                ShootQwq6(angle + 90);
                ShootQwq6(angle - 90);
            };


            //下雨
            var p = Vector2Fight.WorldPosToFightPos(Master.transform.position);
            float ya = p.y;
            float xa = p.x;

            var t2 = Master.NewTask(0, 20, 20, taskShoot);
            t2.Execuse = () =>
            {
                var sig = LuaStg.RandomSign();
                var t3 = Master.NewTask(0, 0, 3, t2);
                t3.OnStart = () =>
                {
                    t3.SetP("ang", Random.Range(10f, 20f) * sig, 3f * sig);
                };
                t3.Execuse = () =>
                {
                    var t4 = Master.NewTask(0, 0, 4, t3);
                    t4.OnStart = () =>
                    {
                        t4.SetP("xmddd", Random.Range(-60f, -30f) * sig, 20f * sig);
                    };
                    t4.Execuse = () =>
                    {
                        var posX = xa + Random.Range(-20f, 20f);
                        var posY = ya + Random.Range(-20f, 20f);
                        ShootWeidao(Vector2Fight.NewLocal(posX, posY), t3.GetP("ang"), t4.GetP("xmddd"));
                    };
                };
            };

            //wait 120
            Master.NewTask(120, 0, 1, taskShoot);
        };


        var taskStar = Master.NewTask(0, 1, 0, MainTask);
        taskStar.AddWait(120);
        taskStar.AddRepeat(0, 273, son1 =>
        {
            son1.AddRepeat(9, 13, son2 =>
            {
                son2.AddRepeat(6, 0, son3 =>
                 {

                 });
            });
        });
    }


    private void ShootWeidao(Vector3 pos, float ang, float xmddd)
    {
        var moveData = MoveData.New(pos);
        BulletFactory.CreateEnemyBullet(WeiDaoBulletId, moveData, onCreate: bullet =>
        {
            bullet.SetHidden();

            var ang3 = bullet.transform.AnglePlayer();

            var task = bullet.NewTask(0, 2, 9, bullet.MainTask);
            task.SetP("number", 0, 1);
            task.Execuse = () =>
            {
                ShootWeidaoDan(bullet.transform.position, ang, xmddd, ang3, task.GetP("number"));
            };
            task.OnEnd = () =>
            {
                BulletFactory.DestroyBullet(bullet);
            };
        });
    }

    private void ShootWeidaoDan(Vector3 pos, float ang, float xmddd, float ang2, float number)
    {
        var startAng = ang + 180 + ang2;
        var moveData = MoveData.New(pos, startAng.AngleToForward(), 3f, 0.2F);
        BulletFactory.CreateEnemyBullet(WeiDaoBulletId, moveData, onCreate: bullet =>
        {
            bullet.Renderer.sortingOrder = SortingOrder.EnemyBullet + (int)number;
            bullet.MoveData.AngleData = new MoveData.AngleAcclerationData
            {
                TargetAngle = xmddd + ang2,
            };
        });
    }

    private void ShootQwq6(float an)
    {
        var pos = Master.transform.position;
        var fwd = (an + 180f).AngleToForward();
        var moveData = MoveData.New(pos, fwd, 2f , -0.65f);
        BulletFactory.CreateEnemyBullet(RedHugeBulletId, moveData, onCreate: bullet =>
        {
            var wait100 = bullet.NewTask(100, 0, 1, bullet.MainTask);
            wait100.OnEnd = () =>
            {
                bullet.MoveData.Acceleration = 0f;
                bullet.MoveData.Speed = 0f;
                bullet.SetForward(an + 180);
            };

            var taskLaser = bullet.NewTask(0, 10, 7, bullet.MainTask);
            taskLaser.OnStart = () =>
            {
                taskLaser.SetP("sit", 0, 1);
            };
            taskLaser.Execuse = () =>
            {
                var sit = taskLaser.GetP("sit");
                ShootLaser(bullet.transform.position, 1, bullet.Rot, sit);
                ShootLaser(bullet.transform.position, -1, bullet.Rot, sit);
            };

            taskLaser.OnEnd = () =>
            {
                var taskDestroy = bullet.NewTask(360, 0, 1, bullet.MainTask);
                taskDestroy.Execuse = () =>
                {
                    BulletFactory.DestroyBullet(bullet);
                };
            };
        });
    }

    private void ShootLaser(Vector3 pos, int sign, float ang, float sit)
    {
        BulletFactory.CreateEnemyLaser(LaserId, 280, 10, 30, MoveData.New(pos), onCreate: bullet => 
        {
            bullet.SetHighLight();
            bullet.Rot = ang;

            var task = bullet.NewTask(30, 2, 45, bullet.MainTask);
            task.OnStart = () =>
            {
                task.SetP("sin2", 0, sign * 2);
            };
            task.Execuse = () =>
            {
                bullet.Rot = ang + LuaStg.Sin(task.GetP("sin2")) * (90f - sit * 15f);
            };

            var task2 = bullet.NewTask(240, 0, 1, bullet.MainTask);
            task2.Execuse = () =>
            {
                bullet.TurnOff(30);
            };
        });
    }

    private void ShootStarGroup(LuaStgTask taskA, int sign)
    {
        var an = Random.Range(-30f, 30f);
        var t1 = Master.NewTask(0, 0, 6, taskA);
        t1.SetP("an4", an, 60);
        t1.Execuse = () =>
        {
            //警告星星
            var fwd = t1.GetP("an4").AngleToForward();
            var moveData = MoveData.New(Master.transform.position, fwd, 3.5f);

            BulletFactory.CreateEnemyBullet(WarnStarId, moveData, onCreate: bullet =>
            {
                bullet.SetSmear(7);
                bullet.SetSelfRota(3);
                bullet.SetHighLight();
            });

            Sound.PlayTHSound("heal");
        };

        //wait 10
        var wait10 = Master.NewTask(10, 0, 1, taskA);

        //星星集群
        var t2 = Master.NewTask(0, 6, 24, taskA);
        t2.SetP("an2", an * sign, 3);
        t2.Execuse = () =>
        {
            var t3 = Master.NewTask(0, 0, 6, t2);
            t3.SetP("an4", t2.GetP("an2"), 60);
            t3.Execuse = () =>
            {
                ShootStarHidePos(t3.GetP("an4") * sign);
            };
        };
    }

    private void ShootStarHidePos(float an)
    {
        var pos = Master.transform.position;
        var fwd = an.AngleToForward();
        var moveData = MoveData.New(pos, fwd, 2f);

        BulletFactory.CreateEnemyBullet(HiddenBulletId, moveData, onCreate: bullet =>
        {
            bullet.SetHidden();
            bullet.SetBoundDestroy(false);

            
            var wait10 = bullet.NewTask(10, 0, 1, bullet.MainTask);

            bullet.SetBoundDestroy(true);

            var angle = an;
            var t1 = bullet.NewTask(0, 0, 15, bullet.MainTask);
            t1.OnStart = () =>
            {
                t1.SetP("ang", bullet.Rot - 21f, 3);
                t1.SetP("sign", -7f, 1f);
            };

            t1.Execuse = () =>
            {
                ShootMiniStar(bullet.transform.position, t1.GetP("ang"), t1.GetP("sign"));
            };
        });
    }

    private void ShootMiniStar(Vector3 pos, float ang, float sign)
    {
        var speed = 5f - 0.2f * Mathf.Abs(sign);
        var moveData = MoveData.New(pos, ang.AngleToForward(), speed);
        BulletFactory.CreateEnemyBullet(MiniStarId, moveData, onCreate: bullet =>
        {
            bullet.SetSelfRota(2f * sign);
            bullet.SetHighLight();

            //wait30
            var wait30 = bullet.NewTask(30, 0, 1, bullet.MainTask);

            var t1 = bullet.NewTask(0, 1, 45, bullet.MainTask);

            t1.SetP("cos", 0, 2);
            t1.SetP("angg", (2f * sign) * 30 + bullet.Rot - 60f * sign, 0);

            t1.Execuse = () =>
            {
                var eulurZ = t1.GetP("angg") - sign * 15f * LuaStg.Sin(t1.GetP("cos"));
                bullet.SetForward(eulurZ);
                bullet.MoveData.Speed = 4f - 0.2f * Mathf.Abs(sign);
            };
        });
    }
}