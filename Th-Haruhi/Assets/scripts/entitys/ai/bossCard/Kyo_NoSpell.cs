using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//boss 符卡2
public class Kyo_NoSpell: BossCardBase
{
    public override string CardName => "Kyo_NoSpell";

    public override float TotalTime => 30;

    public override Vector3 StartPos => Boss.BossUpCenter;


    public int BulletId = 1008;
    public int BigBulletId = 2001;
    public int RedBulletId = 2002;

    protected override void InitDifficult(ELevelDifficult diff)
    {

    }

    protected override void Start()
    {
        base.Start();

        var task1 = Master.CreateTask();
        var r1 = task1.AddRepeat(0, 60);
        DoTask1(r1, 1);
        DoTask1(r1, -1);

        var x = Master.CreateTask();
        x.AddRepeat(0, 1, execuse: task2 =>
        {
            task2.AddWait(60);
            task2.AddWander(60, -96, 96, 112, 144, 16, 32, 8, 16, MovementMode.MOVE_NORMAL, DirectionMode.MOVE_X_TOWARDS_PLAYER);
            task2.AddRepeat(12, 0, () => TaskParms.New("ang", 0, 30), p =>
            {
                BigBallBullet(Master.Pos, p.Get("ang"), -1);
            });

            task2.AddWait(150);
            task2.AddWander(60, -96, 96, 112, 144, 16, 32, 8, 16, MovementMode.MOVE_NORMAL, DirectionMode.MOVE_X_TOWARDS_PLAYER);
            task2.AddRepeat(12, 0, () => TaskParms.New("ang", 0, 30), p =>
            {
                BigBallBullet(Master.Pos, p.Get("ang"), 1);
            });
        });
        
    }

    private void DoTask1(TaskRepeat r1, float sign)
    {
        r1.AddRepeat(1, 60).
        AddRepeat(90, 2, () => TaskParms.New("sing", Random.Range(0f, 120f), 12, "ain", 0, 12)).
        AddRepeat(3, 0, () => TaskParms.New("ANG", 0, 120),  p =>
        {
            var haha = 2.5f + LuaStg.Sin(p.Get("ain")) * 1;
            var huhu = p.Get("sing") + p.Get("ANG") + LuaStg.Sin(p.Get("ain")) * 10;
            MRY(Master.Pos, haha, huhu, sign);
        });
    }

    private void MRY(Vector2 pos, float HAHA, float HUHU, float sign)
    {
        LuaStg.ShootBullet(BulletId, pos.x, pos.y,  shootEffectScale: 1.6f, onCreate: bullet =>
        {
            bullet.SetVelocity(HAHA, HUHU * sign, false, true);

            var task = bullet.CreateTask();

            task.AddWait(40);
            task.AddRepeat(5, 0, () => TaskParms.New("VE", 2, 0.2F), p =>
            {
                var angle = HUHU * sign + sign * 30;
                BulletArrowBig(bullet.Pos, angle, p.Get("VE"));
            });

            task.AddCustom(() =>
            {
                bullet.PlayEffectAndDestroy(502);
            });
        });
    }

    private void BulletArrowBig(Vector2 pos, float angle, float speed)
    {
        LuaStg.ShootBullet(BulletId, pos.x, pos.y, bullet =>
        {
            bullet.SetVelocity(speed, angle, false, true);
        });
    }

    private void BigBallBullet(Vector2 pos, float ang, float sign)
    {
        Sound.PlayTHSound("tan02", true, 1f);

        LuaStg.ShootBullet(BigBulletId, pos.x, pos.y, onCreate: bullet =>
        {
            bullet.SetVelocity(3, ang, true, true);
            bullet.SetBoundDestroy(false);

            var taskRot = bullet.CreateTask();
            taskRot.AddWait(60);
            taskRot.AddRepeat(120, 1, execuse : p =>
            {
                var newAngle = sign * 1 + bullet.Rot;
                bullet.SetVelocity(2F, newAngle, false, true);
            });

            var taskBullet = bullet.CreateTask();
            var randomAngle = Random.Range(0f, 360f);
            taskBullet.AddRepeat(10, 1, () => TaskParms.New("angservant", 0, 36 * sign),  p =>
            {
                AroundBallBullet(p.Get("angservant"), sign, randomAngle, bullet);
            });

            var taskDestroy = bullet.CreateTask();
            taskDestroy.AddWait(500);
            taskDestroy.AddCustom(() =>
            {
                BulletFactory.DestroyBullet(bullet);
            });
        });
    }

    private void AroundBallBullet(float angservant, float sign, float randomAngle, Bullet father)
    {
        LuaStg.ShootBullet(RedBulletId, father.Pos.x, father.Pos.y, shootEffectScale: 0f, onCreate: bullet =>
        {
            bullet.SetFather(father);
            bullet.SetHighLight();
            bullet.SetBoundDestroy(false);

            var task = bullet.CreateTask();
            task.AddRepeat(0, 1, () => TaskParms.New("angservant2", angservant * sign, 1 * sign, "wuhu", randomAngle, 0.5f), p =>
            {
                var posX = LuaStg.Cos(p.Get("angservant2")) * 40 * LuaStg.Sin(p.Get("wuhu"));
                var posY = LuaStg.Sin(p.Get("angservant2")) * 40 * LuaStg.Cos(p.Get("wuhu"));
                var rot = p.Get("angservant2") - 120 * sign;

                bullet.SetRelativePosition(posX, posY, rot);
            });
        });
     }
}