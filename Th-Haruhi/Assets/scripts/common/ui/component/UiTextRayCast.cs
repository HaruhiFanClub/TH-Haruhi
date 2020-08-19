//#define REBUILD_TEST
using System;
using UnityEngine.UI;
using UnityEngine;

public class UiTextRayCast : UiText
{
    protected override void Awake()
    {
        base.Awake();
        raycastTarget = true;
    }
}
