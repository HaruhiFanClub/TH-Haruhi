

using System;
using System.Collections.Generic;
using UnityEngine;

public class UiTextBtnMenuBase : MonoBehaviour
{
    //当前选中第几个的状态记录
    private static Dictionary<Type, int> SelectStatus = new Dictionary<Type, int>();
    public static void ClearSelectStatus()
    {
        SelectStatus.Clear();
    }

    public bool Enable { set; get; }

    private readonly List<UiTextButton> _buttonList = new List<UiTextButton>();
    public UiTextButton CurrSelect { private set; get; }
    private Type _parentUIType;

    protected virtual void Start()
    {
        var btns = GetComponentsInChildren<UiTextButton>();
        var bSelectFirst = false;

        //根据父UI节点类型，拿到默认选项
        var parentUi = GetComponentInParent<UiInstance>();
        int defaultIdx = 0;
        if (parentUi != null) 
        {
            _parentUIType = parentUi.GetType();
            SelectStatus.TryGetValue(_parentUIType, out defaultIdx);
        }

        //初始化
        for (int i = 0; i < btns.Length; i++)
        {
            var btn = btns[i];
            btn.SetMenu(this, i);   

            _buttonList.Add(btn);
        }

        //自动选中
        for (int i = 0; i < btns.Length; i++)
        {
            
            var btn = btns[i];
            if (!bSelectFirst)
            {
                if (btn.IsEnable && i == defaultIdx)
                {
                    btn.SetSelect(true, true);
                    CurrSelect = btn;
                    bSelectFirst = true;
                }
            }
            else
            {
                btn.SetSelect(false);
            }
        }


        GameEventCenter.AddListener(GameEvent.UI_Sure, OnClickSure);
    }

    protected virtual void OnDestroy()
    {
        GameEventCenter.RemoveListener(GameEvent.UI_Sure, OnClickSure);
    }


    public void OnBtnSelected(UiTextButton btn)
    {
        if(_parentUIType != null)
        {
            SelectStatus[_parentUIType] = btn.MenuIndex;
        }
    }

    private void OnClickSure(object argument)
    {
        if(Enable)
            CurrSelect.DoClick();
    }

    protected void SelectNext()
    {
        if (CurrSelect.InClick) return;

        DisableSelectAll();

        var wantStart = CurrSelect.MenuIndex + 1;
        var startIdx = wantStart > _buttonList.Count ? 0 : wantStart;

        for(int i = startIdx; i < _buttonList.Count; i++)
        {
            if (_buttonList[i].IsEnable)
            {
                CurrSelect = _buttonList[i];
                CurrSelect.SetSelect(true);
                return;
            }
        }

        for (int i = 0; i < _buttonList.Count; i++)
        {
            if (_buttonList[i].IsEnable)
            {
                CurrSelect = _buttonList[i];
                CurrSelect.SetSelect(true);
                return;
            }
        }
    }

    protected void SelectPrev()
    {
        if (CurrSelect.InClick) return;

        DisableSelectAll();

        var wantStart = CurrSelect.MenuIndex - 1;
        var startIdx = wantStart < 0 ? _buttonList.Count - 1 : wantStart;

        for (int i = startIdx; i >= 0; i--)
        {
            if (_buttonList[i].IsEnable)
            {
                CurrSelect = _buttonList[i];
                CurrSelect.SetSelect(true);
                return;
            }
        }

        for (int i = _buttonList.Count - 1; i >= 0; i--)
        {
            if (_buttonList[i].IsEnable)
            {
                CurrSelect = _buttonList[i];
                CurrSelect.SetSelect(true);
                return;
            }
        }
    }

    
    private void DisableSelectAll()
    {
        for(int i = 0; i < _buttonList.Count; i++)
        {
            _buttonList[i].SetSelect(false);
        }
    }

    private void Update()
    {
        for (int i = 0; i < _buttonList.Count; i++)
        {
            if (_buttonList[i])
                _buttonList[i].OnUpdate();
        }
    }

}
