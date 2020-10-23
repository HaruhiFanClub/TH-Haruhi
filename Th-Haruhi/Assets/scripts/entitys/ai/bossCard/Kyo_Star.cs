using UnityEngine;
using System.Collections.Generic;

//boss 符卡2
public class Kyo_Star : BossCardBase
{
    public override string CardName => "『诺维科夫所见的宇宙』";

    public override float TotalTime => 35f;

    public override Vector3 StartPos => Boss.BossMidCenter;

    protected override void InitPhase()
    {
        Phase = EBossCardPhase.Two;
    }

    protected override void InitDifficult(ELevelDifficult diff)
    {

    }

    public int WarnStarId = 2003;
    public int HiddenBulletId = 1001;
    public int MiniStarId = 1460;
    public int RedHugeBulletId = 1113;
    public int WeiDaoBulletId = 1244;
    public int LaserId = 1418;
    public int RedStarId = 1189;

 
   protected override void Start()
   {
       base.Start();

        //Boss 移动逻辑
        var taskWander = Master.CreateTask();
        taskWander.AddWait(120);
        taskWander.AddWander(60, -96, 96, 112, 144, 16, 32, 8, 16, MovementMode.MOVE_NORMAL, DirectionMode.MOVE_RANDOM);
        taskWander.AddWait(200);

        var wanderRepeat = taskWander.AddRepeat(0, 240);
        wanderRepeat.AddMoveTo(60, 0, 120, MovementMode.MOVE_NORMAL);
        wanderRepeat.AddWait(240);
        wanderRepeat.AddMoveTo(60, Random.Range(-30f, 30f), 0, MovementMode.MOVE_NORMAL);
        wanderRepeat.AddWait(240);
        wanderRepeat.AddWander(60, -96, 96, 112, 144, 16, 32, 8, 16, MovementMode.MOVE_NORMAL, DirectionMode.MOVE_RANDOM);

        //射击逻辑
        var taskShoot = Master.CreateTask();
        taskShoot.AddWait(60);

        var shootRepeat = taskShoot.AddRepeat(0, 136);

        //星星阵
        ShootStarGroup(shootRepeat, 1);

        shootRepeat.AddWait(36);

        ShootStarGroup(shootRepeat, -1);

        //激光球
        shootRepeat.AddCustom(() => 
        {
            var angle = Master.transform.AnglePlayer();
            ShootRedBall(Master.Pos, angle + 90, 1f);
            ShootRedBall(Master.Pos, angle - 90, 1f);
        });

        float ya = Master.Pos.y;
        float xa = Master.Pos.x;
        float sig = 0f;

        shootRepeat.AddRepeat(20, 20, execuse: a1 =>
        {
            sig = LuaStg.RandomSign();
        })
        .AddRepeat(3, 0, () => TaskParms.New("ang", Random.Range(10f, 20f) * sig, 3f * sig))
        .AddRepeat(4, 0, () => TaskParms.New("xmddd", Random.Range(-60f, -30f) * sig, 20f * sig), p =>
        {
            var x = xa + Random.Range(-20f, 20f);
            var y = ya + Random.Range(-20f, 20f);
            ShootWeidao(x, y, p.Get("ang"), p.Get("xmddd"));
        });

        //red star
        var taskStar = Master.CreateTask();
        taskStar.AddWait(120);
        var repeatStar = taskStar.AddRepeat(0, 273);

        repeatStar.AddRepeat(9, 13).AddRepeat(6, 0, () => TaskParms.New("ang", Random.Range(-30f, 30f), 60f), p =>
        {
            ShootRedStar(Master.Pos, p.Get("ang"));
            Sound.PlayTHSound("tan00", true, 0.1f);
        });

        repeatStar.AddWait(73);

        repeatStar.AddRepeat(9, 13).AddRepeat(6, 0, () => TaskParms.New("ang", Random.Range(-30f, 30f), 60f), p =>
        {
            ShootRedStar(Master.Pos, p.Get("ang"));
            Sound.PlayTHSound("tan00", true, 0.1f);
        });

        repeatStar.AddWait(300);
    }

   private void ShootRedStar(Vector2 pos, float ang)
   {
        LuaStg.ShootEnemyBullet(RedStarId, pos.x, pos.y, bullet => 
        {
            bullet.SetVelocity(4, ang + Random.Range(-15f, 15f), false, true);

            var task = bullet.CreateTask();
            task.AddCustom(() =>
            {
                bullet.SetAcceleration(0.04f, bullet.Rot + 180f, false);

                //set last servant of self

                bullet.SetOmiga(3);
            });

            task.AddWait(100);

            task.AddCustom(() =>
            {
                bullet.SetSmear(8);
                bullet.SetAcceleration(0f, ang + Random.Range(-15f, 15f) + 180f, false);
                Sound.PlayTHSound("enep00", true, 1f);
                bullet.SetVelocity(4, ang + Random.Range(-60f, 60f), false, false);
            });
        });
   }

   private void ShootWeidao(float x, float y, float ang, float xmddd)
   {
        LuaStg.ShootEnemyBullet(WeiDaoBulletId, x, y, onCreate: bullet =>
        {
            bullet.SetHidden();
            var task = bullet.CreateTask();

            var ang3 = bullet.transform.AnglePlayer();
            task.AddRepeat(9, 2, ()=> TaskParms.New("no", 0, 1), p =>
            {
                ShootWeidaoDan(bullet.Pos, ang, xmddd, ang3, p.Get("no"));
            });
            task.AddCustom(() =>
            {
                BulletFactory.DestroyBullet(bullet);
            });
        });
   }

   private void ShootWeidaoDan(Vector2 pos, float ang, float xmddd, float ang2, float number)
   {
        LuaStg.ShootEnemyBullet(WeiDaoBulletId, pos.x, pos.y, bullet =>
        {
            bullet.SetVelocity(3, ang + 180 + ang2, false, true);
            bullet.Navi = true;
            bullet.Renderer.sortingOrder = SortingOrder.EnemyBullet + (int)number;
            bullet.SetAcceleration(0.05F, xmddd + ang2, false);
        });
   }

   private void ShootRedBall(Vector2 pos, float an, float cuxi)
   {
        LuaStg.ShootEnemyBullet(RedHugeBulletId, pos.x, pos.y, onCreate: bullet => 
        {
            bullet.SetVelocity(2f * cuxi, an, false, true);
            bullet.SetAcceleration(0.02f * cuxi, an + 180, false);

            var task = bullet.CreateTask();
            task.AddWait(100);
            task.AddCustom(() =>
            {
                bullet.SetAcceleration(0f, an + 180, false);
            });

            task.AddRepeat(7, 10, () => TaskParms.New("sit", 0, 1), p => 
            {
                ShootLaser(bullet, 1, bullet.Rot, p.Get("sit"));
                ShootLaser(bullet, -1, bullet.Rot, p.Get("sit"));
            });
            task.AddCustom(() =>
            {
                bullet.SetVelocity(0f, 0f, false, true);
            });
            task.AddWait(360, ()=>
            {
                bullet.FadeOut(30);
            });
        });
   }

   private void ShootLaser(EntityBase master, int sign, float ang, float sit)
   {
        LuaStg.ShootLaser(LaserId, 280, 10, 30, master.Pos.x, master.Pos.y, onCreate: bullet =>
        {
            bullet.SetFather(master);
            bullet.SetHighLight();
            bullet.SetVelocity(0f, ang, false, true);

            var task = bullet.CreateTask();
            task.AddRepeat(45, 2, ()=> TaskParms.New("sin2", 0, sign * 2), p =>
            {
                bullet.Rot = ang + LuaStg.Sin(p.Get("sin2")) * (90f - sit * 15f);
            });
       });
   }

   private void ShootStarGroup(TaskRepeat repeat, int sign)
   {
        var an = Random.Range(-30f, 30f);

        //警告星星
        repeat.AddRepeat(6, 0, () => TaskParms.New("an4", an, 60), w1 =>
        {
            var angle = w1.Get("an4");
            LuaStg.ShootEnemyBullet(WarnStarId, Master.Pos.x, Master.Pos.y,  bullet => 
            {
                bullet.SetVelocity(3.5f, angle, false, true);
                bullet.SetSmear(7);
                bullet.SetOmiga(3);
                bullet.SetHighLight();
                bullet.SetBanCollision();
            });
        });

        repeat.AddSoundPlay("heal", 0.5f);
        repeat.AddWait(10);

        //星星集群
        var r1 = repeat.AddRepeat(24, 6, () => TaskParms.New("an2", an, 3));
        r1.AddRepeat(6, 0, () => TaskParms.New("an4", r1.Get("an2"), 60, "ad", -15f, 5f), p =>
        {
            ShootStarHide(Master.Pos, p.Get("an4") * sign);
        });
   }

   private void ShootStarHide(Vector2 pos, float an)
   {
        LuaStg.ShootEnemyBullet(HiddenBulletId, pos.x, pos.y, bullet =>
        {
            bullet.SetVelocity(2f, an, false, true);
            bullet.SetHidden();
            bullet.SetBoundDestroy(false);

            var angle = an;

            var task = bullet.CreateTask();
            task.AddWait(10);
            task.AddRepeat(15, 0, () => TaskParms.New("ang", bullet.Rot - 21f, 3f, "sign", -7f, 1f), p => 
            {
                ShootMiniStar(bullet.Pos, p.Get("ang"), p.Get("sign"));
            });
            task.AddCustom(() =>
            {
                bullet.SetBoundDestroy(true);
            });
        });
   }

   private void ShootMiniStar(Vector2 pos, float ang, float sign)
   {
        LuaStg.ShootEnemyBullet(MiniStarId, pos.x, pos.y, bullet =>
        {
            bullet.SetVelocity(5f - 0.2f * Mathf.Abs(sign), ang, false, true);
            bullet.SetOmiga(2f * sign);
            bullet.SetHighLight();

            var task = bullet.CreateTask();
            task.AddWait(30);

            task.AddRepeat(45, 1, () => TaskParms.New("cos", 0, 2f, "angg", bullet.Rot - 60f * sign, 0), p =>
            {
                bullet.SetVelocity(4f - 0.2f * Mathf.Abs(sign), p.Get("angg") - sign * 15 * LuaStg.Sin(p.Get("cos")), false, false);
            });
        });
   }
}