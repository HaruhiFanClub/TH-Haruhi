
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class UIDrawingChat : UiInstance
{

    public static void Show(List<DialogDeploy> list, Action refreshBossAction, Action overAction)
    {
        UiManager.Show<UIDrawingChat>(view =>
        {
            view.Init(list, refreshBossAction, overAction);
        });
    }

    private UIDrawingChatComponent _bind;
    private UIDrawingChatLR _prevChat;
    private Action _overAction;
    private Action _refreshBossAction;
    private bool _inForceWait;
    private bool _firstLeft = true;
    private bool _firstRight = true;

    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
        _bind = GetComponent<UIDrawingChatComponent>();
        GameEventCenter.AddListener(GameEvent.UI_NextChat, OnClickNext);
        DialogMgr.InDrawingDialog = true;
    }

    public override void OnClose(Action<UiInstance> notify)
    {
        base.OnClose(notify);
        _overAction();
        DialogMgr.InDrawingDialog = false;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        DialogMgr.InDrawingDialog = false;
        GameEventCenter.RemoveListener(GameEvent.UI_NextChat, OnClickNext);
    }

    private float _lastShowTime;
    private void OnClickNext(object o)
    {
        //如果正在播放，则先强制结束
        if (Time.time - _lastShowTime > 0.55f)
        {
            if(!_inForceWait)
            {
                StartCoroutine(ShowNext());
            }
        }
    }

    private List<DialogDeploy> _list;
    private void Init(List<DialogDeploy> list,Action refreshBossAction, Action overAction)
    {
        _list = list;
        _overAction = overAction;
        _refreshBossAction = refreshBossAction;
        StartCoroutine(ShowNext());
    }

    private IEnumerator ShowNext()
    {
      

        //无内容了。关闭
        if (_list.Count <= 0) 
        {
            StartCoroutine(FadeClose());
            yield break;
        }

        var info = _list[0];
        _list.RemoveAt(0);

        if (_prevChat != null)
        {
            //淡出前面的
            if (_prevChat.IsLeft != info.isLeft)
            {
                _prevChat.FadeOut();
            }

            //刷新boss事件
            if(_prevChat.Deploy.refreshBoss)
            {
                if(_refreshBossAction != null)
                {
                    _refreshBossAction();
                    _refreshBossAction = null;
                }
            }

            if(_prevChat.Deploy.forceWaitTime > 0)
            {
                _inForceWait = true;
                yield return new WaitForSeconds(_prevChat.Deploy.forceWaitTime);
                _inForceWait = false;
            }
        }

        //显示后面的
        if (info.isLeft)
        {
            _bind.Left.Show(info, _firstLeft);
            _prevChat = _bind.Left;
            _firstLeft = false;
        }
        else
        {
            _bind.Right.Show(info, _firstRight);
            _prevChat = _bind.Right;
            _firstRight = false;
        }

        _lastShowTime = Time.time;
    }

    private IEnumerator FadeClose()
    {
        _bind.Left.CloseFadeOut(0.6f);
        _bind.Right.CloseFadeOut(0.6f);
        yield return new WaitForSeconds(1f);
        this.Close();
    }
}