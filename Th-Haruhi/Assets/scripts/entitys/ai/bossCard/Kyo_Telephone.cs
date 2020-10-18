using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//boss 符卡2
public class Kyo_Telephone : BossCardBase
{
    public override string CardName => "虚线「如电话线缠在一起般的羁绊」";

    public override float TotalTime => 30f;

    public override Vector3 StartPos => Vector2Fight.NewWorld(0, 110);
   
    protected override void Start()
    {
        base.Start();

        //task wander
        var taskWander = new LuaStgTask(Master, 240, 240, -1, MainTask);
        taskWander.AddWander(60, -96, 96, 112, 144, 16, 32, 8, 16, MovementMode.MOVE_NORMAL, DirectionMode.MOVE_X_TOWARDS_PLAYER);

        //shoot task(800fps loop)
        var shootTask = new LuaStgTask(Master, 0, 800, -1, MainTask);
        shootTask.Execuse = () =>
        {
            var an4 = Random.Range(-120f, 120f);
            var an3 = Random.Range(-15f, 15f);

            //blue line
            var task1 = new LuaStgTask(Master, 0, 2, 75, shootTask);
            task1.SetP("y", 0, 2);
            task1.SetP("an", 0, 15);
            task1.Execuse = () =>
            {
                var task2 = new LuaStgTask(Master, 0, 0, 6, task1);
                task2.SetP("an2", an3, 60);
                task2.SetP("an5", task1.GetP("an"), an4);

                task2.Execuse = () =>
                {
                    var an2 = task2.GetP("an2");
                    var y = task1.GetP("y");
                    var posX = LuaStg.Cos(an2) * y;
                    var posY = LuaStg.Sin(an2) * y;
                    var pos = Master.transform.position + Vector2Fight.NewLocal(posX, posY);

                    var eulurZ = task2.GetP("an5") + an2;

                    ShootBullet(pos, eulurZ, BulletIdBlue);
                };
            };

            var task3 = new LuaStgTask(Master, 0, 2, 6, shootTask);
            task3.SetP("y", 150, 0);
            task3.SetP("an", 0, 20);
            task3.Execuse = () =>
            {
                var task4 = new LuaStgTask(Master, 0, 1, 3, task3);
                task4.SetP("an4", 0, 120);
                task4.Execuse = () =>
                {
                    var task5 = new LuaStgTask(Master, 0, 0, 6, task4);
                    task5.SetP("an2", an3, 60);
                    task5.Execuse = () =>
                    {
                        var an2 = task5.GetP("an2");
                        var y = task3.GetP("y");
                        var posX = LuaStg.Cos(an2) * y;
                        var posY = LuaStg.Sin(an2) * y;
                        var pos = Master.transform.position + Vector2Fight.NewLocal(posX, posY);


                        var p1 = task3.GetP("an");
                        var p2 = an2 + task4.GetP("an4");
                        var eulurZ = p1 + p2;

                        ShootBullet(pos, eulurZ, BulletIdBlue);
                    };
                };
            };


            //wait 200fps
            var taskWait200 = new LuaStgTask(Master, 200, 0, 1, shootTask);

            //red line
            var aa3 = Random.Range(60f, 120f);
            var xxx0 = Random.Range(-60f, 60f);

            var t1 = new LuaStgTask(Master, 0, 2, 80, shootTask);
            t1.SetP("yyy1", 240, -8);
            t1.SetP("cos", 0, 16);
            t1.SetP("an", 0, 16);
            t1.SetP("xxxp", xxx0, LuaStg.RandomSign() * 2);
            t1.Execuse = () =>
            {
                var t2 = new LuaStgTask(Master, 0, 0, 3, t1);
                t2.SetP("an2", 0, aa3);
                t2.SetP("sign", -1, 1);

                t2.Execuse = () =>
                {
                    //BulletIdRed
                    var posX = t1.GetP("xxxp") + 80f * LuaStg.Sin(t1.GetP("cos")) * t2.GetP("sign");
                    var posY = t1.GetP("yyy1");
                    var pos = Master.transform.position + Vector2Fight.NewLocal(posX, posY);

                    var eulurZ = t1.GetP("an") + t2.GetP("an2");
                    ShootBullet(pos, eulurZ, BulletIdRed);
                };
            };

            //wait175
            var taskWait175 = new LuaStgTask(Master, 175, 0, 1, shootTask);

            //狙击
            var t3 = new LuaStgTask(Master, 0, 15, 7, shootTask);
            t3.Execuse =  () =>
            {
                var t4 = new LuaStgTask(Master, 0, 0, 12, t3);
                t4.SetP("ang", 0, 30);
                t4.Execuse = () =>
                {
                    var pos = Master.transform.position;
                    var ang = t4.GetP("ang");
                    var eulurZ = ang + Random.Range(-10f, 10f) + ang;
                    ShootSniper(pos, eulurZ, BulletIdSniper);
                };
            };
        };
    }

    private void ShootSniper(Vector3 pos, float eulurZ, int bulletId)
    {
        //sound 
        Master.PlayShootSound(EShootSound.Tan02 , 0.5f);
    
        var targetX = LuaStg.Cos(eulurZ) * 200;
        var targetY = LuaStg.Sin(eulurZ) * 200;
        var targetPos = pos + Vector2Fight.NewLocal(targetX, targetY);

        List<EventData> e = new List<EventData>
        {
            EventData.Frame_AimToPlayer(120, 5f, -10f, 10f)
        };

        BulletFactory.CreateEnemyBullet(bulletId, MoveData.New(pos), e, onCreate: bullet =>
        {
            bullet.MoveToPos(targetPos, 120, MovementMode.MOVE_DECEL, true);
        });

    }


    private void ShootBullet(Vector3 pos, float eulurZ, int bulletId)
    {
        //sound 
        Master.PlayShootSound(EShootSound.Tan01, 0.5f);

        var moveData = MoveData.New(pos, eulurZ.AngelToForward(), 0f);
        var yysh = 0f;

        List<EventData> e = new List<EventData>
        {
            EventData.NewFrame_Update(150, 3, bullet =>
            {
                bullet.MoveData.Speed = (2f * LuaStg.Sin(yysh));
                bullet.SetForward(eulurZ);
                bullet.RevertBrightness();
                yysh += 2f;
            }, 60)
        };


        BulletFactory.CreateEnemyBullet(bulletId, moveData, e, onCreate: bullet =>
        {
            bullet.SetHighLight();
        });

    }

    public int BulletIdBlue = 1339;
    public int BulletIdRed = 1337;
    public int BulletIdSniper = 1250;
    protected override void InitDifficult(ELevelDifficult diff)
    {
       
    }
}