

using System;
using UnityEngine;
using System.Collections;

public abstract class UiInstance : UiGameObject
{
    public static IEnumerator LoadUi(UiInfo uiInfo, string resource, Type typeUi, Action<UiInstance> notify)
    {
        var async = new AsyncResource();
        yield return ResourceMgr.LoadObjectWait(resource, async);

        var uiObject = ResourceMgr.Instantiate(async.Object);
        uiObject.SetActiveSafe(true);
        var script = GameObjectTools.AddComponent(uiObject, typeUi);
        UiInstance ui = script as UiInstance;
        if (ui)
        {
            ui.UiInfo = uiInfo;
            ui.OnLoadFinish();
            UiManager.ShowUiView(ui, uiInfo.Layer);
            notify(ui);
        }
        else
        {
            Debug.LogError(string.Format("Load UI fail ui == null resource{0}", resource));
        }
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

    protected virtual void OnFullViewShow(bool fromBack)
    {

    }

    public void AfterOnShow(bool fromBack = false)
    {
        OnFullViewShow(fromBack);
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
