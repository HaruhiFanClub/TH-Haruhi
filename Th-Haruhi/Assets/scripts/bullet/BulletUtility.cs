using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class BulletUtility
{
    private static Dictionary<int, float> ShootEffectCd = new Dictionary<int, float>();

    public static void PlayShootEffect(this Bullet bullet, float shootEffectScale = 1.5f)
    {
        if (bullet.InHidden) return;

        //同一位置加2fps cd
        var pos = bullet.Renderer.transform.position;

        var key = (int)(pos.x * 100) + ((int)(pos.y * 100) * 100);
        if (ShootEffectCd.TryGetValue(key, out var lastFrame))
        {
            if (GameSystem.FixedFrameCount - lastFrame < 2)
                return;
        }

        ShootEffectCd[key] = GameSystem.FixedFrameCount;

        var effectId = 0;
        switch (bullet.Deploy.EColor)
        {
            case EColor.Red:
                effectId = 901;
                break;
            case EColor.Purple:
                effectId = 902;
                break;
            case EColor.Blue:
                effectId = 903;
                break;
            case EColor.BlueLight:
                effectId = 904;
                break;
            case EColor.Green:
                effectId = 905;
                break;
            case EColor.Yellow:
                effectId = 906;
                break;
            case EColor.Orange:
                effectId = 907;
                break;
            case EColor.White:
                effectId = 908;
                break;
        }

        if (effectId > 0)
        {
            TextureEffectFactroy.CreateEffect(effectId, SortingOrder.ShootEffect, effect =>
            {
                effect.transform.position = pos;
                effect.transform.localScale = Vector3.one * shootEffectScale;
                effect.Renderer.material.SetFloat("_Brightness", 3f);
                effect.transform.DOScale(0f, 0.4f).onComplete = () =>
                {
                    TextureEffectFactroy.DestroyEffect(effect);
                };
            });
        }
    }


    public static void PlayEffectAndDestroy(this Bullet bullet, int effectId = 501)
    {
        var pos = bullet.Renderer.transform.position;
        BulletFactory.DestroyBullet(bullet);

        if(!bullet.InHidden)
        {
            TextureEffectFactroy.CreateEffect(effectId, SortingOrder.ShootEffect, effect =>
            {
                effect.Renderer.material.SetColor("_TintColor", ColorUtility.GetColor(bullet.Deploy.EColor));
                effect.transform.position = pos;
                effect.AutoDestroy();
            });
        }
    }
}

