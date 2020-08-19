
using System;
using System.Collections.Generic;
using UnityEngine;

public class UiRootScript : MonoBehaviour
{
    public Camera UiCamera;

    public RectTransform Main;
    public RectTransform MainTop;
    public RectTransform PopView;
    public RectTransform PopViewTop;
    public RectTransform Loding;
    public RectTransform Tips;
    
    public Canvas MainCanvas;
    public Canvas MainTopCanvas;
    public Canvas PopViewCanvas;
    public Canvas PopViewTopCanvas;
    public Canvas LodingCanvas;
    public Canvas TipsCanvas;

    public Dictionary<UiLayer, RectTransform> CanvasParentList = new Dictionary<UiLayer, RectTransform>();
    public Dictionary<UiLayer, Canvas> CanvasList = new Dictionary<UiLayer, Canvas>();
    
    public void Init()
    {
        CanvasParentList[UiLayer.Main] = Main;
        CanvasParentList[UiLayer.MainTop] = MainTop;
        CanvasParentList[UiLayer.PopView] = PopView;
        CanvasParentList[UiLayer.PopViewTop] = PopViewTop;
        CanvasParentList[UiLayer.Loding] = Loding;
        CanvasParentList[UiLayer.Tips] = Tips;
        
        CanvasList[UiLayer.Main] = MainCanvas;
        CanvasList[UiLayer.MainTop] = MainTopCanvas;
        CanvasList[UiLayer.PopView] = PopViewCanvas;
        CanvasList[UiLayer.PopViewTop] = PopViewTopCanvas;
        CanvasList[UiLayer.Loding] = LodingCanvas;
        CanvasList[UiLayer.Tips] = TipsCanvas;
    }
}