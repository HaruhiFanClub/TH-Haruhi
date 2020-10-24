using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class StageData
{
    //关卡难度
    public ELevelDifficult Difficult;

    //出生坐标
    public Vector2 PlayerBornPos = new Vector3(0, -144f);

    public int GrazeCount = 0;          //擦弹数
    public int DefaultLifeCount = 3;    //默认3条命
    public int TotalScore = 0;          //总分数
    public int PlayerId;
    public int CurLevelId;

    //剩余生命数量
    private int _leftLifeCount = 3;
    public int LeftLifeCount
    {
        get { return _leftLifeCount; }
        set
        {
            _leftLifeCount = value;
            GameEventCenter.Send(GameEvent.LifeCountChanged);
        }
    }

    //训练bossId
    public int ParctiseBossId;
}

public enum ELevelDifficult
{
    Easy,
    Normal,
    Hard,
    Lunatic,
    Extra
}

/// <summary>
/// 关卡数据管理，管理进出关卡，数据存储，例如命数，分数等
/// </summary>
public static class StageMgr
{
    static StageMgr()
    {
        GameEventCenter.AddListener(GameEvent.OnGraze, OnGraze);
        GameEventCenter.AddListener(GameEvent.OnPlayerDead, OnPlayerDead);
    }


    public static StageData Data;

    public static Player MainPlayer { private set; get; }


    //从第一关开始游戏
    public static void StartGame()
    {
        GameSystem.Start(EnterMission());
    }

    //加载关卡
    private static IEnumerator EnterMission()
    {
        //加载Loading
        yield return GameSystem.ShowLoading();
        yield return Yielders.Frame;

        //加载关卡
        yield return StageBase.Load(Data.CurLevelId);
        yield return Yielders.Frame;

        //加载UI
        UiManager.Show<UIBattle>();

        //cacheSound
        yield return Sound.CacheAllBgm();

        //加载角色
        yield return Player.Create(Data.PlayerId, player =>
        {
            player.transform.position = Data.PlayerBornPos;
            MainPlayer = player;
        });

        //cacheBgm
        Sound.CacheAllBgm();

        //关闭loading
        yield return GameSystem.HideLoading();
    }

    //擦弹
    private static void OnGraze(object o)
    {
        Data.GrazeCount++;
    }

    //玩家死亡处理
    private static void OnPlayerDead(object o)
    {
        //生命数-1
        Data.LeftLifeCount--;
        MainPlayer = null;

        if (Data.LeftLifeCount > 0)
        {
            //1秒后复活
            GameSystem.Start(PlayerReborn());
        }
        else
        {
            //无剩余生命数量，弹出结算
            DOVirtual.DelayedCall(1f, () => { UiManager.Show<UIDeadView>(); });
        }
    }

    private static IEnumerator PlayerReborn(float sec = 1.2f)
    {
        yield return new WaitForSeconds(sec);
        yield return Player.Create(Data.PlayerId, p =>
        {
            MainPlayer = p;

            //从屏幕外移动进来
            p.transform.position = new Vector3(0, -240f);
            p.transform.DOMove(Data.PlayerBornPos, 0.5f).SetEase(Ease.Linear);

            //给5秒无敌时间
            p.SetInvincibleTime(5f);
        });
    }

    //续关
    public static void Retry()
    {
        //1秒后复活
        GameSystem.Start(PlayerReborn(0.2f));
        Data.LeftLifeCount = Data.DefaultLifeCount;
    }


    //从头开始
    public static void ReStart()
    {
        Data.CurLevelId = 1;
        Data.TotalScore = 0;
        Data.LeftLifeCount = Data.DefaultLifeCount;
    }

}