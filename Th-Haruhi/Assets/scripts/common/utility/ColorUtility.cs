
//////////////////////////////////////////////////////////////////////////
//
//   FileName : ColorUtility.cs
//     Author : Chiyer
// CreateTime : 2014-07-17
//       Desc :
//
//////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;

public enum EColor
{
    Red,
    Purple,
    Blue,
    BlueLight,
    Green,
    Yellow,
    Orange,
    White
}
public static class ColorUtility
{
    public static string FormatEasyColor(string str)
    {
        return string.Format("<color=#06CA00>{0}</color>", str);
    }
    public static string FormatNormalColor(string str)
    {
        return string.Format("<color=#006EFF>{0}</color>", str);
    }
    public static string FormatHardColor(string str)
    {
        return string.Format("<color=#FF2F39>{0}</color>", str);
    }
    public static string FormatLunaticColor(string str)
    {
        return string.Format("<color=#E000FF>{0}</color>", str);
    }

    public static Color Rgb(byte r, byte g, byte b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    public static Color RGBA(byte r, byte g, byte b, byte a)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    public static bool ParseHtmlString(string htmlString, out Color color)
    {
        return UnityEngine.ColorUtility.TryParseHtmlString(htmlString, out color);
    }

    public static string ToHtmlString(Color color)
    {
        return UnityEngine.ColorUtility.ToHtmlStringRGBA(color);
    }

}
