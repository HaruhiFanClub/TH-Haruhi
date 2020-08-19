
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

public static class ColorUtility
{
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
