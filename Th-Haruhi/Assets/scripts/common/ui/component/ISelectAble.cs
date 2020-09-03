using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;


public interface ISelectAble 
{
    bool IsEnable { get; }
    bool InClick { get; }
    void SetSelect(bool b, bool fromAuto = false);
    void DoClick();
    void OnUpdate();

    RectTransform RectTransform { get; }
}