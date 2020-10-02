
using DG.Tweening;
using UnityEngine;

/// <summary>
/// boss
/// 分阶段
/// 有圆圈血条
/// 有特殊UI展示
/// 有特殊立绘
/// </summary>
public class Boss : Enemy
{
    private string BossHpBar = "ui/prefabs/battle/UIBossHpHud.prefab";
    private string BossHpCircle = "ui/prefabs/battle/UIBossCircle.prefab";

    //boss符卡管理器
    private BossCardMgr CardMgr;

    private UIBossHpComponent _bossHpHud;
    private bool _initedHpBar;


    public override void Init(SpriteRenderer renderer, EnemyDeploy deploy)
    {
        base.Init(renderer, deploy);

        MoveToTarget(Vector2Fight.New(0f, 50f), 10f);

        CardMgr = new BossCardMgr();
        CardMgr.Init(this);

        DOVirtual.DelayedCall(1.5F, StartFight);
    }

    private void StartFight()
    {
        //血条
        var bossHpHudObj = ResourceMgr.Instantiate(ResourceMgr.LoadImmediately(BossHpBar));
        bossHpHudObj.transform.SetParent(gameObject.transform, false);
        _bossHpHud = bossHpHudObj.GetComponent<UIBossHpComponent>();
        _bossHpHud.Canvas.sortingOrder = SortingOrder.EnmeyBg;
        _bossHpHud.Canvas.worldCamera = StageCamera2D.Instance.MainCamera;
        _bossHpHud.Bar.fillAmount = 0f;
        _bossHpHud.Bar.DOFillAmount(1f, 1f).onComplete = ()=> { _initedHpBar = true; };

        //boss背景
        var bossCircle = ResourceMgr.Instantiate(ResourceMgr.LoadImmediately(BossHpCircle));
        bossCircle.transform.SetParent(gameObject.transform, false);

        //bossCard
        DOVirtual.DelayedCall(1F, CardMgr.OnStartFight);
    }

    private void UpdateHpHud()
    {
        if (!_initedHpBar) return;
        if (_bossHpHud == null) return;
        _bossHpHud.Bar.fillAmount = (float)HP / HPMax;

    }

    protected override void OnDead()
    {
        GameEventCenter.Send(GameEvent.OnEnemyDie);

        //销毁子弹
        BulletExplosion.Create(transform.position, 0.05f);

        DOVirtual.DelayedCall(0.6f, () =>
        {
            StageCamera2D.Instance?.Shake(0.7f, 1.2f);
            StageCamera3D.Instance?.Shake(0.7f, 1.2f);

            //震屏
            Sound.PlayUiAudioOneShot(105);

            //播放shader特效
            StageCamera2D.Instance.PlayDeadEffect(transform.position);

            Destroy(gameObject);
        });
    }
    protected override void Update()
    {
        base.Update();
        UpdateHpHud();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        CardMgr?.OnFixedUpdate();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        CardMgr?.OnDestroy();
        CardMgr = null;
    }
}

