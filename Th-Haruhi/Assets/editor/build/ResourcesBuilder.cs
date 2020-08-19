

#define BUILDER_OPT
#define BUILDER_DIFF

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public class ModelMatTool : AssetPostprocessor
{
    /*
    private void OnPostprocessModel(GameObject model)
    {
        string path = assetPath.ToLower();
        if (path.EndsWith(".fbx"))
        {
            Debug.LogWarning("process fbx " + path);
            Renderer[] renderComs = model.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderComs.Length; i++)
            {
                renderComs[i].sharedMaterial = null;

                if (renderComs[i].sharedMaterials != null)
                {
                    renderComs[i].sharedMaterials = new Material[renderComs[i].sharedMaterials.Length];
                }
            }

            List<AnimationClip> clips = new List<AnimationClip>(AnimationUtility.GetAnimationClips(model));

            if (clips.Count == 0)
            {
                AnimationClip[] objectList = UnityEngine.Object.FindObjectsOfType(typeof(AnimationClip)) as AnimationClip[];
                if (objectList != null)
                {
                    clips.AddRange(objectList);
                }
            }

            for (int i = 0; i < clips.Count; i++)
            {
                CompressAnim(clips[i]);
            }
        }
    }
    */
    public const string ScaleKeyName = "localscale";
    public static void CompressAnim(AnimationClip clip)
    {
        //ReduceScaleKey(clip, ScaleKeyName);
        ReduceFloatPrecision(clip);
    }

    public static void CompressAnim2(AnimationClip clip)
    {
        ReduceScaleKey(clip, ScaleKeyName);
        ReduceFloatPrecision(clip,"f2");
    }
    private static void ReduceScaleKey(AnimationClip clip, string keyName)
    {
        EditorCurveBinding[] curves = AnimationUtility.GetCurveBindings(clip);

        for (int j = 0; j < curves.Length; j++)
        {
            EditorCurveBinding curveBinding = curves[j];

            if (curveBinding.propertyName.ToLower().Contains(keyName))
            {
                AnimationUtility.SetEditorCurve(clip, curveBinding, null);
            }
        }
    }
    public static void ReduceFloatPrecision(AnimationClip clip, string str = "f3")
    {
        EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip);

        for (int j = 0; j < bindings.Length; j++)
        {
            EditorCurveBinding curveBinding = bindings[j];
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, curveBinding);

            if (curve == null || curve.keys == null)
            {
                continue;
            }

            Keyframe[] keys = curve.keys;
            for (int k = 0; k < keys.Length; k++)
            {
                Keyframe key = keys[k];
                key.value = float.Parse(key.value.ToString(str));
                key.inTangent = float.Parse(key.inTangent.ToString(str));
                key.outTangent = float.Parse(key.outTangent.ToString(str));
                keys[k] = key;
            }
            curve.keys = keys;

            AnimationUtility.SetEditorCurve(clip, curveBinding, curve);
        }
    }
}

public static class ResourcesBuilder
{
    static string buildPath;
    static BuildTarget buildTarget;
    static BuildAssetBundleOptions buildAssetOptions;
    static readonly HashSet<string> resourceSet = new HashSet<string>();

    public static bool Build(string _buildPath, BuildTarget target, List<string> resourceList)
    {
        //var resourceList = ResourceBuildTool.GetBuildResources(PathUtility.FullPathToProjectPath(PathUtility.ResourcesPath));

        if (string.IsNullOrEmpty(_buildPath)) return false;
        if(resourceList.Count == 0) return false;

        AssetDatabase.SaveAssets();

        buildPath = _buildPath;

        if (!Directory.Exists(buildPath)) Directory.CreateDirectory(buildPath);

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
        AssetBundleManifest assetManifest = BuildPipeline.BuildAssetBundles(buildPath, buildAssetOptions, buildTarget);

        if (assetManifest != null)
        {
            if (File.Exists(buildPath + "/StreamingAssets"))
            {
                FileInfo file = new FileInfo(buildPath + "/StreamingAssets");
                string path = buildPath + "/StreamingAssets.haruhi";
                if (File.Exists(path))
                    File.Delete(path);
                file.MoveTo(path);
            }
        }
        else
        {
            Debug.LogError("Error assetManifest is null, buildPath:" + buildPath + " buildAssetOptions:" +
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
        foreach (string path in list)
        {
            string text = FileUtility.GetTextFromFile(PathUtility.ProjectPathToFullPath(path));
            if (path.ToLower().EndsWith(".sos"))
            {
                text = TableDatabase.RemoveComment(text);
            }

            var outpath = PathUtility.AssetBundlePath + "/" + PathUtility.GetUniquePathByProjectPath(path) + ".haruhi";
            string oldText = "";
            if(File.Exists(outpath))
                oldText = FileUtility.GetTextFromFile(outpath);
            if (text != oldText)
            {
                MemoryStream ms = new MemoryStream();
                using (StreamWriter writer = new StreamWriter(ms,new System.Text.UTF8Encoding(true)))
                {
                    writer.Write(text);
                    writer.Close();
                }
                var buff = ms.ToArray();
                if (path.ToLower().EndsWith(".sos"))
                {
                    buff = FileUtility.CopyFrom(buff, 3);                //保存资源加密
                }
                File.Delete(outpath);
                FileStream fs = new FileStream(outpath, FileMode.OpenOrCreate);
                fs.Write(buff, 0, buff.Length);
                fs.Close();
            }
        }
        Debug.LogWarning("build text ok. " + list.Count + " items.");
    }
    
}

