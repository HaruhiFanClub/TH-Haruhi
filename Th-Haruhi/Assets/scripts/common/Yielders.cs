﻿
using UnityEngine;

public class Yielders
{
    public static WaitForEndOfFrame Frame = new WaitForEndOfFrame();
    public static WaitForFixedUpdate FixedFrame = new WaitForFixedUpdate();
    public static WaitForSeconds HalfSecond = new WaitForSeconds(0.5f);
    public static WaitForSeconds OneSecond = new WaitForSeconds(0.5f);
}