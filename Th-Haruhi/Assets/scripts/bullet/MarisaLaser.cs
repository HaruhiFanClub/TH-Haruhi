using UnityEngine;

public class MarisaLaser : Bullet
{
    private Material _material;
    private float _scaleSpeed = 10f;
    private float _maxScaleMultiple;
    private float _uvSpeed = 1f;

    private const int HitEffectId = 5;
    private TextureEffect _hitEffect;

    private float _lastHurtEnemyTime;

    //激光命中最小间隔
    private int HurtEnemyFrame = 4;

    public override void Init(BulletDeploy deploy, Transform master, GameObject model)
    {
        _material = model.GetComponent<Renderer>().material;
        base.Init(deploy, master, model);
        AutoDestroy = false;

        //激光的中心点在底部
        var bRota = (int)deploy.rota % 90 == 0;
        var localScale = model.transform.localScale;
        model.transform.localPosition = new Vector3(0, bRota ? localScale.x / 2f : localScale.y / 2f, 0);

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

    private void OnHitEnemy(GameObject enemyObj)
    {
        if (Time.time - _lastHurtEnemyTime > GameSystem.FrameTime * HurtEnemyFrame)
        {
            _lastHurtEnemyTime = Time.time;
            var enemy = enemyObj.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.OnEnemyHit(Deploy.atk);
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        if (InCache) return;
        if (Master == null) return;

        transform.position = Master.position;

        var delteTime = Time.deltaTime;

        //根据射线，更新最长距离
        var bPlayHitEffect = false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 100f, LayersMask.Enemy | LayersMask.BulletDestroy);
        if (hit.collider != null)
        {
            var dist = Vector2.Distance(hit.point, transform.position);
            _maxScaleMultiple = dist / Model.transform.localScale.x;

            //如果击中的是敌人，播放特效
            if(hit.collider.gameObject.layer == Layers.Enemy)
            {
                bPlayHitEffect = true;
                if(_hitEffect)
                {
                    _hitEffect.transform.position = hit.point;
                    _hitEffect.SetActiveSafe(true);
                }

                //激光击中敌人伤害
                OnHitEnemy(hit.collider.gameObject);
            }
        }

        if(!bPlayHitEffect)
        {
            if (_hitEffect)
                _hitEffect.SetActiveSafe(false);
        }

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

    private float _nextSoundTime;
    private void UpdateSoundPlay()
    {
        if (Time.time > _nextSoundTime)
        {
            Sound.PlayUiAudioOneShot(2002, true);
            _nextSoundTime = Time.time + GameSystem.FrameTime * 12;
        }
    }


    public override void OnBulletHitEnemy()
    {
        //base.OnBulletHitEnemy();
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
