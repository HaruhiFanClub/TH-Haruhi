
using System;

public enum UiLayer
{
    Bg,
    Main,
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
