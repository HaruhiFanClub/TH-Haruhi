﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public enum EImagePath
{
    Public,
}

public static class UIUtility
{
    public static CanvasGroup CreateCanvasGroup(this UIBehaviour uiObject)
    {
        var group = uiObject.GetComponent<CanvasGroup>();
        if (group == null)
            group = uiObject.gameObject.AddComponent<CanvasGroup>();
        group.ignoreParentGroups = false;
        return group;
    }

    public static void SetSprite(this UiImage image, EImagePath path, string spriteName, bool nativeSize = false)
    {
        if (string.IsNullOrEmpty(spriteName))
        {
            //Debug.LogError("XUI_Image SetSprite 不要为空! path:" + path + " spriteName:" + spriteName);
            image.Sprite = null;
            image.SetActiveByCanvasGroup(false);
        }
        else
        {
            var bSame = image.Sprite != null && spriteName == image.Sprite.name;
            if (!bSame)
            {
                SpriteAtlasMgr.LoadSprite(path.ToString(), atlas =>
                {
                    image.Sprite = atlas.GetSprite(spriteName);
                    if (nativeSize) image.SetNativeSize();
                    image.SetActiveByCanvasGroup(true);
                });
            }
        }
    }


    public static void SetRawImageTexture(this RawImage rawImage, string path, Action<RawImage> onFinished)
    {
        if (!rawImage) return;
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        ResourceMgr.LoadObject(path, o =>
        {
            var texture = o as Texture;
            if (texture)
            {
                rawImage.texture = texture;
                onFinished.Invoke(rawImage);
            }
        });
    }
}
