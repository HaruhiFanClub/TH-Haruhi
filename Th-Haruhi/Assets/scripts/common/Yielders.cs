﻿
using System.Collections;
using UnityEngine;

public class Yielders
{
    public static WaitForEndOfFrame Frame = new WaitForEndOfFrame();
    public static WaitForFixedUpdate FixedFrame = new WaitForFixedUpdate();
    public static WaitForSeconds HalfSecond = new WaitForSeconds(0.5f);
    public static WaitForSeconds OneSecond = new WaitForSeconds(1f);

    public static IEnumerator WaitFrame(int frameCount)
    {
        if (frameCount > 1) frameCount--;
        for (int j = 0; j < frameCount; j++)
        {
            yield return FixedFrame;
        }
    }
}