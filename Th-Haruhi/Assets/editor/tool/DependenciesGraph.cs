
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class DependenciesGraph : EditorWindow
{
    static DependenciesGraph window;

    [MenuItem("Assets/Dependencies Graph", false, 38)]
    public static void Open()
    {
        if (!Selection.activeObject)
            return;
        if (Selection.activeObject.GetType() == typeof(UnityEditor.DefaultAsset))
            return;
        
        if (window == null)
        {
            window =
                EditorWindow.GetWindow<DependenciesGraph>(true, "Dependencies Graph", true);
            if (!EditorPrefs.GetBool("firstOpenDependenciesGraph"))
            {
                int initWidth = 600;
                int initHeight = 600;
                int x = (Screen.currentResolution.width - initWidth) / 2;
                int y = (Screen.currentResolution.height - initHeight) / 2;
                window.position = new Rect(x, y, initWidth, initHeight);
                EditorPrefs.SetBool("firstOpenDependenciesGraph", true);
            }
        }
        else
            window.Analyze();
    }

    Vector2 scrollpos;

    class Depender
    {
        public string path;
        public string name;
        public Object _object;
        public Depender parent;
        public List<Depender> _subList;
        public bool on;
        public bool circulation;
    }

    Depender dependerRoot;
    Dictionary<string, string[]> dependCache;

    void Awake()
    {
    }

    void OnDestroy()
    {
        window = null;
    }

    void OnEnable()
    {
        Analyze();  
    }

    void OnDisable()
    {
    }

    string[] GetDirectDependencies(string path)
    {
        string[] _list;
        if (dependCache.TryGetValue(path, out _list))
            return _list;
        _list = AssetDatabase.GetDependencies(path, false);
        dependCache.Add(path, _list);
        return _list;
    }

    void Analyze()
    {
        dependCache = new Dictionary<string, string[]>();
        dependerRoot = GenDepender(AssetDatabase.GetAssetPath(Selection.activeObject));
        scrollpos = Vector2.zero;
        Repaint();
    }

    Depender GenDepender(string path, Depender parent = null)
    {
        Depender depender = new Depender();

        depender.path = path;
        depender.name = Path.GetFileNameWithoutExtension(path);
        depender.parent = parent;
        depender._object = AssetDatabase.LoadMainAssetAtPath(path);
        depender._subList = new List<Depender>();
        depender.on = true;

        if (IsCirculation(path, parent))
            depender.circulation = true;
        else
        {
            string[] dependList = GetDirectDependencies(path);
            for (int i = 0; i < dependList.Length; ++i)
                depender._subList.Add(GenDepender(dependList[i], depender));
        }

        return depender;
    }

    void OnGUI()
    {
        scrollpos = EditorGUILayout.BeginScrollView(scrollpos);
        DoDependerGUI(dependerRoot);
        EditorGUILayout.EndScrollView();
    }

    void DoDependerGUI(Depender depender, int indent = 0)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(indent * 16);
        if (depender._subList.Count > 0)
            depender.on = GUILayout.Toggle(depender.on, "", EditorStyles.foldout, GUILayout.Width(10f));
        EditorGUIUtility.SetIconSize(new Vector2(16, 16));
        GUILayout.Label(AssetDatabase.GetCachedIcon(depender.path));
        if (GUILayout.Button(depender.name, EditorStyles.label))
            Selection.activeObject = depender._object;
        if (depender.circulation)
        {
            GUILayout.Space(16);
            GUI.color = Color.red;
            GUILayout.Label("circular reference");
            GUI.color = Color.white;
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        if (depender.on)
            for (int i = 0; i < depender._subList.Count; ++i)
                DoDependerGUI(depender._subList[i], indent + 1);
    }

    bool IsCirculation(string path, Depender parent)
    {
        while (parent != null)
        {
            if (string.Compare(path, parent.path, true) == 0)
                return true;
            parent = parent.parent;
        }

        return false;
    }
}
