
using System;
using System.Collections.Generic;
using UnityEngine;

public class UiRootScript : MonoBehaviour
{
    public Camera UiCamera;
    public RectTransform Bg;
    public RectTransform Battle;
    public RectTransform Main;
    public RectTransform Tips;
    public RectTransform DontDestroy;
    public UiBackGround BackGround;
    public UILoadingCompoent Loading;

    public Dictionary<UiLayer, RectTransform> CanvasParentList = new Dictionary<UiLayer, RectTransform>();
    public Dictionary<UiLayer, Canvas> CanvasList = new Dictionary<UiLayer, Canvas>();
    
    public void Init()
    {
        CanvasParentList[UiLayer.Bg] = Bg;
        CanvasParentList[UiLayer.Battle] = Battle;
        CanvasParentList[UiLayer.Main] = Main;
        CanvasParentList[UiLayer.Tips] = Tips;
        CanvasParentList[UiLayer.DontDestroy] = DontDestroy;
        CanvasList[UiLayer.Bg] = Bg.GetComponent<Canvas>();
        CanvasList[UiLayer.Battle] = Battle.GetComponent<Canvas>();
        CanvasList[UiLayer.Main] = Main.GetComponent<Canvas>(); 
        CanvasList[UiLayer.Tips] = Tips.GetComponent<Canvas>();
        CanvasList[UiLayer.DontDestroy] = DontDestroy.GetComponent<Canvas>();
    }
}