

#define BUILDER_OPT
#define BUILDER_DIFF

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public static class ResourcesBuilder
{
    static string _assetPath;
    static BuildTarget buildTarget;
    static BuildAssetBundleOptions buildAssetOptions;
    static readonly HashSet<string> resourceSet = new HashSet<string>();

    public static bool Build(string assetPath, BuildTarget target, List<string> resourceList)
    {
        //var resourceList = ResourceBuildTool.GetBuildResources(PathUtility.FullPathToProjectPath(PathUtility.ResourcesPath));

        if (string.IsNullOrEmpty(assetPath)) return false;
        if(resourceList.Count == 0) return false;

        AssetDatabase.SaveAssets();

        _assetPath = assetPath;

        if (!Directory.Exists(_assetPath)) 
            Directory.CreateDirectory(_assetPath);


        buildTarget = target;

        //lz4 格式 压缩
        buildAssetOptions = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle;

        /*
        string[] abNameArr = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < abNameArr.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(abNameArr[i], true);
        }*/
        AssetDatabase.Refresh();
       
        CollectionUtility.Insert(resourceSet, resourceList);

        if (!SetAssetsBundleName(resourceSet))
        {
            EditorUtility.ClearProgressBar();
            Debug.LogError("SetAssetsBundleName Error");
            return false;
        }

        EditorUtility.ClearProgressBar();
        AssetBundleManifest assetManifest = BuildPipeline.BuildAssetBundles(_assetPath, buildAssetOptions, buildTarget);

        if (assetManifest != null)
        {
            if (File.Exists(_assetPath + "/StreamingAssets"))
            {
                FileInfo file = new FileInfo(_assetPath + "/StreamingAssets");
                string path = _assetPath + "/StreamingAssets.haruhi";
                if (File.Exists(path))
                    File.Delete(path);
                file.MoveTo(path);
            }
        }
        else
        {
            Debug.LogError("Error assetManifest is null, buildPath:" + _assetPath + " buildAssetOptions:" +
                                        buildAssetOptions + " buildTarget:" + buildTarget);

        }
		
        GC.Collect();
        return true;
    }

    private static void ShowProgressBar(string str1, string strInfo, float progress)
    {
        bool bCancel = EditorUtility.DisplayCancelableProgressBar(str1, strInfo, progress);
        if (bCancel)
        {
            EditorUtility.ClearProgressBar();
            throw new Exception("User break!");
        }
    }

    private static bool SetAssetsBundleName(HashSet<string> rList)
    {
        int step = 0;

        //step 1: 遍历所有依赖, 缓存资源被依赖次数
        var dependCount = new Dictionary<string, int>();
        var dependResourceList = new Dictionary<string, List<string>>();
        foreach (string resource in rList)
        {
            var dependenies = AssetDatabase.GetDependencies(resource, true);
            for (int i = 0; i < dependenies.Length; i++)
            {
                var rName = dependenies[i];
                if (!dependCount.ContainsKey(rName))
                {
                    dependCount[rName] = 1;
                    dependResourceList[rName] = new List<string>();
                }
                else
                {
                    dependCount[rName]++;
                }
                dependResourceList[rName].Add(resource);

            }
            ++step;
            ShowProgressBar("Get DependFileList", resource, step / (float)rList.Count);
        }

        
        step = 0;
        var e = dependCount.GetEnumerator();
        using (e)
        {
            while (e.MoveNext())
            {
                //Debug.Log("resource:" + e.Current.Key + "  count:" + e.Current.Value);

                var resource = e.Current.Key;
                ResourceType type = ResourcesUtility.GetResourceTypeByPath(e.Current.Key);
                var importer = AssetImporter.GetAtPath(resource);
                if (importer == null)
                {
                    Debug.LogError("Error Resource importer is Null. --> " + resource);
                    return false;
                }
                switch (type)
                {
                    case ResourceType.texture:
                        TextureImporter textureImporter = importer as TextureImporter;
                        var isUiSprite = resource.Contains("ui/textures");
                        if (isUiSprite && textureImporter != null && textureImporter.textureType == TextureImporterType.Sprite)
                        {
                            importer.assetBundleName = "";
                        }
                        else
                        {
                            importer.assetBundleName = PathUtility.GetAbName(resource);
                        }
                        break;
                    case ResourceType.scene:
                    case ResourceType.prefab:
                    case ResourceType.fbx:
                    case ResourceType.material:
                    case ResourceType.physicMaterial:
                    case ResourceType.audio:
                    case ResourceType.controller:
                    case ResourceType.font:
                    case ResourceType.mask:
                    case ResourceType.anim:
                    case ResourceType.mp4:
                    case ResourceType.shadervariants:
                    case ResourceType.speedTree:
                    case ResourceType.terrainLayer:
                    case ResourceType.flare:
                    case ResourceType.sbsar:
                    case ResourceType.bytes:
                    case ResourceType.asset: // 光照贴图的ab名称需与unity场景文件ab名称一致
                    case ResourceType.spriteatlas:
                        importer.assetBundleName = PathUtility.GetAbName(resource);
                        break;
                    case ResourceType.shader:
                        importer.assetBundleName = PathUtility.GetAbName(resource);
                        break;
                    case ResourceType.table:
                    case ResourceType.config:
                    case ResourceType.script:
                    case ResourceType.folder:
                        break;
                    default:

                        importer.assetBundleName = PathUtility.GetAbName(resource);
                        Debug.LogError("unknow depend resource : " + resource + " type:" + type);
                        break;
                }
                ++step;
                ShowProgressBar("Set Resource AssetBundleName", resource, step / (float)dependCount.Count);
            }
        }

        return true;
    }

    public static void BuildText()
    {
        List<string> list = new List<string>();
        list.AddRange(Directory.GetFiles("Assets/", "*.sos", SearchOption.AllDirectories));
        list.AddRange(Directory.GetFiles("Assets/", "*.cfg", SearchOption.AllDirectories));


        var p = PathUtility.AssetBundlePath + "/tables/";
        if (!Directory.Exists(p))
            Directory.CreateDirectory(p);

        foreach (string path in list)
        {
            string text = FileUtility.GetTextFromFile(PathUtility.ProjectPathToFullPath(path)) + ".haruhi_table";
            /* RemoveComment
            if (path.ToLower().EndsWith(".sos"))
            {
                text = TableDatabase.RemoveComment(text);
            }*/

            var outpath = p + PathUtility.GetUniquePathByProjectPath(path);

            string oldText = "";
            if (File.Exists(outpath))
            {
                oldText = FileUtility.GetTextFromFile(outpath);
            }
            if (text != oldText)
            {
                MemoryStream ms = new MemoryStream();
                using (StreamWriter writer = new StreamWriter(ms,new System.Text.UTF8Encoding(true)))
                {
                    writer.Write(text);
                    writer.Close();
                }
                var buff = ms.ToArray();
                
                /*
                if (path.ToLower().EndsWith(".sos"))
                {
                    buff = FileUtility.CopyFrom(buff, 3);                //保存资源加密
                }
                */

                File.Delete(outpath);
                FileStream fs = new FileStream(outpath, FileMode.OpenOrCreate);
                fs.Write(buff, 0, buff.Length);
                fs.Close();
            }
        }
        Debug.LogWarning("build text ok. " + list.Count + " items.");
    }
    
}

