
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;

public class EditorTools
{

    [MenuItem("Assets/Copy Full Path ", false, 15)]
    public static void CopyPath()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        EditorGUIUtility.systemCopyBuffer = path;
    }

    [MenuItem("Assets/Copy Resource Path", false, 16)]
    public static void CopyResourcePath()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        string resPath = "Assets/res/";
        if (path.StartsWith(resPath))
            path = path.Remove(0, resPath.Length);
        EditorGUIUtility.systemCopyBuffer = path;
    }

    [MenuItem("Assets/Copy All Resource Path", false, 17)]
    public static void CopyAllResourcePath()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        string fullpath = PathUtility.ProjectPathToFullPath(path);
        if (Directory.Exists(fullpath))
        {
            string[] paths = Directory.GetFiles(fullpath, "*", SearchOption.TopDirectoryOnly);
            string allpath = string.Empty;
            for (int i = 0; i < paths.Length; ++i)
            {
                if (Path.GetExtension(paths[i]) == ".meta")
                    continue;
                allpath += PathUtility.FullPathToResourcePath(paths[i]);
                if (i < paths.Length - 1)
                    allpath += "\r\n";
            }
            EditorGUIUtility.systemCopyBuffer = allpath;
        }
    }

    static void DoDirty(GameObject gameObject)
    {
        EditorUtility.SetDirty(gameObject);
        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null)
        {
            EditorSceneManager.MarkSceneDirty(prefabStage.scene);
        }
    }
}