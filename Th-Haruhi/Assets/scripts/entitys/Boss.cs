
using DG.Tweening;
using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// boss
/// 分阶段
/// 有圆圈血条
/// 有特殊UI展示
/// 有特殊立绘
/// </summary>
public class Boss : Enemy
{
    private const string BossHpBar = "ui/prefabs/battle/UIBossHpHud.prefab";
    private const string BossHpCircle = "ui/prefabs/battle/UIBossCircle.prefab";

    //上方中心位置
    public static Vector3 BossUpCenter = new Vector3(0f, 144f);

    //中心位置
    public static Vector3 BossMidCenter = new Vector3(0f, 0f);

    //boss符卡管理器
    private BossCardMgr CardMgr;
    private UIBossHpComponent _bossHpHud;
    private UIBossCircleComponent _bossCircle;
    private bool _initedHpBar;


    public override IEnumerator Init(EnemyDeploy deploy)
    {
        yield return base.Init(deploy);

        //血条
        var asyncHp = new AsyncResource();

        yield return ResourceMgr.LoadObjectWait(BossHpBar, asyncHp);
        var bossHpHudObj = ResourceMgr.Instantiate(asyncHp.Object);
        bossHpHudObj.transform.SetParent(gameObject.transform, false);
        _bossHpHud = bossHpHudObj.GetComponent<UIBossHpComponent>();
        _bossHpHud.SetActiveSafe(false);

        //boss背景
        var asyncBg = new AsyncResource();
        yield return ResourceMgr.LoadObjectWait(BossHpCircle, asyncBg); 
        var circle = ResourceMgr.Instantiate(asyncBg.Object);
        circle.transform.SetParent(gameObject.transform, false);
        _bossCircle = circle.GetComponent<UIBossCircleComponent>();
        _bossCircle.SetActiveSafe(false);



        CardMgr = new BossCardMgr();
        CardMgr.Init(this, HPMax);

        Invisible = true;

        TryDialog();
    }

    private void TryDialog()
    {
        if(StageMgr.MainPlayer != null)
        {
            var deploy = DialogMgr.GetBossDialog(StageMgr.MainPlayer.Deploy.id, Deploy.id, true);
            if(deploy != null)
            {
                var list = DialogMgr.GetDrawList(deploy.dialogId);
                UIDrawingChat.Show(list, 
                    ()=> 
                    { 
                        MoveToPos(BossUpCenter, 60, MovementMode.MOVE_NORMAL); 
                    },
                    () =>
                    {
                        StartFight();
                        if (deploy.bgmId > 0)
                        {
                            Sound.PlayMusic(deploy.bgmId);
                            UIBgmTip.Show(deploy.bgmId);
                        }
                    });
                return;
            }
        }

        //如果没有对话，直接移动到指定位置，并开始战斗
        MoveToPos(BossUpCenter, 60, MovementMode.MOVE_NORMAL);
        DOVirtual.DelayedCall(1.15F, StartFight, false);
    }

    private void StartFight()
    {
        //第一符卡，播放收放特效(关卡中途加入的boss不需要)
        if (!CardMgr.IsSingleCard())
        {
            PlayShirnkEffect(true);
        }

        //显示血条
        _bossHpHud.Canvas.sortingOrder = SortingOrder.EnemyBullet + 1;
        _bossHpHud.Canvas.worldCamera = StageCamera2D.Instance.MainCamera;

        //圆圈
        _bossCircle.TurnOn();

        //bossMark显示
        UIBattle.SetBossMarkActive(true);


        //bossCard
        DOVirtual.DelayedCall(1F, CardMgr.OnStartFight, false);

        //禁止无敌
        Invisible = false;
    }

    //显示隐藏boss气流扰动效果
    public void ShowCircleRaoDong(bool b)
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

    public void ShowHpCircle(bool playFullAni, bool showPoint)
    {
        if (!_bossHpHud)
        {
            Debug.LogError("ShowHpCircle Error, BossHpHud = null");
            return;
        }
        _bossHpHud.SetActiveSafe(true);
        _bossHpHud.Point.SetActiveByCanvasGroup(showPoint);
        if (playFullAni)
        {
            _bossHpHud.Bar.fillAmount = 0f;
            _bossHpHud.Bar.DOFillAmount(1f, 1f).onComplete = () => { _initedHpBar = true; };
        }
    }

    public void HideHpCircle()
    {
        if (!_bossHpHud)
        {
            Debug.LogError("HideHpCircle Error, BossHpHud = null");
            return;
        }
        _bossHpHud.SetActiveSafe(false);
    }

    private void UpdateHpHud()
    {
        if (!_initedHpBar) return;
        if (_bossHpHud == null) return;

        _bossHpHud.Bar.fillAmount = CardMgr.GetHpPercent();
    }

    public void PlayShirnkEffect(bool bPlayAmplify = false)
    {
        //音效
        Sound.PlayTHSound("power3", true, 0.8f);

        ResourceMgr.LoadObject("effects_tex/prefab/bossStart2.prefab", obj =>
        {
            var effect = ResourceMgr.Instantiate(obj);
            effect.SetRendererOrderSort(SortingOrder.Top);
            var e = effect.AddComponent<EffectMono>();
            e.transform.SetParent(transform, false);
            e.AutoDestory();
        });

        if (bPlayAmplify)
        {
            StartCoroutine(PlayAmplifyEffect(1.2f));
        }

    }

    public IEnumerator PlayAmplifyEffect(float sec)
    {
        yield return new WaitForSeconds(sec);
        var pos = transform.position; 
        EffectFactory.CreateEffect("effects_tex/prefab/bossStart.prefab", eff =>
        {
            if (gameObject == null)
            {
                eff.transform.position = pos;
            }
            else
            {
                eff.transform.SetParent(transform, false);
            }
            eff.gameObject.SetRendererOrderSort(SortingOrder.Top);
            Sound.PlayTHSound("tan02", false, 0.4f);
            eff.AutoDestory();
        });
    }


    protected override void CalculateHp(int atk)
    {
        //改为扣符卡血量
        CardMgr?.CalculateHp(atk);
    }

    protected override void OnDead()
    {
        StartCoroutine(DoBossDead());
    }

    private IEnumerator DoBossDead()
    {
        //对话配置
        var dialogDeploy = DialogMgr.GetBossDialog(StageMgr.MainPlayer.Deploy.id, Deploy.id, false);

        //隐藏血条
        HideHpCircle();

        //bossMark隐藏
        UIBattle.SetBossMarkActive(false);

        yield return new WaitForSeconds(0.3f);

        //震屏
        if(StageCamera2D.Instance) StageCamera2D.Instance.Shake(0.7f, 1.2f);
        if(StageCamera3D.Instance) StageCamera3D.Instance.Shake(0.7f, 1.2f);

        Sound.PlayTHSound("enep01", false, 0.3f);

        //播放shader特效
        StageCamera2D.Instance.PlayDeadEffect(transform.position);

        //隐藏renderer
        MainRenderer.enabled = false;
        if(_bossCircle)
        {
            _bossCircle.SetActiveSafe(false);
        }

        yield return new WaitForSeconds(0.4f);

        //尝试显示对话
        if (dialogDeploy != null)
        {
            var list = DialogMgr.GetDrawList(dialogDeploy.dialogId);
            UIDrawingChat.Show(list, null,
            () =>
            {
                GameEventCenter.Send(GameEvent.OnEnemyDie);
            });
        }
        else
        {
            GameEventCenter.Send(GameEvent.OnEnemyDie);
        }

        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        UpdateHpHud();
        UIBattle.SetBossMarkPos(transform.position);
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        CardMgr?.OnFixedUpdate();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        CardMgr?.OnDestroy();
        CardMgr = null;
    }
}

