using UnityEngine;

public class MarisaLaser : Bullet
{
    private Material _material;
    private float _defaultScaleY;
    private float _scaleSpeed = 10f;
    private float _maxScaleMultiple = 3f;
    private float _uvSpeed = 1f;
    
    public override void Init(BulletDeploy deploy, Transform master, GameObject model)
    {
        _material = model.GetComponent<Renderer>().material;
        _defaultScaleY = transform.localScale.y;
        base.Init(deploy, master, model);
    }

    public override void ReInit(Transform t)
    {
        base.ReInit(t);

        //缩放默认为0
        ScaleMultiple = 0;
        _material.mainTextureOffset = Vector2.zero;
    }

    private float _scaleMultiple;
    private float ScaleMultiple
    {
        set
        { 
            if(_scaleMultiple != value)
            {
                var scale = transform.localScale;
                scale.y = value * _defaultScaleY;
                transform.localScale = scale;
            }
            _scaleMultiple = value; 
        }
        get
        {
            return _scaleMultiple;
        }
    }

    private float _nextSoundTime;
    protected override void Update()
    {
        base.Update();
        if (InCache) return;

        transform.position = Master.position;

        var delteTime = Time.deltaTime;
        var t = _material.mainTextureOffset;
        t.x += delteTime * _uvSpeed;
        _material.mainTextureOffset = t;

        if (ScaleMultiple < _maxScaleMultiple)
        {
            ScaleMultiple += Time.deltaTime * _scaleSpeed;
            var multi = transform.localScale.y / _defaultScaleY;
            var texScale = _material.mainTextureScale;
            texScale.x = multi;
            _material.mainTextureScale = texScale;
        }


        if (Time.time > _nextSoundTime)
        {
            Sound.PlayUiAudioOneShot(2002, true);
           _nextSoundTime = Time.time + GameSystem.FrameTime * 12;
        }
    }
}
