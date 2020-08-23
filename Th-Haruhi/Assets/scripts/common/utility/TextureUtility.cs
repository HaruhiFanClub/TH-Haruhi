using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;

public static class TextureUtility
{
    private static Dictionary<string, Texture2D> _texturesCache = new Dictionary<string, Texture2D>();

    public static IEnumerator LoadResourceById(int resourceId, Action<List<Sprite>> callBack)
    {
        var d = TableUtility.GetDeploy<TextureResourceDeploy>(resourceId);
        if (d == null) 
        {
            Debug.LogError("资源ID在表中找不到，ID:" + resourceId);
            yield break;
        }


        //找不到资源则加载并缓存
        Texture2D texture;
        if (!_texturesCache.TryGetValue(d.texture, out texture))
        {
            yield return LoadTexture(d.texture, tex => 
            {
                texture = tex;
            });
            _texturesCache[d.texture] = texture;
        }

        var sprites = LoadSpriteGroup(texture, d.startX, d.startY, d.width, d.height, d.xCount, d.yCount);
        callBack(sprites);
    }


    public static IEnumerator LoadTexture(string url, Action<Texture2D> callBack)
    {
        var path = PathUtility.ImagesPath + url;
        if (File.Exists(path))
        {
            yield return LoadExternalTexture(path, callBack);
        }
        else
        {
            var obj = ResourceMgr.LoadImmediately(url);
            callBack(obj as Texture2D);
        }
    }

    public static IEnumerator LoadExternalTexture(string url, Action<Texture2D> callBack)
    {
        var request = UnityWebRequest.Get(url);
        request.downloadHandler = new DownloadHandlerTexture();
        yield return request.SendWebRequest();

        if (request.isHttpError || request.isNetworkError)
        {
            Debug.LogError("LoadTextureError:" + request.error + " url:" + url);
        }
        else
        {
            var texture = DownloadHandlerTexture.GetContent(request);
            callBack(texture);
        }
    }

    public static List<Sprite> LoadSpriteGroup(Texture2D tex, int startX, int startY, int width, int height, int row, int column)
    {
        List<Sprite> list = new List<Sprite>();

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                var x = startX + i * width;
                var y = (tex.height - height) - startY - height * j;
                Texture2D tx2d = new Texture2D(width, height, TextureFormat.RGBA32, false);
                tx2d.SetPixels(tex.GetPixels(x, y, width, height));
                tx2d.Apply();
                var sprite = Sprite.Create(tx2d, new Rect(0, 0, tx2d.width, tx2d.height), new Vector2(0.5f, 0.5f), 100);
                list.Add(sprite);
            }
        }
        return list;
    }
}


public class TextureResourceDeploy : Conditionable
{
    public int id;
    public string texture;
    public int startX;
    public int startY;
    public int width;
    public int height;
    public int xCount;
    public int yCount;
}