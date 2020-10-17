
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleComponent : MonoBehaviour
{
    public UiText Difficult;
    public Outline DifficultOutLine;
    public List<GameObject> LifeList;
    public List<GameObject> SpellList;

    public UiText MaxScore;
    public UiText CurScore;
    public UiText Power;
    public UiText Point;
    public UiText Graze;

    public RectTransform LeftTimeRoot;
    public UiText LeftTimeSec1;
    public UiText LeftTimeSec2;
    public RectTransform EnemyMark;

    public RectTransform CardNameRoot;
    public CanvasGroup CardNameAlpha;
    public UiText CardName;

    public RectTransform CardBonusRoot;
    public CanvasGroup CardBonusAlpha;
    public UiText CardBonusScore;
    public UiText CardBonusHistory;


    //debug
    public UiButton DebugWudi;
    public UiButton DebugSlow;
    public UiButton DebugTest;
}
