using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasMgr : MonoBehaviour
{

    // Use this for initialization
    void Awake()
    {
        SpriteAtlasManager.atlasRequested += OnAtlasRequested;
    }

    private void OnAtlasRequested(string t, Action<SpriteAtlas> action)
    {
        LoadSprite(t, action);
    }

    public static void ClearCache()
    {
        if (!GameSetting.CacheAtlas)
        {
            Debug.Log("SpriteAtlasMgr ClearCache!");
            var e = SpriteCache.GetEnumerator();
            using (e)
            {
                Resources.UnloadAsset(e.Current.Value);
            }
            SpriteCache.Clear();
        }
    }

    public static Dictionary<string, SpriteAtlas> SpriteCache = new Dictionary<string, SpriteAtlas>();
    public static void LoadSprite(string t, Action<SpriteAtlas> action)
    {

        //缓存有，直接取缓存中的
        SpriteAtlas spiritList;
        if (SpriteCache.TryGetValue(t, out spiritList))
        {
            action(spiritList);
            return;
        }

        //读取图集
        string fullPath = string.Format("atlas/{0}/{1}.spriteatlas", t, t);
        var obj = ResourceMgr.LoadImmediately(fullPath);
        SpriteAtlas sa = obj as SpriteAtlas;
        SpriteCache[t] = sa;
        action(sa);
    }
}
