

using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class UiFullView : UiInstance
{
    public static UiFullView CurrentView { private set; get; }
    public static List<Type> PrevViewTypes { private set; get; }

    static UiFullView()
    {
        PrevViewTypes = new List<Type>();
        GameEventCenter.AddListener(GameEvent.UI_Back, OnBack);
    }

    public static void Clear()
    {
        PrevViewTypes.Clear();
        CurrentView = null;
    }

    private static bool _inClose;
    private static void OnBack(object o)
    {
        if (CurrentView == null) return;
        if (CurrentView.GetType() == typeof(UIMainView))
            return;

        var prevViewType = PrevViewTypes[PrevViewTypes.Count - 1];
        if (_inClose) return;

        CurrentView.Close(()=>
        {
            if (prevViewType == typeof(UIMainView))
            {
                UIMainView.Show(false);
            }
            else
            {
                UiManager.ShowUIFromBack(prevViewType, null);
            }
            PrevViewTypes.Remove(prevViewType);
            _inClose = false;
        });
        _inClose = true;
        Sound.PlayUiAudioOneShot(1003);
       
    }

    protected abstract Animator Animator { get; }

    protected abstract void OnOpenOver();

    private bool _bOpening;
    private bool _bClosing;
    protected override void Update()
    {
        base.Update();

        if (_bOpening && Animator.GetCurrentAnimatorStateInfo(0).IsName("Loop"))
        {
            OnOpenOver();
            _bOpening = false;
        }

        if (_bClosing && Animator.GetCurrentAnimatorStateInfo(0).IsName("CloseOver"))
        {
            RealClose();
            _bClosing = false;
        }
    }


    //记录上一个选择的界面
    protected override void OnFullViewShow(bool fromBack)
    {
        _bOpening = true;

        if (GetType() != typeof(UIMainView))
        {
            Animator.Play("Open");
        }

        if (CurrentView != null)
        {
            if(!fromBack)
            {
                PrevViewTypes.Add(CurrentView.GetType());
            }
            CurrentView.Close();
        }

        CurrentView = this;
    }

    private Action<UiInstance> _closeNotify;
    public override void OnClose(Action<UiInstance> notify)
    {
        if (GetType() != typeof(UIMainView))
        {
            _bClosing = true;
            _closeNotify = notify;
            Animator.Play("Close");
        }
        else
        {
            base.OnClose(notify);
        }
    }

    protected virtual void RealClose()
    {
        base.OnClose(_closeNotify);
    }
}
