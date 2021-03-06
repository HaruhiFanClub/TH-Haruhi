﻿
//////////////////////////////////////////////////////////////////////////
//
//   FileName : XUI_Manager.cs
//     Author : Felon
// CreateTime : 2017-05-02
//       Desc :
//
//////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using UnityEngine.EventSystems;

public static class UiManager
{
    public static GameObject UguiRoot;

    public static Camera UiCamera
    {
        get
        {
            if (_uiBind == null)
                return null;
            return _uiBind.UiCamera;
        }
    }

    private static readonly Dictionary<Type, UiInstance> InstDic = new Dictionary<Type, UiInstance>();
    private static readonly Dictionary<string, bool> InstLoadState = new Dictionary<string, bool>();
    private static readonly Dictionary<string, UiInfo> RegisteredUi = new Dictionary<string, UiInfo>();


    public static void Update()
    {
        
    }

    public static RectTransform GetCanvas(UiLayer layer)
    {
        return _uiBind.CanvasParentList[layer];
    }

    public static Canvas GetCanvasByLayer(UiLayer layer)
    {
        return _uiBind.CanvasList[layer];
    }
    
    public static void SetDeltaSize2(Vector2 size, float safeHeightRadio, float bottomGiveY)
    {
        SetDeltaInfo2(UiLayer.Main, _uiBind.CanvasList[UiLayer.Main], size, safeHeightRadio, bottomGiveY);
        SetDeltaInfo2(UiLayer.Tips, _uiBind.CanvasList[UiLayer.Tips], size, safeHeightRadio, bottomGiveY);
    }

    private static void SetDeltaInfo2(UiLayer layer, Canvas canvas, Vector2 size, float safeHeightRadio, float bottomGiveY)
    {
        var rect = canvas.GetComponent(typeof(RectTransform)) as RectTransform;
        if (rect == null) return;

        var s = size;
        s.y *= safeHeightRadio;
        s.y -= bottomGiveY;
        rect.sizeDelta = s;

        var pos = Vector3.zero;
        var a = size.y - rect.sizeDelta.y;
        pos.y -= a / 2;
        pos.y += bottomGiveY;
        rect.anchoredPosition = pos;
    }


    #region UI注册模块

    public static void R<T>(string viewPath, UiLayer layer, UiLoadType loadType) where T : UiInstance
    {
        var fullName = typeof(T).FullName;
        if (fullName != null && RegisteredUi.ContainsKey(fullName))
        {
            return;
        }
        var uiInfo = new UiInfo
        {
            Type = typeof(T),
            ViewPath = viewPath,
            Layer = layer,
            LoadType = loadType,
        };
        var name = typeof(T).FullName;
        if (name != null) RegisteredUi[name] = uiInfo;
    }

    public static UiInfo GetUiInfo(Type type)
    {
        var fullName = type.FullName;
        if (fullName != null && RegisteredUi.ContainsKey(fullName))
        {
            var name = type.FullName;
            if (name != null) return RegisteredUi[name];
        }
        Debug.LogError("UI未注册，请在XUI_LIST.cs中注册:" + type.FullName);
        return null;
    }


    public static UiInfo GetUiInfo<T>() where T : UiInstance
    {
        var fullName = typeof(T).FullName;
        if (fullName != null && RegisteredUi.ContainsKey(fullName))
        {
            var name = typeof(T).FullName;
            if (name != null) return RegisteredUi[name];
        }
        Debug.LogError("UI未注册，请在XUI_LIST.cs中注册:" + typeof(T).FullName);
        return null;
    }
    #endregion


    private static UiRootScript _uiBind;
    public static UiRootScript GetUiBind()
    {
        return _uiBind;
    }

    public static UiBackGround BackGround { private set; get; }
    public static void Init(UiRootScript uiBind)
    {
        UguiRoot = uiBind.gameObject;

        _uiBind = uiBind;

        //注册UI
        UiList.RegisterUI();

        BackGround = _uiBind.BackGround;
    }

    private static void ClearAllUi()
    {
        //清空层里面的所有物体
        if (_uiBind != null && _uiBind.CanvasList != null)
        {
            var e = _uiBind.CanvasList.GetEnumerator();
            using (e)
            {
                while (e.MoveNext())
                {
                    var key = e.Current.Key;
                    if (key == UiLayer.DontDestroy)
                        continue;
                    if (e.Current.Value != null)
                    {
                        GameObjectTools.DestroyAllChild(e.Current.Value.gameObject);
                    }
                }
            }
        }
    }

    public static void ShowUiView(UiInstance view, UiLayer layerName)
    {
        if (view == null)
        {
            Debug.LogError("ShowUiView View Is null, layer:" + layerName);
            return;
        }
        string layer = Enum.GetName(typeof(UiLayer), layerName);

        var parent = _uiBind.CanvasParentList[layerName];

        if (parent == null)
        {
            Debug.LogError("ShowUIView 层级错误，使用了存在不存在的层：" + layer);
            return;
        }

        view.transform.SetParent(parent, false);
        view.transform.SetAsLastSibling();
        view.gameObject.SetActiveSafe(true);
    }


    //清除所有不可见单例界面
    private static void RemoveAllInstance()
    {
        Dictionary<Type, UiInstance>.Enumerator enumerator = InstDic.GetEnumerator();
        List<Type> removeList = new List<Type>();
        using (enumerator)
        {
            while (enumerator.MoveNext())
            {
                if (!enumerator.Current.Value) continue;
                if (enumerator.Current.Value.gameObject.activeSelf) continue;
                removeList.Add(enumerator.Current.Key);
            }
        }

        for (int i = 0; i < removeList.Count; i++)
        {
            UiInstance xuiInstance;
            bool exists = InstDic.TryGetValue(removeList[i], out xuiInstance);
            if (exists)
            {
                Object.DestroyImmediate(xuiInstance.gameObject);
                InstDic.Remove(removeList[i]);
            }
        }

        enumerator.Dispose();
    }

    private static void ShowInstanceUi<T>(UiInfo uiInfo, bool fromBack, Action<T> notify = null, Type classType = null) where T : UiInstance
    {
        Type typeKey = classType ?? typeof(T);

        //存在缓存
        if (InstDic.ContainsKey(typeKey))
        {
            UiInstance inst = InstDic[typeKey];
            AfterLoadInstance(inst, uiInfo, fromBack, notify);
        }
        else
        {
            //已经在加载中
            bool bLoading;
            if (InstLoadState.TryGetValue(uiInfo.ViewPath, out bLoading) && bLoading)
            {
                //do noting
            }
            else
            {
                //加载
                InstLoadState[uiInfo.ViewPath] = true;

                GameSystem.Start(UiInstance.LoadUi(uiInfo, uiInfo.ViewPath, typeKey, inst =>
                {
                    InstLoadState[uiInfo.ViewPath] = false;
                    InstDic[typeKey] = inst;
                    AfterLoadInstance(inst, uiInfo, fromBack, notify);
                }));
            }
        }
    }

    private static void AfterLoadInstance<T>(UiInstance inst, UiInfo uiInfo, bool fromBack, Action<T> notify) where T : UiInstance
    {
        inst.AfterOnShow(fromBack);
        if (notify != null) notify.Invoke(inst as T);
    }


    /// <summary>
    /// 获取单例UI实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetInstance<T>() where T : UiInstance
    {
        UiInstance ins;
        InstDic.TryGetValue(typeof(T), out ins);
        return (ins as T);
    }

    public static void ShowUIFromBack(Type type, Action<UiInstance> notify)
    {
        var uiInfo = GetUiInfo(type);
        if (uiInfo == null)
        {
            Debug.LogError("UI未注册，请在XUI_LIST.cs中注册:" + type.FullName);
            return;
        }

        //单例UI
        if (uiInfo.LoadType == UiLoadType.Single)
        {
            ShowInstanceUi(uiInfo, true, notify);
        }
        //非单例
        else
        {
            GameSystem.Start(UiInstance.LoadUi(uiInfo, uiInfo.ViewPath, type, inst =>
            {
                if (notify != null) notify.Invoke(inst);
                inst.AfterOnShow(true);
            }));
        }
    }

    /// <summary>
    /// 显示界面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="notify"></param>
    public static void Show<T>(Action<T> notify = null) where T : UiInstance
    {
        var uiInfo = GetUiInfo<T>();
        if (uiInfo == null)
        {
            Debug.LogError("UI未注册，请在XUI_LIST.cs中注册:" + typeof(T).FullName);
            return;
        }

        //单例UI
        if (uiInfo.LoadType == UiLoadType.Single)
        {
            ShowInstanceUi<T>(uiInfo, false, notify);
        }
        //非单例
        else
        {
            GameSystem.Start(UiInstance.LoadUi(uiInfo, uiInfo.ViewPath, typeof(T), inst =>
            {
                if (notify != null) notify.Invoke(inst as T);
                inst.AfterOnShow();
            }));
        }
    }

    /// <summary>
    /// 显示UI特效
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="layerName"></param>
    public static void ShowEffect(TextureEffect effect, UiLayer layerName = UiLayer.Tips)
    {
        string layer = Enum.GetName(typeof(UiLayer), layerName);
        Transform parent = _uiBind.CanvasParentList[layerName];
        if (parent == null)
        {
            Debug.LogError("ShowUIView 层级错误，使用了存在不存在的层：" + layer);
            return;
        }
        effect.transform.SetLayer(Layers.Ui);
        effect.gameObject.transform.SetParent(parent, false);
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="notify"></param>
    public static void Close(this UiInstance instance, Action notify = null)
    {
        if (instance == null) return;
        instance.OnClose(inst =>
        {
            if (notify != null) notify.Invoke();
            //如果是单例UI，则隐藏
            if (inst.UiInfo.LoadType == UiLoadType.Single)
            {
                inst.gameObject.SetActiveSafe(false);
            }
            else
            {
                Object.Destroy(inst.gameObject);
            }
        });

    }

    public static void Close<T>(Action notify = null) where T : UiInstance
    {
        Close(GetInstance<T>(), notify);
    }

    /// <summary>
    /// 清空所有成员
    /// </summary>
    /// <param name="instance"></param>
    public static void RemoveAllChild(this GameObject instance)
    {
        if (instance == null) return;
        for (var i = 0; i < instance.transform.childCount; i++)
        {
            var item = instance.transform.GetChild(i);
            Object.Destroy(item.gameObject);
        }
        instance.transform.DetachChildren();
    }
    public static void RemoveAllChild(this Transform instance)
    {
        if (instance == null) return;
        for (var i = 0; i < instance.childCount; i++)
        {
            var item = instance.GetChild(i);
            Object.Destroy(item.gameObject);
        }
        instance.DetachChildren();
    }

    /// <summary>
    /// 单例释放
    /// </summary>
    /// <param name="instance"></param>
    public static void OnDestroyInManager(this UiInstance instance)
    {
        if (InstDic.ContainsKey(instance.GetType()))
            InstDic.Remove(instance.GetType());
    }

    /// <summary>
    /// 设置某个Prefab的显示层，包含child
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="layer"></param>
    public static void SetLayers(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayers(child.gameObject, layer);
        }
    }

    public static void Clear()
    {
        UiFullView.Clear();
        UiCacheInstance.BackAllToPool();
        ClearAllUi();
    }

    public static void Destory()
    {
       
    }
}
