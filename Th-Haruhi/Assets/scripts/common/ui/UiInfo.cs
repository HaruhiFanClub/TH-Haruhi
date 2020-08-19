
//////////////////////////////////////////////////////////////////////////
//
//   FileName : XUI_OpenStateMgr.cs
//     Author : Felon
// CreateTime : 2017-05-02
//       Desc : 窗口打开状态记忆管理
//
//////////////////////////////////////////////////////////////////////////

using System;

public enum UiLayer
{
    Main,
    MainTop,
    PopView,
    PopViewTop,
    Loding,
    Tips
}

public enum UiLoadType
{
    Single,     //单例
    Multi,      //多例
}

public class UiInfo
{
    public Type Type;
    public string ViewPath;
    public UiLayer Layer;
    public UiLoadType LoadType;
}
