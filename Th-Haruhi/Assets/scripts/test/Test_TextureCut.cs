using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_TextureCut : MonoBehaviour
{
    public Button Btn;
    public Transform Parent;
    public Text ParamX;
    public Text ParamY;
    public Text ParamW;
    public Text ParamH;
    public Text ParamR;
    public Text ParamC;
    public Text ParamUrl;

    private void Awake()
    {
        Btn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        for(int i = 0; i < _imageList.Count; i++)
        {
            DestroyImmediate(_imageList[i]);
        }
        _imageList.Clear();

        var x = int.Parse(ParamX.text);
        var y = int.Parse(ParamY.text);
        var w = int.Parse(ParamW.text);
        var h = int.Parse(ParamH.text);
        var r = int.Parse(ParamR.text);
        var c = int.Parse(ParamC.text);
        var url = ParamUrl.text;
        if(!string.IsNullOrEmpty(url))
        {
            StartCoroutine(PreView(url, x, y, w, h, r, c));
        }
    }

    private List<GameObject> _imageList = new List<GameObject>();

    private IEnumerator PreView(string url, int x, int y, int w, int h, int row, int column)
    {
        Texture2D texture = null;
        yield return TextureUtility.LoadTexture(url, tex =>
        {
            texture = tex;
        });
        yield return Yielders.Frame;

        var rawImage = GetComponentInChildren<RawImage>();
        rawImage.texture = texture;

        try
        {
            var list = TextureUtility.LoadSpriteGroup(texture, x, y, w, h, row, column);
            var startX = 0f;
            for (int i = 0; i < list.Count; i++)
            {
                var gameObj = new GameObject();
                var image = gameObj.AddComponent<Image>();
                image.GetComponent<RectTransform>().sizeDelta = new Vector2(w * 2, h * 2);
                image.sprite = list[i];
                gameObj.transform.SetParent(Parent, false);
                gameObj.transform.localPosition = new Vector3(startX, 0, 0);
                startX += w + 10;
                _imageList.Add(gameObj);
            }
        }
        catch(Exception e)
        {

        }
        
    }
}
