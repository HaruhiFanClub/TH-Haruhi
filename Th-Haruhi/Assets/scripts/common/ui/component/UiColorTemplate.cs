using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

public class UiColorTemplate : MonoBehaviour
{
    public enum EImportantColor
    {
        Color140_207_42 = 0,
        Color231_52_56,
        Color247_125_14,
    }
    public List<Color> descColorList;
    public List<Color> rarityTextList;
    public List<Color> rarityImageList;
    public List<Color> importantColorList;
    public List<Color> rarityShaderColorList;

    public Color GetTextRarityColor(int rarity)
    {
        if (rarity <= 0 || rarity > rarityTextList.Count)
            return Color.white;

        return rarityTextList[rarity - 1];
    }

    public Color GetShaderRarityColor(int rarity)
    {
        if (rarity < 0 || rarity >= rarityShaderColorList.Count)
            return Color.white;

        return rarityShaderColorList[rarity - 1];
    }

    public Color GetImageRarityColor(int rarity)
    {
        if (rarity < 0 || rarity >= rarityImageList.Count)
            return Color.white;

        return rarityImageList[rarity];
    }

    public Color GetImportantColor(EImportantColor color)
    {
        int index = (int)color;
        if (index < 0 || index >= importantColorList.Count)
            return Color.white;

        return importantColorList[index];
    }
}