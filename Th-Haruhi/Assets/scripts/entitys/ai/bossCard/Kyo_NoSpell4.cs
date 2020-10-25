using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//boss 符卡2
public class Kyo_NoSpell4 : BossCardBase
{
    public override string CardName => "Kyo_NoSpell4";

    public override float TotalTime => 30;

    public override Vector3 StartPos => Boss.BossUpCenter;

    protected override void InitPhase()
    {
        Phase = EBossCardPhase.One;
    }

    public int BulletId = 1008;
    public int BigBulletId = 2001;
    public int RedBulletId = 2002;

    protected override void InitDifficult(ELevelDifficult diff)
    {

    }

    protected override void Start()
    {
        base.Start();

        var sign = 2f;
        var sign2 = 1f;

        var task1 = Master.CreateTask();
        var repeat1 = task1.AddRepeat(0, 60, execuse: p =>
        {
            sign2 = -sign2;
        });
        repeat1.SetVariable("an1", LuaStg.AnglePlayer(Master.transform));

        repeat1.AddRepeat(3, 0, execuse: p =>
        {
            sign = -sign;

        }).AddRepeat(15, 6, execuse: p =>
         {
             repeat1.SetVariable("an1", sign + repeat1.GetVariable("an1"));

         }).AddRepeat(12, 0, () => TaskParms.New("an2", 0, 30), par =>
         {
             var HAHA = 3F;
             var HUHU = sign2 + (repeat1.GetVariable("an1") + par.Get("an2")) + Random.Range(-3f, 3f);
             MRY2(Master.Pos, HAHA, HUHU);
         });


        var task2 = Master.CreateTask().AddRepeat(0, 0);
        task2.AddWait(90);
        task2.AddWander(60, -96, 96, 112, 144, 16, 32, 8, 16, MovementMode.MOVE_NORMAL, DirectionMode.MOVE_X_TOWARDS_PLAYER);
        
        
        task2.AddRepeat(15, 0, () => TaskParms.New("ang", 0, 24), p =>
        {
            BigBallBullet(Master.Pos, p.Get("ang"), -1);
        });

        task2.AddWait(90);
        task2.AddWander(60, -96, 96, 112, 144, 16, 32, 8, 16, MovementMode.MOVE_NORMAL, DirectionMode.MOVE_X_TOWARDS_PLAYER);
        
        task2.AddRepeat(15, 0, () => TaskParms.New("ang", 0, 24), p =>
        {
            BigBallBullet(Master.Pos, p.Get("ang"), 1);
        });
    }

    private void MRY2(Vector2 pos, float HAHA, float HUHU)
    {
        LuaStg.ShootEnemyBullet(BulletId, pos.x, pos.y,  shootEffectScale: 1.6f, onCreate: bullet =>
        {
            bullet.SetVelocity(HAHA, HUHU, false, true);
           
            var task = bullet.CreateTask();

            task.AddWait(40, ()=>
            {
                bullet.SetShaderAdditive();
            });
            task.AddRepeat(120, 1, () => TaskParms.New("VE", 1, 0.02F), p =>
            {
                bullet.SetVelocity(p.Get("VE"), HUHU, false, true);
            });
        });
    }

    private void BigBallBullet(Vector2 pos, float ang, float sign)
    {
        Sound.PlayTHSound("tan02", true, 1f);

        LuaStg.ShootEnemyBullet(BigBulletId, pos.x, pos.y, onCreate: bullet =>
        {
            bullet.SetVelocity(3, ang + bullet.Rot, true, true);
            bullet.SetBoundDestroy(false);
            bullet.SetShaderAdditive();
            var taskDestroy = bullet.CreateTask();
            taskDestroy.AddWait(350);
            taskDestroy.AddCustom(() =>
            {
                BulletFactory.DestroyBullet(bullet);
            });

            var taskRot = bullet.CreateTask();
            taskRot.AddWait(60);
            taskRot.AddRepeat(50, 1, () => TaskParms.New("VE", 0, 0.03F), p =>
            {
                bullet.SetVelocity(3f- p.Get("VE"), bullet.Rot, false, true);
            });

            var SSS3 = Random.Range(0F, 30F);
            var SSS = Random.Range(0F, 360F);
            var taskBullet = bullet.CreateTask();


            //shoot son bullet
            var rr1 = taskBullet.AddRepeat(3, 0, () => TaskParms.New("ang", 0, 120));
            rr1.AddRepeat(7, 0, () => TaskParms.New("FLLOW", rr1.Get("ang"), 6), p =>
            {
                AroundBallBullet(p.Get("FLLOW"), sign, SSS, SSS3, bullet);
            });

        });
    }

    private void AroundBallBullet(float FLLOW, float sign, float SSS, float SSS3, Bullet father)
    {
        LuaStg.ShootEnemyBullet(RedBulletId, father.Pos.x, father.Pos.y, shootEffectScale: 0f, onCreate: bullet =>
        {
            bullet.SetFather(father);
            bullet.SetShaderAdditive();
            bullet.SetBoundDestroy(false);

            var task = bullet.CreateTask();
            task.AddRepeat(0, 1, () => TaskParms.New("wuhu", SSS - FLLOW * 2F, 0.5f), p =>
            {
                var wuhu = p.Get("wuhu");
                
                var posX = 60f * (LuaStg.Sin(wuhu * sign) * LuaStg.Cos(SSS3) - LuaStg.Cos(wuhu * sign) * LuaStg.Sin(SSS3));
                var posY = 60f * (LuaStg.Cos(wuhu * sign) * LuaStg.Cos(SSS3) + LuaStg.Sin(wuhu * sign) * LuaStg.Sin(SSS3));

                bullet.SetRelativePosition(posX, posY, 0);
            }) ;
        });
     }
}