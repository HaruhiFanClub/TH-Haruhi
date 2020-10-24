using DG.Tweening;
using UnityEngine;

public class Laser : EnemyBullet
{
    private float _defaultWidth;

    public override void OnCreate(Vector3 pos)
    {
        base.OnCreate(pos);

        _defaultWidth = CacheTransform.localScale.x;
        Renderer.sortingOrder = SortingOrder.EnemyBullet - 1;

        //先turnoff
        Hide();
    }

    private void Hide()
    {
        SetBanCollision();
        SetWidth(0);
    }

    private void SetWidth(float width)
    {
        var scale = CacheTransform.localScale;
        scale.x = width;
        CacheTransform.localScale = scale;
    }

    private Tweener _turnOnTween;
    public void TurnOn(int frame)
    {
        Sound.PlayTHSound("lazer00", true, 0.5f);

        if (_turnOnTween != null) _turnOnTween.Kill();
        _turnOnTween = CacheTransform.DOScaleX(_defaultWidth, frame * 0.01666f);
        _turnOnTween.onComplete = () =>
        {
            RevertBanCollision();
        };
    }

    private Tweener _turnOffTween;
    public override void DoFadeOut(int frame)
    {
        if (_turnOnTween != null) _turnOnTween.Kill();
        if (_turnOffTween != null) _turnOffTween.Kill();

        SetBanCollision();
        _turnOffTween = CacheTransform.DOScaleX(0, frame * 0.01666f);
        _turnOffTween.onComplete = () =>
        {
            BulletFactory.DestroyBullet(this);
        };
    }

    public override void OnRecycle()
    {
        base.OnRecycle();
        SetWidth(_defaultWidth);
    }
}
