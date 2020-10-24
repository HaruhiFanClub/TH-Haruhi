using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//boss 符卡2
public class Kyo_Telephone : BossCardBase
{
    public override string CardName => "虚线「如电话线缠在一起般的羁绊」";

    public override float TotalTime => 35f;

    public override Vector3 StartPos => new Vector3(0, 110);


    protected override void InitPhase()
    {
        Phase = EBossCardPhase.Two;
    }

    protected override void InitDifficult(ELevelDifficult diff)
    {

    }

    public int BulletIdBlue = 1339;
    public int BulletIdRed = 1337;
    public int BulletIdSniper = 1250;


    protected override void Start()
    {
        base.Start();

        //task wander
        var taskWander = Master.CreateTask();
        taskWander.AddWait(240);
        var repeat = taskWander.AddRepeat(0, 240);
        repeat.AddWander(60, -96, 96, 112, 144, 16, 32, 8, 16, MovementMode.MOVE_NORMAL, DirectionMode.MOVE_X_TOWARDS_PLAYER);


        //shoot task(800fps loop)
        Master.CreateTask().AddRepeat(0, 800, execuse: e1 => 
        {
            var shoot = Master.CreateTask();

            var an4 = Random.Range(-120f, 120f);
            var an3 = Random.Range(-15f, 15f);

            var r1 = shoot.AddRepeat(75, 3, () => TaskParms.New("y", 0, 2f, "an", 0, 15f));
            var r2 = r1.AddRepeat(6, 0, () => TaskParms.New("an2", an3, 60f, "an5", r1.Get("an"), an4), p => 
            {
                var posX = LuaStg.Cos(p.Get("an2")) * p.Get("y");
                var posY = LuaStg.Sin(p.Get("an2")) * p.Get("y");

                ShootBullet(new Vector2(Master.Pos.x + posX, Master.Pos.y + posY), p.Get("an5"), p.Get("an2"), 0, BulletIdBlue);
            });

            shoot.AddRepeat(6, 2, () => TaskParms.New("an", 0, 20, "y", 150, 0)).
                  AddRepeat(3, 1, () => TaskParms.New("an4", 0, 120)).
                  AddRepeat(6, 0, () => TaskParms.New("an2", an3, 60), p =>
                  {
                      var posX = LuaStg.Cos(p.Get("an2")) * p.Get("y");
                      var posY = LuaStg.Sin(p.Get("an2")) * p.Get("y");

                      ShootBullet(new Vector2(Master.Pos.x + posX, Master.Pos.y + posY), p.Get("an"), p.Get("an2") + p.Get("an4"), 0, BulletIdBlue);
                  });


            shoot.AddWait(200);

            var aa3 = Random.Range(60f, 120f);
            var xxx0 = Random.Range(-60f, 60f);

            shoot.AddRepeat(80, 2, () => TaskParms.New("yyy1", 240, -8, "cos", 0, 16, "an", 0, 16, "xxxp", xxx0, LuaStg.RandomSign() * 2)).
                  AddRepeat(3, 0, () => TaskParms.New("an2", 0, aa3, "sign", -1, 1), p =>
                  {
                      //BulletIdRed
                      var posX = p.Get("xxxp") + 80f * LuaStg.Sin(p.Get("cos")) * p.Get("sign");
                      var posY = p.Get("yyy1");

                      
                      ShootBullet(new Vector2(Master.Pos.x + posX, Master.Pos.y + posY), p.Get("an") + p.Get("an2"), 0, 0, BulletIdRed);
                  });

            shoot.AddWait(175);
            shoot.AddRepeat(7, 15).AddRepeat(12, 0, () => TaskParms.New("ang", 0, 30), p =>
            {
                var ang = p.Get("ang");
                var eulurZ = ang + Random.Range(-10f, 10f) + ang;
                ShootSniper(Master.Pos, eulurZ, BulletIdSniper);
            });
        });
    }

    private void ShootSniper(Vector2 pos, float ang, int bulletId)
    {
        //sound 
        Sound.PlayTHSound("tan02", true, 0.2f);

        var targetX = pos.x + LuaStg.Cos(ang) * 200;
        var targetY = pos.y + LuaStg.Sin(ang) * 200;

        LuaStg.ShootEnemyBullet(bulletId, pos.x, pos.y, onCreate: bullet =>
        {
            bullet.SetVelocity(0, ang, false, true);

            var task = bullet.CreateTask();
            task.AddMoveTo(120, targetX, targetY, MovementMode.MOVE_DECEL, true);
            task.AddCustom(() =>
            {
                bullet.SetVelocity(5, Random.Range(-5f, 5f), true, true);
            });
        });
    }

    private void ShootBullet(Vector2 pos, float an, float an2, int t, int bulletId)
    {
        Sound.PlayTHSound("tan01", true, 0.1f);

        LuaStg.ShootEnemyBullet(bulletId, pos.x, pos.y, onCreate: bullet =>
         {
             bullet.Rot = an + an2;
             bullet.SetShaderAdditive();

             var task = bullet.CreateTask();
             task.AddWait(150 + t, () =>
             {
                 bullet.RevertShader();
             });

             task.AddRepeat(60, 3, () => TaskParms.New("yjsh", 0, 2), p =>
             {
                 bullet.SetVelocity(2f * LuaStg.Sin(p.Get("yjsh")), bullet.Rot, false, true);
             });
         }) ;
    }

}