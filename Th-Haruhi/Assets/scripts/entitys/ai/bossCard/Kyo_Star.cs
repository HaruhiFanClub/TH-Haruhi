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
    public int RedStarId = 1189;

    /*
   protected override void Start()
   {
       base.Start();



       //Boss 移动逻辑
       var taskWander = Master.NewTask(0, 1, 0, MainTask);
       taskWander.AddWait(120);
       taskWander.AddWander(60, -96, 96, 112, 144, 16, 32, 8, 16, MovementMode.MOVE_NORMAL, DirectionMode.MOVE_RANDOM);
       taskWander.AddWait(260);

       taskWander.AddRepeat(0, 240, repeatA =>
       {
           repeatA.AddMoveTo(60, 0, 120, MovementMode.MOVE_NORMAL);
           repeatA.AddWait(240);
           repeatA.AddMoveTo(60, Random.Range(-30f, 30f), 0, MovementMode.MOVE_NORMAL);
           repeatA.AddWait(360);
           repeatA.AddWander(60, -96, 96, 112, 144, 16, 32, 8, 16, MovementMode.MOVE_NORMAL, DirectionMode.MOVE_RANDOM);
       });


       //射击逻辑
       var taskShoot = Master.NewTask(60, 136, 0, MainTask);
       taskShoot.OnRepeat = () =>
       {

           //正星星
           ShootStarGroup(taskShoot, 1);

           //wait 36
           Master.NewTask(36, 0, 1, taskShoot);

           //反星星
           ShootStarGroup(taskShoot, -1);

           //激光球
           var t1 = Master.NewTask(0, 0, 1, taskShoot);
           t1.OnRepeat = () =>
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
           t2.OnRepeat = () =>
           {
               var sig = LuaStg.RandomSign();
               var t3 = Master.NewTask(0, 0, 3, t2);
               t3.SetP("ang", Random.Range(10f, 20f) * sig, 3f * sig);
               t3.OnRepeat = () =>
               {
                   var t4 = Master.NewTask(0, 0, 4, t3);
                   t4.SetP("xmddd", Random.Range(-60f, -30f) * sig, 20f * sig);
                   t4.OnRepeat = () =>
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

       //red star
       var taskStar = Master.NewTask(0, 1, 0, MainTask);
       taskStar.AddWait(120);
       taskStar.AddRepeat(0, 273, son1 =>
       {
           son1.AddRepeat(9, 13, son2 =>
           {
               son2.AddRepeat(6, 0, son3 =>
               {
                   ShootRedStar(Master.transform.position, son3.GetP("ang"));
                   Sound.PlayTHSound("tan00", true, 0.1f);
               },
               TaskParms.New("ang", Random.Range(-30f, 30f), 60f));
           });

           son1.AddWait(73);

           son1.AddRepeat(9, 13, son2 =>
           {
               son2.AddRepeat(6, 0, son3 =>
               {
                   ShootRedStar(Master.transform.position, son3.GetP("ang"));
                   Sound.PlayTHSound("tan00", true, 0.1f);
               },
               TaskParms.New("ang", Random.Range(-30f, 30f), 60f));
           });

           son1.AddWait(300);
       });


   }

   private void ShootRedStar(Vector3 pos, float ang)
   {
       var angel = ang + Random.Range(-15f, 15f);
       var moveData = MoveData.New(pos, angel.AngleToForward(), 4);
       BulletFactory.CreateEnemyBullet(RedStarId, moveData, onCreate: bullet =>
       {
           bullet.SetSelfRota(3);
           //bullet.SetSmear(7);
           bullet.SetAcceleration(0.04f, angel + 180f);

           var t1 = bullet.NewTask(100, 0, 1, bullet.MainTask);
           t1.OnRepeat = () =>
           {
               bullet.RevertAcceleration();
               bullet.MoveData.Speed = 4;
               bullet.SetAngle(ang + Random.Range(-60f, 60f));
               bullet.SetAcceleration(0f, ang + Random.Range(-15f, 15f) + 180f);
               Sound.PlayTHSound("enep00", true, 1f);
           };
       });
   }

   private void ShootWeidao(Vector3 pos, float ang, float xmddd)
   {
       var moveData = MoveData.New(pos);
       BulletFactory.CreateEnemyBullet(WeiDaoBulletId, moveData, onCreate: bullet =>
       {
           bullet.SetHidden();

           var ang3 = bullet.transform.AnglePlayer();

           bullet.MainTask.AddRepeat(9, 2, task1 =>
           {
               ShootWeidaoDan(bullet.transform.position, ang, xmddd, ang3, task1.GetP("number"));
           }, TaskParms.New("number", 0, 1));

           bullet.MainTask.AddCustom(() =>
           {
               BulletFactory.DestroyBullet(bullet);
           });
       });
   }

   private void ShootWeidaoDan(Vector3 pos, float ang, float xmddd, float ang2, float number)
   {
       var startAng = ang + 180 + ang2;
       var moveData = MoveData.New(pos, startAng.AngleToForward(), 3f);
       BulletFactory.CreateEnemyBullet(WeiDaoBulletId, moveData, onCreate: bullet =>
       {
           bullet.Renderer.sortingOrder = SortingOrder.EnemyBullet + (int)number;
           bullet.SetAcceleration(0.05F, xmddd + ang2);
       });
   }

   private void ShootQwq6(float an)
   {
       var pos = Master.transform.position;
       var fwd = (an + 180f).AngleToForward();
       var moveData = MoveData.New(pos, fwd, 2f);
       BulletFactory.CreateEnemyBullet(RedHugeBulletId, moveData, onCreate: bullet =>
       {
           bullet.MainTask.AddWait(100);
           bullet.MainTask.AddCustom(() =>
           {
               bullet.RevertAcceleration();
               bullet.MoveData.Speed = 0f;
               bullet.SetAngle(an + 180);
           });

           bullet.MainTask.AddRepeat(7, 10, taskLaser =>
           {
               var sit = taskLaser.GetP("sit");
               ShootLaser(bullet.transform.position, 1, bullet.Rot, sit);
               ShootLaser(bullet.transform.position, -1, bullet.Rot, sit);

           }, TaskParms.New("sit", 0, 1));

           bullet.MainTask.AddWait(360);
           bullet.MainTask.AddCustom(() =>
           {
               BulletFactory.DestroyBullet(bullet);
           });
       });
   }

   private void ShootLaser(Vector3 pos, int sign, float ang, float sit)
   {
       BulletFactory.CreateEnemyLaser(LaserId, 280, 10, 30, MoveData.New(pos), onCreate: bullet => 
       {
           bullet.SetHighLight();
           bullet.Rot = ang;

           var task = bullet.NewTask(30, 2, 45, bullet.MainTask);
           task.SetP("sin2", 0, sign * 2);
           task.OnRepeat = () =>
           {
               bullet.Rot = ang + LuaStg.Sin(task.GetP("sin2")) * (90f - sit * 15f);
           };

           var task2 = bullet.NewTask(240, 0, 1, bullet.MainTask);
           task2.OnRepeat = () =>
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
       t1.OnRepeat = () =>
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
       t2.OnRepeat = () =>
       {
           var t3 = Master.NewTask(0, 0, 6, t2);
           t3.SetP("an4", t2.GetP("an2"), 60);
           t3.OnRepeat = () =>
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
           t1.SetP("ang", bullet.Rot - 21f, 3);
           t1.SetP("sign", -7f, 1f);

           t1.OnRepeat = () =>
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

           t1.OnRepeat = () =>
           {
               var eulurZ = t1.GetP("angg") - sign * 15f * LuaStg.Sin(t1.GetP("cos"));
               bullet.SetAngle(eulurZ);
               bullet.MoveData.Speed = 4f - 0.2f * Mathf.Abs(sign);
           };
       });
   }
   */
}