

using System;
using System.Collections.Generic;
using UnityEngine;

public class UiMenuBase : MonoBehaviour
{
    //当前选中第几个的状态记录
    private static Dictionary<Type, int> SelectStatus = new Dictionary<Type, int>();

    public static void SetDefultIndex(Type type, int idx)
    {
        if(!SelectStatus.ContainsKey(type))
        {
            SelectStatus[type] = idx;
        }
    }

    public static void ClearSelectStatus()
    {
        SelectStatus.Clear();
    }

    public bool Enable { set; get; }

    private Type _parentUIType;

    public int CurrSelectIdx { private set; get; }
    public readonly List<ISelectAble> ItemList = new List<ISelectAble>();
    public event Action OnInitOver;
    public event Action OnSelectChange;

    protected virtual void Start()
    {
        var btns = GetComponentsInChildren<ISelectAble>();
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
            btn.SetSelect(false);   
            ItemList.Add(btn);
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
                    CurrSelectIdx = i;
                    bSelectFirst = true;
                }
            }
            else
            {
                btn.SetSelect(false);
            }
        }

        OnInitOver?.Invoke();

        GameEventCenter.AddListener(GameEvent.UI_Sure, OnClickSure);
    }

    protected virtual void OnDestroy()
    {
        GameEventCenter.RemoveListener(GameEvent.UI_Sure, OnClickSure);
    }


    public void OnSelected(ISelectAble btn)
    {
        btn.SetSelect(true);
        Sound.PlayUiAudioOneShot(1001);

        if (_parentUIType != null)
        {
            SelectStatus[_parentUIType] = CurrSelectIdx;
        }

        OnSelectChange?.Invoke();
    }

    public void OnUnSelected(ISelectAble btn)
    {
        btn.SetSelect(false);
    }

    private void OnClickSure(object argument)
    {
        if(Enable)
        {
            ItemList[CurrSelectIdx].DoClick();
        }
    }

    protected void SelectNext()
    {
        if (ItemList[CurrSelectIdx].InClick) return;

        DisableSelectAll();

        var wantStart = CurrSelectIdx + 1;
        var startIdx = wantStart > ItemList.Count ? 0 : wantStart;

        for(int i = startIdx; i < ItemList.Count; i++)
        {
            if (ItemList[i].IsEnable)
            {
                CurrSelectIdx = i;
                OnSelected(ItemList[i]);
                return;
            }
        }

        for (int i = 0; i < ItemList.Count; i++)
        {
            if (ItemList[i].IsEnable)
            {
                CurrSelectIdx = i;
                OnSelected(ItemList[i]);
                return;
            }
        }
    }

    protected void SelectPrev()
    {
        if (ItemList[CurrSelectIdx].InClick) return;

        DisableSelectAll();

        var wantStart = CurrSelectIdx - 1;
        var startIdx = wantStart < 0 ? ItemList.Count - 1 : wantStart;

        for (int i = startIdx; i >= 0; i--)
        {
            if (ItemList[i].IsEnable)
            {
                CurrSelectIdx = i;
                OnSelected(ItemList[i]);
                return;
            }
        }

        for (int i = ItemList.Count - 1; i >= 0; i--)
        {
            if (ItemList[i].IsEnable)
            {
                CurrSelectIdx = i;
                OnSelected(ItemList[i]);
                return;
            }
        }
    }

    
    private void DisableSelectAll()
    {
        for(int i = 0; i < ItemList.Count; i++)
        {
            if (CurrSelectIdx == i)
            {
                OnUnSelected(ItemList[i]);
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < ItemList.Count; i++)
        {
            if (ItemList[i] != null)
                ItemList[i].OnUpdate();
        }
    }

}
