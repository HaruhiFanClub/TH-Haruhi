

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
    private string BossHpBar = "ui/prefabs/battle/UIBossHpCircle.prefab";

    private UIBossHpComponent _bossHpHud;
    public override void Init(SpriteRenderer renderer, EnemyDeploy deploy)
    {
        base.Init(renderer, deploy);

        //血条
        var bossHpHudObj = ResourceMgr.Instantiate(ResourceMgr.LoadImmediately(BossHpBar));
        bossHpHudObj.transform.SetParent(gameObject.transform, false);
        _bossHpHud = bossHpHudObj.GetComponent<UIBossHpComponent>();
        _bossHpHud.Canvas.worldCamera = StageCamera2D.Instance.MainCamera;
    }

    private void UpdateHpHud()
    {
        if (_bossHpHud == null) return;
        _bossHpHud.Bar.fillAmount = (float)HP / HPMax;

    }
    protected override void Update()
    {
        base.Update();
        UpdateHpHud();
    }
}

