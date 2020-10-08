using UnityEngine;

public class MarisaLaser : PlayerBullet
{
    private Material _material;
    private float _scaleSpeed = 10f;
    private float _maxScaleMultiple;
    private float _uvSpeed = 1f;

    private const int HitEffectId = 5;
    private TextureEffect _hitEffect;

    private int _lastHurtEnemyFrame;
    private int _nextSoundFrame;

    //激光命中最小间隔
    private int HurtEnemyFrame = 4;
    private int SoundFrame = 12;

    public override void Init(BulletDeploy deploy, Transform master, MeshRenderer model)
    {
        _material = model.material;
        base.Init(deploy, master, model);

        //初始化被击特效
        TextureEffectFactroy.CreateEffect(HitEffectId, SortingOrder.Effect, e => 
        { 
            _hitEffect = e;
            _hitEffect.SetActiveSafe(false);
        });
    }

    public override void ReInit(Transform t)
    {
        base.ReInit(t);
        _material.mainTextureOffset = Vector2.zero;
        transform.localScale = new Vector2(1f, 0f);
    }

    protected override void OnBulletHitEnemy(Enemy enemy)
    {
        if(enemy != null)
        {
            if (GameSystem.FixedFrameCount - _lastHurtEnemyFrame >  HurtEnemyFrame)
            {
                _lastHurtEnemyFrame = GameSystem.FixedFrameCount;
                enemy.OnEnemyHit(Atk);
            }
        }
    }

    protected override void CheckHitEnemy()
    {
        if (InCache) return;
        if (!Shooted) return;
        if (Master == null) return;

        CacheTransform.position = Master.position;

        var delteTime = Time.deltaTime;

        //根据射线，更新最长距离
        var bPlayHitEffect = false;
        var maxDist = 20f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 100f, LayersMask.Enemy);
        if (hit.collider != null)
        {
            maxDist = Vector2.Distance(hit.point, transform.position);

            //如果击中的是敌人，播放特效
            var gameObj = hit.collider.gameObject;
            if (gameObj.layer == Layers.Enemy)
            {
                bPlayHitEffect = true;
                if (_hitEffect)
                {
                    _hitEffect.transform.position = hit.point;
                    _hitEffect.SetActiveSafe(true);
                }

                var enemy = gameObj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    //激光击中敌人伤害
                    OnBulletHitEnemy(enemy);
                }
            }
        }

        if (!bPlayHitEffect)
        {
            if (_hitEffect)
                _hitEffect.SetActiveSafe(false);
        }
        _maxScaleMultiple = maxDist / Renderer.transform.localScale.x;

        //mainTextureOffset(UV动画)
        var t = _material.mainTextureOffset;
        t.x += delteTime * _uvSpeed;
        _material.mainTextureOffset = t;


        //激光长度（动态缩放)
        var scale = transform.localScale;
        scale.y += Time.deltaTime * _scaleSpeed;
        scale.y = Mathf.Clamp(scale.y, 0, _maxScaleMultiple);
        transform.localScale = scale;

        //mainTexture的Scale，防止激光变长后拉伸(与Scale同步)
        var texScale = _material.mainTextureScale;
        texScale.x = transform.localScale.y / 1f;
        _material.mainTextureScale = texScale;

        //音效
        UpdateSoundPlay();
    }

    private void UpdateSoundPlay()
    {
        if (GameSystem.FixedFrameCount > _nextSoundFrame)
        {
            Sound.PlayUiAudioOneShot(2002, true);
            _nextSoundFrame = GameSystem.FixedFrameCount + SoundFrame;
        }
    }

    public override void OnRecycle()
    {
        base.OnRecycle();
        if(_hitEffect)
        {
            _hitEffect.SetActiveSafe(false);
        }
    }
}
