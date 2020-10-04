
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

    //上方中心位置
    public static Vector3 BossUpCenter = Vector2Fight.New(0f, 50f);

    //中心位置
    public static Vector3 BossMidCenter = Vector2Fight.New(0f, 0f);

    //boss符卡管理器
    private BossCardMgr CardMgr;

    private UIBossHpComponent _bossHpHud;
    private UIBossCircleComponent _bossCircle;
    private bool _initedHpBar;


    public override void Init(SpriteRenderer renderer, EnemyDeploy deploy)
    {
        base.Init(renderer, deploy);

        MoveToTarget(BossUpCenter, 10f);

        CardMgr = new BossCardMgr();
        CardMgr.Init(this, HPMax);

        Invisible = true;

        DOVirtual.DelayedCall(0.5F, StartFight, false);
    }

    private void StartFight()
    {
        Sound.PlayUiAudioOneShot(107);

        //血条
        var bossHpHudObj = ResourceMgr.Instantiate(ResourceMgr.LoadImmediately(BossHpBar));
        bossHpHudObj.transform.SetParent(gameObject.transform, false);
        _bossHpHud = bossHpHudObj.GetComponent<UIBossHpComponent>();
        _bossHpHud.Canvas.sortingOrder = SortingOrder.EnemyBullet + 1;
        _bossHpHud.Canvas.worldCamera = StageCamera2D.Instance.MainCamera;

        //boss背景
        var circle = ResourceMgr.Instantiate(ResourceMgr.LoadImmediately(BossHpCircle));
        circle.transform.SetParent(gameObject.transform, false);
        _bossCircle = circle.GetComponent<UIBossCircleComponent>();

        //bossCard
        DOVirtual.DelayedCall(1F, CardMgr.OnStartFight, false);

        //禁止无敌
        Invisible = false;
    }

    //显示隐藏boss气流扰动效果
    public void ShowCircleRaoDong(bool b, Color? color = null)
    {
        if(_bossCircle)
        {
            _bossCircle.RaoDong.material.SetFloat("_DistortStrength", b ? 0.06f : 0f);
        }
    }

    public void SetCircleColor(Color color)
    {
        if (_bossCircle)
        {
            _bossCircle.RaoDong.material.SetColor("_Color", color);
        }
    }

    //显示隐藏血条上的标志点
    public void SetHpHudPointActive(bool b)
    {
        if (_bossHpHud)
        {
            _bossHpHud.Point.SetActiveByCanvasGroup(b);
        }
    }

    //显示隐藏血条
    public void SetHpHudActive(bool b)
    {
        if(_bossHpHud)
        {
            if (b) 
            {
                _bossHpHud.SetActiveSafe(true);
                _bossHpHud.Bar.fillAmount = 0f;
                _bossHpHud.Bar.DOFillAmount(1f, 1f).onComplete = () => { _initedHpBar = true; };
            }
            else
            {
                _bossHpHud.SetActiveSafe(false);
            }
        }
    }

    private void UpdateHpHud()
    {
        if (!_initedHpBar) return;
        if (_bossHpHud == null) return;

        _bossHpHud.Bar.fillAmount = CardMgr.GetHpPercent();
    }

    protected override void CalculateHp(int atk)
    {
        //改为扣符卡血量
        CardMgr.CalculateHp(atk);
    }

    protected override void OnDead()
    {
        GameEventCenter.Send(GameEvent.OnEnemyDie);

        //隐藏血条
        SetHpHudActive(false);

        DOVirtual.DelayedCall(0.3f, () =>
        {
            StageCamera2D.Instance?.Shake(0.7f, 1.2f);
            StageCamera3D.Instance?.Shake(0.7f, 1.2f);

            //震屏
            Sound.PlayUiAudioOneShot(105);

            //播放shader特效
            StageCamera2D.Instance.PlayDeadEffect(transform.position);

            Destroy(gameObject);
        }, false);
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

