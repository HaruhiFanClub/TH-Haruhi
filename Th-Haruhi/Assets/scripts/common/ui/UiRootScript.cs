
using System;
using System.Collections.Generic;
using UnityEngine;

public class UiRootScript : MonoBehaviour
{
    public Camera UiCamera;
    public RectTransform Main;
    public RectTransform Loding;
    public RectTransform Tips;
    public UiBackGround BackGround;

    public Dictionary<UiLayer, RectTransform> CanvasParentList = new Dictionary<UiLayer, RectTransform>();
    public Dictionary<UiLayer, Canvas> CanvasList = new Dictionary<UiLayer, Canvas>();
    
    public void Init()
    {
        CanvasParentList[UiLayer.Main] = Main;
        CanvasParentList[UiLayer.Loding] = Loding;
        CanvasParentList[UiLayer.Tips] = Tips;

        CanvasList[UiLayer.Main] = Main.GetComponent<Canvas>(); 
        CanvasList[UiLayer.Loding] = Loding.GetComponent<Canvas>(); 
        CanvasList[UiLayer.Tips] = Tips.GetComponent<Canvas>(); 
    }
}