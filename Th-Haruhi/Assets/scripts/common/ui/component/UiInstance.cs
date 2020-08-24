

using System;
using System.Collections;
using UnityEngine;



public abstract class UiInstance : UiGameObject
{
    public static UiInstance ShowImmediately(string resource, Type typeUi)
    {
        var obj = ResourceMgr.LoadImmediately(resource);
        var uiObject = ResourceMgr.Instantiate(obj);
        if (uiObject)
        {
            uiObject.SetActiveSafe(true);
            var script = GameObjectTools.AddComponent(uiObject, typeUi);
            UiInstance ui = script as UiInstance;
            if (ui)
            {
                ui.OnLoadFinish();
                return ui;
            }
            Debug.LogError(string.Format("Load UI fail ui == null resource{0}", resource));
        }
        else
        {
            Debug.LogError(string.Format("Load UI fail gameObject == null resource{0}", resource));
        }
        return null;
    }


    public static void LoadUi(UiInfo uiInfo, string resource, Type typeUi, Action<UiInstance> notify)
    {
        ResourceMgr.Load(resource, obj =>
        {
            ResourceMgr.InstantiateX(obj, uiObject =>
            {
                uiObject.SetActiveSafe(true);
                var script = GameObjectTools.AddComponent(uiObject, typeUi);
                UiInstance ui = script as UiInstance;
                if (ui)
                {
                    ui.UiInfo = uiInfo;
                    ui.OnLoadFinish();
                    notify(ui);
                }
                else
                {
                    Debug.LogError(string.Format("Load UI fail ui == null resource{0}", resource));
                }
            });
        });
    }

    public UiInfo UiInfo;
    public float ViewOpenTime;

    public bool InOpen
    {
        get { return gameObject.activeSelf; }
    }

    protected virtual void Update() { }

    protected Canvas Canvas
    {
        get
        {
            var layer = UiInfo.Layer;
            return UiManager.GetCanvasByLayer(layer);
        }
    }

    /// <summary>
    /// 加载结束时机
    /// </summary>
    protected virtual void OnLoadFinish() { }


    /// <summary>
    /// 显示时机
    /// </summary>
    protected virtual void OnShow()
    {
        ViewOpenTime = Time.time;
    }

    public void AfterOnShow()
    {
        OnShow();
    }

    /// <summary>
    /// 关闭时机
    /// </summary>
    public virtual void OnClose(Action<UiInstance> notify)
    {
        if (!gameObject.activeSelf)
            return;

        if (notify != null) notify.Invoke(this);
    }

    protected void OnClickClose()
    {
        this.Close();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        this.OnDestroyInManager();
    }
}
