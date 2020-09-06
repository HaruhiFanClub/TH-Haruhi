
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetBundleNameProcess
{
    static readonly HashSet<string> resourceSet = new HashSet<string>();

    [MenuItem("Tools/Reset All AssetBundleName")]
    private static void ResetAllAssetBundleName()
    {
        string[] abNameArr = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < abNameArr.Length; i++)
        {
            EditorUtility.DisplayProgressBar("Remove All AssetBundleName", abNameArr[i], (float)i / abNameArr.Length);
            AssetDatabase.RemoveAssetBundleName(abNameArr[i], true);
        }
        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();

        //set abname
        var resourceList = ResourceBuildTool.GetBuildResources(PathUtility.FullPathToProjectPath(PathUtility.ResourcesPath));
        CollectionUtility.Insert(resourceSet, resourceList);

        if (!ResourcesBuilder.SetAssetsBundleName(resourceSet))
        {
            Debug.LogError("Error SetAssetsBundleName失敗");
        }
        EditorUtility.ClearProgressBar();
    }
}
