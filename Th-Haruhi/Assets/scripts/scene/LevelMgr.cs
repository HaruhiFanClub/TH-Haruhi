using DG.Tweening;
using System.Collections;
using UnityEngine;

public class LevelData
{
    //关卡难度
    public ELevelDifficult Difficult;

    //出生坐标
    public Vector2 PlayerBornPos = Vector2Fight.New(0, -80f);

    public int MaxLifeCount = 3;        //总生命数量
    public int LeftLifeCount = 3;       //剩余生命数量
    public int TotalScore = 0;          //总分数
    public int PlayerId;
    public int CurLevelId;
}

public enum ELevelDifficult
{
    Easy,
    Normal,
    Hard,
    Lunatic
}

/// <summary>
/// 关卡数据管理，管理进出关卡，数据存储，例如命数，分数等
/// </summary>
public static class LevelMgr
{
    static LevelMgr()
    {
        GameEventCenter.AddListener(GameEvent.OnPlayerDead, OnPlayerDead);
        GameEventCenter.AddListener(GameEvent.OnEnemyDie, OnEnemyDie);
    }

    public static LevelData Data;

    //从第一关开始游戏
    public static void StartNewGame(int playerId)
    {
        int levelId = 1;

        //初始化数据
        Data.PlayerId = playerId;
        Data.CurLevelId = levelId;

        GameSystem.CoroutineStart(EnterMission());
    }

    //加载关卡
    private static IEnumerator EnterMission()
    {
        //加载Loading
        yield return UILoading.YieldShow();
        yield return Yielders.Frame;

        //加载关卡
        yield return Level.Load(Data.CurLevelId);
        yield return Yielders.Frame;

        //加载角色
        yield return Player.Create(Data.PlayerId, player =>
        {
            player.transform.position = Data.PlayerBornPos;
        });

        //关闭loading
        UILoading.Close();
    }


    //玩家死亡处理
    private static void OnPlayerDead(object o)
    {
        //生命数-1
        Data.LeftLifeCount--;

        if (Data.LeftLifeCount > 0)
        {
            //1秒后复活
            GameSystem.CoroutineStart(PlayerReborn());
        }
        else
        {
            //无剩余生命数量，弹出结算
        }
    }

    //测试用
    private static void OnEnemyDie(object o)
    {
        GameSystem.CoroutineStart(TestCreateEnemy());
    }

    private static IEnumerator TestCreateEnemy()
    {
        yield return new WaitForSeconds(3f);
        var enemyId = UnityEngine.Random.Range(1, 10);
        yield return Enemy.Create(enemyId);
    }

    private static IEnumerator PlayerReborn()
    {
        yield return new WaitForSeconds(1f);
        yield return Player.Create(Data.PlayerId, p =>
        {
            //从屏幕外移动进来
            p.transform.position = Vector2Fight.New(0, -110);
            p.transform.DOMove(Data.PlayerBornPos, 0.5f).SetEase(Ease.Linear);

            //给5秒无敌时间
            p.SetInvincibleTime(5f);
        });
    }

}