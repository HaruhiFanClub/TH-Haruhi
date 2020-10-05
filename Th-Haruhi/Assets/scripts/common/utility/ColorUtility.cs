
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
    None,
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
    public static Color Red = Color.red;
    public static Color Purple = new Color(0.86f, 0f, 1f, 1f);
    public static Color Blue = Color.blue;
    public static Color BlueLight = new Color(0.077f, 0.86f, 0.86f, 1f);
    public static Color Green = Color.green;
    public static Color Yellow = Color.yellow;
    public static Color Orange = new Color(1f, 0.5f, 0f, 1f);
    public static Color White = Color.white;

    public static Color GetColor(EColor color)
    {
        switch (color)
        {
            case EColor.Red:
                return Red;
            case EColor.Purple:
                return Purple;
            case EColor.Blue:
                return Blue;
            case EColor.BlueLight:
                return BlueLight;
            case EColor.Green:
                return Green;
            case EColor.Yellow:
                return Yellow;
            case EColor.Orange:
                return Orange;
            case EColor.White:
                return White;
        }
        return Color.red;
    }

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
