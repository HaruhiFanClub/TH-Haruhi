
using System;

public enum UiLayer
{
    Bg,
    Main,
    Tips,
    DontDestroy
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
