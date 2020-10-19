using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class Laser : EnemyBullet
{
    private float _defaultWidth;
    private float _defaultBoxWidth;
    private float _defaultBoxHeight;

    public override void Shoot(MoveData moveData, List<EventData> eventList = null, int atk = 1, Action<Bullet> onDestroy = null)
    {
        base.Shoot(moveData, eventList, atk, onDestroy);
        Sound.PlayTHSound("lazer00", true, 0.5f);

        _defaultWidth = CacheTransform.localScale.x;
        _defaultBoxWidth = CollisionInfo.BoxWidth;
        _defaultBoxHeight = CollisionInfo.BoxHeight;

        CollisionInfo.BoxWidth *= CacheTransform.localScale.x;
        CollisionInfo.BoxHeight *= CacheTransform.localScale.y;

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

    private TweenerCore<Vector3, Vector3, VectorOptions> _turnOnTween;
    public void TurnOn(int frame)
    {
        if (_turnOnTween != null) _turnOnTween.Kill();
        _turnOnTween = CacheTransform.DOScaleX(_defaultWidth, frame * 0.01666f);
        _turnOnTween.onComplete = () =>
        {
            RevertBanCollision();
        };
    }

    private TweenerCore<Vector3, Vector3, VectorOptions> _turnOffTween;
    public void TurnOff(int frame)
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
        CollisionInfo.BoxWidth = _defaultBoxWidth;
        CollisionInfo.BoxHeight = _defaultBoxHeight;
    }
}
