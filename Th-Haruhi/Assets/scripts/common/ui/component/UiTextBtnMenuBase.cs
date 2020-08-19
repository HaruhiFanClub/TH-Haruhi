

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiTextBtnMenuBase : MonoBehaviour
{
    private readonly List<UiTextButton> _buttonList = new List<UiTextButton>();
    public UiTextButton CurrSelect { private set; get; }

    protected virtual void Start()
    {
        var btns = GetComponentsInChildren<UiTextButton>();
        var bSelectFirst = false;
        DisableSelectAll();

        for (int i = 0; i < btns.Length; i++)
        {
            var btn = btns[i];
            btn.MenuIndex = i;

            //自动选中第一个按钮
            if (!bSelectFirst)
            {
                if (btn.IsEnable)
                    btn.SetSelect(true);

                CurrSelect = btn;
                bSelectFirst = true;
            }
            else
            {
                btn.SetSelect(false);
            }

            _buttonList.Add(btn);
        }


        GameEventCenter.AddListener(GameEvent.UI_Sure, OnClickSure);
    }

    protected virtual void OnDestroy()
    {
        GameEventCenter.RemoveListener(GameEvent.UI_Sure, OnClickSure);
    }

    private void OnClickSure(object argument)
    {
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
