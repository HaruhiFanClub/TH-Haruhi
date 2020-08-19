
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class InversionDependenciesGraphUI : EditorWindow
{
	static InversionDependenciesGraphUI window;

    [MenuItem("Assets/Find This Asset In UI Prefab", false, 39)]
    public static void Open()
    {
        if (window == null)
        {
            window = GetWindow<InversionDependenciesGraphUI>(true, "Find This Asset In UI Prefab", true);
            if (!EditorPrefs.GetBool("first Find This Asset In UI Prefab"))
            {
                int initWidth = 300;
                int initHeight = 600;
                int x = (Screen.currentResolution.width - initWidth) / 2;
                int y = (Screen.currentResolution.height - initHeight) / 2;
                window.position = new Rect(x, y, initWidth, initHeight);
                EditorPrefs.SetBool("first Find This Asset In UI Prefab", true);
            }
        }
        else
            window.AnalyzeInUi();
    }


	[MenuItem("Assets/Inversion Dependencies Graph", true)]
	static bool OpenValidate()
	{
		Object _object = Selection.activeObject;
		if (_object == null)
			return false;
		if (AssetDatabase.IsSubAsset(_object))
			return false;
		string path = AssetDatabase.GetAssetPath(_object);
		if (System.IO.Directory.Exists(path))
			return false;
		return true;
	}

	List<Object> dependerList;
	Vector2 scrollpos;

	void Awake()
	{
		dependerList = new List<Object>();
	}

	void OnDestroy()
	{
		window = null;
	}

	void OnEnable()
    {
        AnalyzeInUi();
    }

	void OnDisable()
	{
		dependerList.Clear();
	}


    void AnalyzeInUi()
    {
        string depend = AssetDatabase.GetAssetPath(Selection.activeObject);
        string[] ids = AssetDatabase.FindAssets("t:prefab", new[] { "Assets/res/ui/prefabsx" });

		dependerList.Clear();

        for (int i = 0; i < ids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);
            EditorUtility.DisplayCancelableProgressBar("Find In UI", path, (float)i / ids.Length);
            string[] list = AssetDatabase.GetDependencies(new string[] { path });
            int index = System.Array.IndexOf(list, depend);
            if (index != -1)
                dependerList.Add(AssetDatabase.LoadMainAssetAtPath(path));
		}

        EditorUtility.ClearProgressBar();
        scrollpos = Vector2.zero;
        titleContent.text = "Dependers Of " + depend;
        Repaint();
    }

	void OnGUI()
	{
		scrollpos = EditorGUILayout.BeginScrollView(scrollpos);
		foreach (var t in dependerList)
		{
			string path = AssetDatabase.GetAssetPath(t);
			EditorGUILayout.BeginHorizontal();
			EditorGUIUtility.SetIconSize(new Vector2(16, 16));
			GUILayout.Label(AssetDatabase.GetCachedIcon(path));
			if (GUILayout.Button(Path.GetFileNameWithoutExtension(path), EditorStyles.label))
				Selection.activeObject = t;
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndScrollView();
	}
}