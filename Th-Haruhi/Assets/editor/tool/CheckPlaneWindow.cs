

using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CheckPlaneWindow : EditorWindow
{
	private static EditorWindow _window;

    private static List<string> AllPrefabPathList
    {
        get
        {
            List<string> ids = new List<string>();
            var aaa = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/res" });
            foreach (var a in aaa)
            {
                string path = AssetDatabase.GUIDToAssetPath(a);
                ids.Add(path);
            }

            return ids;
        }
    }

	[MenuItem("Haruhi/自动检查工具")]
	private static void Init()
	{
		_window = GetWindow(typeof(CheckPlaneWindow), true, "CheckPlane", true);
		_window.Show();
	}

	private void OnDestroy()
	{
		_window = null;
	}
    private void OnGUI()
	{
        if (GUILayout.Button("检查并删除所有预制体中的空引用"))
		{
			FindAndDelAllPrefabMissingComp();
		}

		if (GUILayout.Button("检查所有的预制体的MeshCollider"))
		{
			var count = FindAllPrefabsMeshCollider();
			ShowFinishDialog("共检查预制体：" + AllPrefabPathList.Count + "    找到MeshCollider：" + count +
			                 "详细信息请留意控制台并手动处理！");
		}

		if (GUILayout.Button("检查所有的预制体的BoxCollider"))
		{
			var count = CheckAllBoxCollider();
			ShowFinishDialog("共检查预制体：" + AllPrefabPathList.Count + "    找到需要处理的BoxCollider：" + count +
			                 "详细信息请留意控制台并手动处理！");
		}

		if (GUILayout.Button("删除所有空状态机"))
		{
			var count = FindAllAnimatorController();
			ShowFinishDialog("共检查预制体：" + AllPrefabPathList.Count + "    找到空状态机：" + count +
			                 "详细信息请留意控制台并手动处理！");
		}

		if (GUILayout.Button("去掉所有Fbx的默认Material"))
		{
			ProcessFbxMaterial();
		}
        if (GUILayout.Button("检查所有ui中状态机的引用"))
		{
			FindAllAnimatorInUIPrefabs();
		}
	    if (GUILayout.Button("替换默认的Shader，mesh(prefab)"))
	    {
	        ClearSpriteTagName();
            RepleceDefaultBulidinResource();
	    }
	    if (GUILayout.Button("替换默认的Shader，mesh(InScene)"))
	    {
	        RepleceDefaultBulidinResource(true);
	        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
        if (GUILayout.Button("检查GridLayoutGroup"))
	    {
	        CheckGridLayoutGroup();
	    }
	    if (GUILayout.Button("检查Text的BestFit"))
	    {
	        CheckTextBestFit();
	    }
	    if (GUILayout.Button("检查UI的Mask(因为会造成Build)"))
	    {
	        CheckUIMask();
	    }
	    if (GUILayout.Button("检查UI的FillCenter"))
	    {
	        CheckUIFillCenter();
	    }
        if (GUILayout.Button("检查Image的Tiled"))
	    {
	        CheckImageTiled();
	    }
        if (GUILayout.Button("检查自带Rigidbody的Prefab"))
	    {
	        CheckRigidBody();
	    }
	    if (GUILayout.Button("删除不用的Material"))
	    {
	        DelMaterialTextureModel();
	    }
	    if (GUILayout.Button("删除不用Prefab(需要代码制定目录)"))
	    {
	        DelPrefab();
	    }

	    if (GUILayout.Button("检查不符合规范的粒子特效"))
	    {
	        CheckParticleSystem();

	    }
	    if (GUILayout.Button("检查Static"))
	    {
	        CheckStatic();

	    }
	    if (GUILayout.Button("检查图片格式，是否为2次幂"))
	    {
	        CheckTextureSize();
	    }
	    if (GUILayout.Button("设置GpuInstance"))
	    {
	        SetMaterialGpuInstance();
	    }
        if (GUILayout.Button("Test"))
        {
            //Test();
            ControlSceneRoot();
        }
    }

    private static void Test()
    {
        SetLayer();}

    private static void SetLayer()
    {
        string[] ids = AssetDatabase.FindAssets("t:prefab", new[] { "Assets/res" });
        for (int i = 0; i < ids.Length; i++)
        {
            EditorUtility.DisplayProgressBar("SetLayer", "", (float)i / ids.Length);
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);
            GameObject mainObj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (!mainObj) return;

            Transform[] ts = mainObj.GetComponentsInChildren<Transform>(true);
            if (ts != null)
            {
                foreach (var t in ts)
                {
                   
                }
            }
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
    }

    private static void SetMaterialGpuInstance()
    {
        string[] ids = AssetDatabase.FindAssets("t:material", new string[] { "Assets/res" });
        for (int i = 0; i < ids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);
            Material mainObj = AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material;
            if (!mainObj) return;
            EditorUtility.SetDirty(mainObj);
            mainObj.enableInstancing = true;
        }
        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }

    private static void ControlSceneRoot()
    {
        
        var gameOjbs = Resources.FindObjectsOfTypeAll<GameObject>();
        Dictionary<string, List<GameObject>> dic = new Dictionary<string, List<GameObject>>();
        foreach (var mainObj in gameOjbs)
        {
            if(!mainObj) continue;
            if(!PrefabUtility.IsAnyPrefabInstanceRoot(mainObj)) continue;
            if (!dic.ContainsKey(mainObj.name))
            {
                dic[mainObj.name] = new List<GameObject>();
            }
            dic[mainObj.name].Add(mainObj);
        }

        int count = 0;
        var e = dic.GetEnumerator();
        using (e)
        {
            while (e.MoveNext())
            {
                var obj = new GameObject();
                obj.name = e.Current.Key;
                for (int j = 0; j < e.Current.Value.Count; j++)
                {
                    count++;
                    EditorUtility.DisplayProgressBar("processing", obj.name, (float)count / gameOjbs.Length);
                    e.Current.Value[j].transform.SetParent(obj.transform);
                }
            }
        }
        EditorUtility.ClearProgressBar();
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }


    private static void CheckTextureSize()
    {
        string path = "Assets/";
        var allfiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith("psd") || s.EndsWith("tga") || s.EndsWith("png") || s.EndsWith("jpg") || s.EndsWith("bmp") || s.EndsWith("tif") || s.EndsWith("gif")).ToArray();
        int count = 0;
        foreach (var item in allfiles)
        {
            count++;
            EditorUtility.DisplayProgressBar("检查prefab 1", "", (float)count / allfiles.Length);
            Texture t = AssetDatabase.LoadAssetAtPath<Texture>(item);
            if(!t) continue;
            if (!Get2Flag(t.width) || !Get2Flag(t.height))
            {
                Debug.LogError("图片错误，必须为2次幂："+item+ " width:"+ t.width + " height:"+ t.height);
            }
        }
        EditorUtility.ClearProgressBar();
    }

    private static bool Get2Flag(int num)
    {
        if (num < 1) return false;
        return (num & num - 1) == 0;
    }

    private static void CheckStatic()
    {
        int count = 0;
        string[] ids = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/res" });
        for (int i = 0; i < ids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);
            GameObject mainObj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (!mainObj) return;
            if (mainObj.isStatic)
            {
                EditorUtility.SetDirty(mainObj);
                mainObj.isStatic = false;
                Debug.LogError("使用了Static:" + mainObj.name);
                count++;
            }

            
            var objects = mainObj.GetComponentsInChildren<Component>(true);
            foreach (var o in objects)
            {
                if(o.gameObject == null) continue;
                if (o.gameObject.isStatic)
                {
                    EditorUtility.SetDirty(o.gameObject);
                    o.gameObject.isStatic = false;
                    Debug.LogError("使用了Static:" + mainObj.name);
                    count++;
                }
            }
        }
        AssetDatabase.SaveAssets();
    }

    public static void CheckParticleSystem()
    {
        string[] ids = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/res/effects" });
        for (int i = 0; i < ids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);
            GameObject mainObj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (!mainObj) return;
           
            //检查粒子数量
            var pCount = mainObj.GetComponentsInChildren<ParticleSystem>();
            if (pCount.Length > 8)
            {
                Debug.LogError("##粒子，不得大于8个, 当前:"+pCount.Length + " "+path);
            }

            bool bSize = false;
            //检查贴图数量
            var mtarials = mainObj.GetComponentsInChildren<Renderer>();
            List<string> ms = new List<string>();
            int count = 0;
            foreach (var m in mtarials)
            {
                if (m.sharedMaterials != null && m.sharedMaterials.Length > 0)
                {
                    foreach (var mm in m.sharedMaterials)
                    {
                        if (mm != null && mm.mainTexture != null)
                        {
                            if (!ms.Contains(mm.mainTexture.name))
                            {
                                count++;
                                ms.Add(mm.mainTexture.name);
                                if (mm.mainTexture.width > 512 || mm.mainTexture.height > 512)
                                {
                                    bSize = true;
                                }

                            }
                        }
                    }
                }
                
                if(m.sharedMaterial == null || m.sharedMaterial.mainTexture == null) continue;
                if (!ms.Contains(m.sharedMaterial.mainTexture.name))
                {
                    count++;
                    ms.Add(m.sharedMaterial.mainTexture.name);

                    if (m.sharedMaterial.mainTexture.width > 512 || m.sharedMaterial.mainTexture.width > 512)
                    {
                        bSize = true;
                    }

                }
            }
            if (count > 3)
            {
                Debug.LogError("特效贴图超过限制，不得大于3张 当前:" + count + " "+ path);
            }

            if (bSize)
            {
                Debug.LogError("特效贴图尺寸错误，存在>512 " + path);
            }

        }
    }

    private static List<string> allDependList = new List<string>();
    private static void DelPrefab()
    {
        allDependList.Clear();


        string[] pathList = AssetDatabase.GetAllAssetPaths();
        for (int i = 0; i < pathList.Length; ++i)
        {
            EditorUtility.DisplayProgressBar("检查prefab 1", pathList[i], (float)i / pathList.Length);
            string path = pathList[i];
            if (Path.GetExtension(path) == ".prefab" || Path.GetExtension(path) == ".unity")
            {
                string[] _list = AssetDatabase.GetDependencies(new string[] { path });
                allDependList.AddRange(_list);
            }
        }
  
        
        var findRoot = new[] { "Assets/res/scenes/stage1/PolygonFarm" };
        string[] ids = AssetDatabase.FindAssets("t:prefab", findRoot);
        List<string> notUsePrefab = new List<string>();
        for (int i = 0; i < ids.Length; i++)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(ids[i]);

            EditorUtility.DisplayProgressBar("检查prefab 2", prefabPath, (float)i / ids.Length);

            int count = 0;
            bool bFind = false;
            foreach (var s in allDependList)
            {
                if (s == prefabPath)
                {
                    count++;
                    if (count > 1)
                    {
                        bFind = true;
                        Debug.Log("Find  AllDependList的Prefab：" + prefabPath);
                        break;
                    }
                }
                
            }
            
            if(bFind) continue;
            var newPath = prefabPath.Replace("Assets/res/", "");
            if(!FindPrefabInTab(newPath) && !FindPrefabInCs(newPath))
            {
                notUsePrefab.Add(prefabPath);
            }
        }

        for (int i = 0; i < notUsePrefab.Count; i++)
        {
            Debug.LogError("删除未使用Prefab:"+notUsePrefab[i]);
            AssetDatabase.DeleteAsset(notUsePrefab[i]);
        }
        EditorUtility.ClearProgressBar();
    }

  
    private static bool FindPrefabInTab(string newPath)
    {
        string path = "Assets/tables";
        var allfiles = Directory.GetFiles(path, "*.sos", SearchOption.AllDirectories);
        bool bFind = false;
        foreach (var tab in allfiles)
        {
            var str = File.ReadAllText(tab);
            if (str.Contains(newPath))
            {
                bFind = true;
                Debug.Log("FindPrefabInTab的Prefab：" + newPath);
                break;
            }
        }

        return bFind;
    }
    private static bool FindPrefabInCs(string newPath)
    {
        string path = "Assets/scripts";
        var allfiles = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
        bool bFind = false;
        foreach (var tab in allfiles)
        {
            var str = File.ReadAllText(tab);
            if (str.Contains(newPath))
            {
                bFind = true;
                Debug.Log("FindPrefabInCs的Prefab：" + newPath);
                break;
            }
        }

        return bFind;
    }
    private static void DelMaterialTextureModel()
    {
        allDependList.Clear();


        string[] pathList = AssetDatabase.GetAllAssetPaths();
        for (int i = 0; i < pathList.Length; ++i)
        {
            EditorUtility.DisplayProgressBar("DelMaterialTextureModel 1", pathList[i], (float)i / pathList.Length);
            string path = pathList[i];
            if (Path.GetExtension(path) == ".prefab")// || Path.GetExtension(path) == ".unity")
            {
                string[] _list = AssetDatabase.GetDependencies(new string[] { path });
                allDependList.AddRange(_list);
            }
        }


        var findRoot = new[] { "Assets/res/scenes/stage1/PolygonFarm" };

        string[] ids = AssetDatabase.FindAssets("t:prefab", new[] { "Assets" });
        List<string> materialsStr = new List<string>();
        List<string> textureStr = new List<string>();
        List<string> modelsStr = new List<string>();
        List<string> animators = new List<string>();
        List<string> animations = new List<string>();
        for (int i = 0; i < ids.Length; i++)
        {
            
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);
            EditorUtility.DisplayProgressBar("DelMaterialTextureModel 2", path, (float)i / ids.Length);
            GameObject mainObj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (!mainObj) return;

            var roots = new UnityEngine.Object[] { mainObj };
            var dependObjs = EditorUtility.CollectDependencies(roots);
            foreach (var obj in dependObjs)
            {
                if (obj is Texture2D)
                {
                    string texturePath = AssetDatabase.GetAssetPath(obj.GetInstanceID());
                    if (!textureStr.Contains(texturePath))
                        textureStr.Add(texturePath);
                }
                else if (obj is Material)
                {
                    string texturePath = AssetDatabase.GetAssetPath(obj.GetInstanceID());
                    if (!materialsStr.Contains(texturePath))
                        materialsStr.Add(texturePath);
                }
                else if (obj is Mesh || obj is MeshFilter)
                {
                    string texturePath = AssetDatabase.GetAssetPath(obj.GetInstanceID());
                    if (!modelsStr.Contains(texturePath))
                        modelsStr.Add(texturePath);
                }
                else if (obj is RuntimeAnimatorController)
                {
                    string texturePath = AssetDatabase.GetAssetPath(obj.GetInstanceID());
                    if (!animators.Contains(texturePath))
                        animators.Add(texturePath);
                }
                else if (obj is AnimationClip)
                {
                    string texturePath = AssetDatabase.GetAssetPath(obj.GetInstanceID());
                    if (!animations.Contains(texturePath))
                        animations.Add(texturePath);
                    if (!modelsStr.Contains(texturePath))
                        modelsStr.Add(texturePath);
                }

            }
        }

       
        //删除没有用的Model
        string[] modelFiles = AssetDatabase.FindAssets("t:Model", findRoot);
        for (int i = 0; i < modelFiles.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(modelFiles[i]);
            EditorUtility.DisplayProgressBar("DelMaterialTextureModel 3", path, (float)i / modelFiles.Length);
            bool bFind = false;
            for (int j = 0; j < modelsStr.Count; j++)
            {
                if (path == modelsStr[j])
                {
                    bFind = true;
                    Debug.Log("find models :" + modelsStr[j]);
                    break;
                }
            }
            if (!bFind)
            {
                Debug.LogError("不用的models:" + path);
                AssetDatabase.DeleteAsset(path);
            }
        }
      
        string[] animationFiles = AssetDatabase.FindAssets("t:AnimationClip", findRoot);
        for (int i = 0; i < animationFiles.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(animationFiles[i]);
            EditorUtility.DisplayProgressBar("DelMaterialTextureModel 4", path, (float)i / animationFiles.Length);
            bool bFind = false;
            for (int j = 0; j < animations.Count; j++)
            {
                if (path == animations[j])
                {
                    bFind = true;
                    Debug.Log("find Animation :" + animations[j]);
                    break;
                }
            }
            if (!bFind && !allDependList.Contains(path))
            {
                Debug.LogError("不用的Animation:" + path);
                AssetDatabase.DeleteAsset(path);
            }
        }


        string[] animas = AssetDatabase.FindAssets("t:AnimatorController", findRoot);
        for (int i = 0; i < animas.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(animas[i]);
            EditorUtility.DisplayProgressBar("DelMaterialTextureModel 5", path, (float)i / animas.Length);
            
            bool bFind = false;
            for (int j = 0; j < animators.Count; j++)
            {
                if (path == animators[j])
                {
                    bFind = true;
                    Debug.Log("find animators :" + animators[j]);
                    break;
                }

            }
            if (!bFind && !allDependList.Contains(path))
            {
                Debug.LogError("不用的animators:" + path);
                AssetDatabase.DeleteAsset(path);
            }
        }
        //删除没有用的Material
        string[] materialFiles = AssetDatabase.FindAssets("t:Material", findRoot);
        for (int i = 0; i < materialFiles.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(materialFiles[i]);
            EditorUtility.DisplayProgressBar("DelMaterialTextureModel 6", path, (float)i / materialFiles.Length);
            bool bFind = false;
            for (int j = 0; j < materialsStr.Count; j++)
            {
                if (path == materialsStr[j])
                {
                    bFind = true;
                    Debug.Log("find materials :" + materialsStr[j]);
                    break;
                }
            }
            if (!bFind && !allDependList.Contains(path))
            {
                Debug.LogError("不用的materials:" + path);
                AssetDatabase.DeleteAsset(path);
            }
        }

        //删除没有用的Texture
        string[] textureFiles = AssetDatabase.FindAssets("t:Texture", findRoot);
        for (int i = 0; i < textureFiles.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(textureFiles[i]);
            EditorUtility.DisplayProgressBar("DelMaterialTextureModel 7", path, (float)i / textureFiles.Length);
            bool bFind = false;
            for (int j = 0; j < textureStr.Count; j++)
            {
                if (path == textureStr[j])
                {
                    bFind = true;
                    Debug.Log("find texture :" + textureStr[j]);
                    break;
                }
            }
            if (!bFind && !allDependList.Contains(path))
            {
                Debug.LogError("不用的texture:" + path);
                AssetDatabase.DeleteAsset(path);
            }
        }

        EditorUtility.ClearProgressBar();
    }
    private static void CheckRigidBody()
    {
        string[] ids = AssetDatabase.FindAssets("t:prefab", new string[] {"Assets"});
        int count = 0;
        for (int i = 0; i < ids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);
            GameObject mainObj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (!mainObj) return;
            var rigidbodies = mainObj.GetComponentsInChildren<Rigidbody>(true);
            if (rigidbodies.Length > 0)
            {
                count++;
                Debug.LogError("Has RigidBody => "+ mainObj.name);
            }

            var ccs = mainObj.GetComponentsInChildren<CharacterController>(true);
            if (ccs.Length > 0)
            {
                count++;
                Debug.LogError("Has CharacterController => " + mainObj.name);
            }
        }
        Debug.LogError("检查结束，数量:" + count);
    }

    public static void CheckUIFillCenter()
    {
        string[] ids = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/res" });
        int count = 0;
        for (int i = 0; i < ids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);
            GameObject mainObj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (!mainObj) return;
            var images = mainObj.GetComponentsInChildren<Image>(true);
            for (int j = 0; j < images.Length; j++)
            {
                if (images[j].type == Image.Type.Sliced && images[j].fillCenter)
                {
                    count++;
                    Debug.LogError("CheckUIFillCenter:" + images[j].name + " path:" + path);
                }
            }
        }
        Debug.LogError("检查结束，数量:" + count);
    }

    private static void CheckUIMask()
    {
        string[] ids = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/res" });
        int count = 0;
        for (int i = 0; i < ids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);
            GameObject mainObj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (!mainObj) return;
            var images = mainObj.GetComponentsInChildren<Mask>(true);
            for (int j = 0; j < images.Length; j++)
            {
                count++;
                Debug.LogError("CheckUIMask:" + mainObj.name + " path:" + path);
            }
        }
        Debug.LogError("检查结束，数量:" + count);
    }
    private static void CheckTextBestFit()
    {
        string[] ids = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/res" });
        int count = 0;
        for (int i = 0; i < ids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);
            GameObject mainObj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (!mainObj) return;
            var images = mainObj.GetComponentsInChildren<Text>(true);
            for (int j = 0; j < images.Length; j++)
            {
                if (images[j].resizeTextForBestFit)
                {
                    count++;
                    Debug.LogError("TextBestFit:" + mainObj.name + " path:" + path+" name:"+images[j].name);
                }
            }
        }
        Debug.LogError("检查结束，数量:"+count);
    }

    private static void CheckImageTiled()
    {
        int count = 0;
        string[] ids = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/res" });
        for (int i = 0; i < ids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);
            GameObject mainObj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (!mainObj) return;
            var images = mainObj.GetComponentsInChildren<Image>(true);
            for (int j = 0; j < images.Length; j++)
            {
                if (images[j].type == Image.Type.Tiled)
                {
                    count++;
                    Debug.LogError("ImageTiled:" + images[j].name + " path:" + path);
                }
            }   
        }
        Debug.LogError("检查结束，数量:" + count);
    }
    private static void CheckGridLayoutGroup()
    {
        string[] ids = AssetDatabase.FindAssets("t:prefab", new string[] {"Assets/res"});
        for (int i = 0; i < ids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);
            GameObject mainObj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (!mainObj) return;
            var groups = mainObj.GetComponentsInChildren<GridLayoutGroup>(true);
            if(groups.Length > 0)
                Debug.LogError("GridLayoutGroup:"+mainObj.name+" path:"+path);
        }
    }

    public static void ClearSpriteTagName()
    {
        string[] guidList = AssetDatabase.FindAssets("t:Texture", new string[] { "Assets/res" });

        for (int i = 0; i < guidList.Length; ++i)
        {
            EditorUtility.DisplayProgressBar("ClearSpriteTagName", " ClearSpriteTagName...", (float)i / guidList.Length);
            string path = AssetDatabase.GUIDToAssetPath(guidList[i]);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter)
            {
                if (textureImporter.textureType == TextureImporterType.Sprite)
                {
                    textureImporter.spritePackingTag = "";
                    textureImporter.sRGBTexture = false;
                    textureImporter.mipmapEnabled = false;
                    AssetDatabase.ImportAsset(path);
                }
            }
        }
       
        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }
  
    /// <summary>
    /// 获取GameObject上的AnimationClips
    /// </summary>
    public static AnimationClip[] GetAnimationClips(GameObject go)
    {
        List<AnimationClip> clips = new List<AnimationClip>();

        string path = AssetDatabase.GetAssetPath(go);
        UnityEngine.Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);

        foreach (var o in objects)
        {
            if (o is AnimationClip)
            {
                clips.Add((AnimationClip)o);
            }
        }

        return clips.ToArray();
    }
    public static void ProcessFbxMaterial()
	{
		string[] ids = AssetDatabase.FindAssets("t:Model", new[] {"Assets/res"});

		for (int i = 0; i < ids.Length; i++)
		{
			string path = AssetDatabase.GUIDToAssetPath(ids[i]);
			GameObject mainObj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if(!mainObj) continue;
		 
			Renderer[] renderComs = mainObj.GetComponentsInChildren<Renderer>(true);
			for (int j = 0; j < renderComs.Length; j++)
			{
			    if (renderComs[j])
			    {
			        EditorUtility.SetDirty(renderComs[j]);
			        renderComs[j].sharedMaterial = null;
			        renderComs[j].sharedMaterials = new Material[0];
                }
			   
            }
		    Animator[] animators = mainObj.GetComponentsInChildren<Animator>();
		    foreach (var a in animators)
		    {
		        EditorUtility.SetDirty(a);
                DestroyImmediate(a, true);
		    }

            /*
            //降低动作精度
		    List<AnimationClip> animationClipList = new List<AnimationClip>(AnimationUtility.GetAnimationClips(mainObj));
		    if (animationClipList.Count == 0)
		    {
                AnimationClip[] objectList = FindObjectsOfType(typeof(AnimationClip)) as AnimationClip[];
		        if (objectList != null) animationClipList.AddRange(objectList);
		    }

		    foreach (AnimationClip theAnimation in animationClipList)
		    {
		        EditorUtility.SetDirty(theAnimation);
                ModelMatTool.ReduceFloatPrecision(theAnimation);
		    }*/
        }
		AssetDatabase.SaveAssets();
	}

	private static void ShowFinishDialog(string msg)
	{
		if (EditorUtility.DisplayDialog("检查结束", msg, "确定"))
		{
		}
	}

	//********************************************************************************************
	private static int FindAndDelAllPrefabMissingComp()
	{
		int count = 0;
		int inc = 0;
		foreach (var prefabPath in AllPrefabPathList)
		{
			inc++;
			EditorUtility.DisplayProgressBar("正在检查空引用", prefabPath, (float) inc / AllPrefabPathList.Count);
			try
			{
				var prefab = AssetDatabase.LoadMainAssetAtPath(prefabPath) as GameObject;
				if (prefab != null)
				{
                    EditorUtility.SetDirty(prefab);
                    var gos = prefab.GetComponentsInChildren<Transform>(true);

                    foreach(var t in gos)
                    {
                        var item = t.gameObject;
				        SerializedObject so = new SerializedObject(item);
				        var soProperties = so.FindProperty("m_Component");
				        var components = item.GetComponents<Component>();
				        int propertyIndex = 0;
				        foreach (var c in components)
				        {
				            if (c == null)
				            {
                                Debug.Log("删除空引用:"+item.name);
				                soProperties.DeleteArrayElementAtIndex(propertyIndex);
				            }
				            ++propertyIndex;
				        }
				        so.ApplyModifiedProperties();
                    }
				}
			}
			catch (Exception e)
			{
			    EditorUtility.ClearProgressBar();
				Debug.LogWarning(e.ToString());
				throw;
			}
		}

        AssetDatabase.Refresh();
		EditorUtility.ClearProgressBar();
		return count;
	}

	private static int FindAllPrefabsMeshCollider()
	{
		var count = 0;
		var inc = 0;
		foreach (var prefabPath in AllPrefabPathList)
		{
			inc++;
			EditorUtility.DisplayProgressBar("正在检测MeshCollider", prefabPath, (float) inc / AllPrefabPathList.Count);
			try
			{
				var prefab = AssetDatabase.LoadMainAssetAtPath(prefabPath) as GameObject;
				if (prefab)
				{
					var components = prefab.GetComponentsInChildren<Component>(true);
					foreach (var comp in components)
					{
						if (comp is MeshCollider)
						{
							string path = comp.name;
							var tran = comp.transform;
							while (tran.parent != null)
							{
								path = tran.parent.name + "/" + path;
								tran = tran.parent;
							}

							count++;
							Debug.LogWarning("<color=#FF8C00>MeshCollider →→→ </color>" + path);
						}
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning(e.ToString());
				throw;
			}
		}

		EditorUtility.ClearProgressBar();
		return count;
	}

	private static int CheckAllBoxCollider()
	{
		var count = 0;
		var inc = 0;
		foreach (var prefabPath in AllPrefabPathList)
		{
			inc++;
			EditorUtility.DisplayProgressBar("正在检查BoxCollider", prefabPath, (float) inc / AllPrefabPathList.Count);
			try
			{
				var prefab = AssetDatabase.LoadMainAssetAtPath(prefabPath) as GameObject;
				if (prefab)
				{
					var components = prefab.GetComponentsInChildren<Component>(true);
					foreach (var comp in components)
					{
						var boxCollider = comp as BoxCollider;
						if (boxCollider != null)
						{
							if (boxCollider.size.x < 0 || boxCollider.size.y < 0 || boxCollider.size.y < 0)
							{
								string path = boxCollider.name;
								var tran = boxCollider.transform;
								while (tran.parent != null)
								{
									path = tran.parent.name + "/" + path;
									tran = tran.parent;
								}

								count++;
								Debug.LogWarning("<color=#FF8C00>BoxCollider →→→ </color>" + path);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning(e.ToString());
				throw;
			}
		}

		EditorUtility.ClearProgressBar();
		return count;
	}

	private static int FindAllAnimatorController()
	{
		int count = 0;
		int inc = 0;
		foreach (var prefabPath in AllPrefabPathList)
		{
			inc++;
			EditorUtility.DisplayProgressBar("删除所有空状态机", prefabPath,
				(float) inc / AllPrefabPathList.Count);
			if (!prefabPath.StartsWith("Assets/res")) continue;
			try
			{
				var prefab = AssetDatabase.LoadMainAssetAtPath(prefabPath) as GameObject;
				if (prefab)
				{
					var components = prefab.GetComponentsInChildren<Component>(true);
					foreach (var comp in components)
					{
						var animator = comp as Animator;
						if (animator != null)
						{
							if (animator.runtimeAnimatorController == null)
							{
								count++;
                                DestroyImmediate(animator, true);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning(e.ToString());
				throw;
			}
		}

	    AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
		return count;
	}

    private static bool CheckDefault(string str)
    {
        if (str.Contains("builtin") || str.Contains("unity default resources"))
            return true;
        return false;
    }

    private static void RepleceDefaultBulidinResource(bool isScene = false)
    {
       

        Material defaultUiMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/res/common/Default-UI.mat");
        Mesh cube = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/res/common/mesh_Cube.asset");
        Mesh capsule = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/res/common/mesh_Capsule.asset");
        Mesh cylinder = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/res/common/mesh_Cylinder.asset");
        Mesh plane = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/res/common/mesh_Plane.asset");
        Mesh quad = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/res/common/mesh_Quad.asset");
        Mesh sphere = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/res/common/mesh_Sphere.asset");

        if (!isScene)
        {
            string path = "Assets/";
            var allfiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith("mat") || s.EndsWith("prefab") || s.EndsWith("unity")).ToArray();
            foreach (var item in allfiles)
            {
                if (item.EndsWith("prefab"))
                {
                    int inc = 0;
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(item);
                    if (prefab)
                    {
                        inc++;
                        EditorUtility.DisplayProgressBar("RepleceDefaultBulidinResource", item, (float)inc / allfiles.Length);


                        #region 检查空Animation

                        var animations = prefab.GetComponentsInChildren<Animation>(true);
                        foreach (var a in animations)
                        {
                            if (!a) return;
                            if (a.clip == null)
                            {
                                Debug.LogError("Animation == null:" + prefab.name);
                            }
                        }

                        #endregion
                        #region 替换系统MESH

                        var meshColliders = prefab.GetComponentsInChildren<MeshCollider>(true);
                        foreach (var filter in meshColliders)
                        {
                            if (!filter) continue;
                            if (filter.sharedMesh == null)
                            {
                                //Debug.LogError("MeshFilter == null:" + prefab.name);
                                continue;
                            }
                            var assetPath = AssetDatabase.GetAssetPath(filter.sharedMesh);
                            if (CheckDefault(assetPath))
                            {
                                EditorUtility.SetDirty(filter.sharedMesh);
                                if (filter.sharedMesh.name == "Cube")
                                {
                                    if (cube)
                                        filter.sharedMesh = cube;
                                }
                                else if (filter.sharedMesh.name == "Capsule")
                                {
                                    if (capsule)
                                        filter.sharedMesh = capsule;
                                }
                                else if (filter.sharedMesh.name == "Cylinder")
                                {
                                    if (cylinder)
                                        filter.sharedMesh = cylinder;
                                }
                                else if (filter.sharedMesh.name == "Plane")
                                {
                                    if (plane)
                                        filter.sharedMesh = plane;
                                }
                                else if (filter.sharedMesh.name == "Quad")
                                {
                                    if (quad)
                                        filter.sharedMesh = quad;
                                }
                                else if (filter.sharedMesh.name == "Sphere")
                                {
                                    if (sphere)
                                        filter.sharedMesh = sphere;
                                }
                            }
                        }

                        var filters = prefab.GetComponentsInChildren<MeshFilter>(true);

                        foreach (var filter in filters)
                        {
                            if (!filter) continue;
                            if (filter.sharedMesh == null)
                            {
                                //Debug.LogError("MeshFilter == null:" + prefab.name);
                                continue;
                            }
                            var assetPath = AssetDatabase.GetAssetPath(filter.sharedMesh);
                            if (CheckDefault(assetPath))
                            {
                                EditorUtility.SetDirty(filter.sharedMesh);
                                if (filter.sharedMesh.name == "Cube")
                                {
                                    if (cube)
                                        filter.sharedMesh = cube;
                                }
                                else if (filter.sharedMesh.name == "Capsule")
                                {
                                    if (capsule)
                                        filter.sharedMesh = capsule;
                                }
                                else if (filter.sharedMesh.name == "Cylinder")
                                {
                                    if (cylinder)
                                        filter.sharedMesh = cylinder;
                                }
                                else if (filter.sharedMesh.name == "Plane")
                                {
                                    if (plane)
                                        filter.sharedMesh = plane;
                                }
                                else if (filter.sharedMesh.name == "Quad")
                                {
                                    if (quad)
                                        filter.sharedMesh = quad;
                                }
                                else if (filter.sharedMesh.name == "Sphere")
                                {
                                    if (sphere)
                                        filter.sharedMesh = sphere;
                                }
                            }
                        }


                        #endregion
                        #region UIMaterial
                        var defaultName = "Default UI Material";
                        var images = prefab.GetComponentsInChildren<Image>(true);
                        foreach (var image in images)
                        {
                            if (image.material.name == defaultName)
                            {
                                EditorUtility.SetDirty(image);
                                image.material = defaultUiMaterial;
                            }
                        }

                        var rawImages = prefab.GetComponentsInChildren<RawImage>(true);
                        foreach (var r in rawImages)
                        {
                            if (r.material.name == defaultName)
                            {
                                EditorUtility.SetDirty(r);
                                r.material = defaultUiMaterial;
                            }
                        }

                        var texts = prefab.GetComponentsInChildren<Text>(true);
                        foreach (var t in texts)
                        {
                            if (t.material.name == defaultName)
                            {
                                EditorUtility.SetDirty(t);
                                t.material = defaultUiMaterial;
                            }
                        }

                        #endregion
                    }
                }
            }
        }
        else
        {
            Material defaultMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/res/common/Default-Material.mat");
            var sceneObjects  = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
            foreach (var obj in sceneObjects)
            {
                if(!obj)continue;

                var renderers = obj.GetComponentsInChildren<Renderer>(true);
                foreach (var r in renderers)
                {
                    if(!r) continue;
                    if (r.sharedMaterial && r.sharedMaterial.name == "Default-Material")
                    {
                        EditorUtility.SetDirty(r);
                        r.sharedMaterial = defaultMaterial;
                    }

                    for (int i = 0; r.sharedMaterials != null && i < r.sharedMaterials.Length; i++)
                    {
                        if(r.sharedMaterials[i] == null) continue;
                        if(r.sharedMaterials[i].name != "Default-Material") continue;
                        
                        EditorUtility.SetDirty(r);
                        r.sharedMaterials[i] = defaultMaterial;
                    }
                }

                var filters = obj.GetComponentsInChildren<MeshFilter>(true);
                foreach (var filter in filters)
                {
                    if (!filter) continue;
                    if (filter.sharedMesh == null)
                    {
                        //Debug.LogError("MeshFilter == null:" + prefab.name);
                        continue;
                    }
                    var assetPath = AssetDatabase.GetAssetPath(filter.sharedMesh);
                    if (CheckDefault(assetPath))
                    {
                        EditorUtility.SetDirty(filter.sharedMesh);
                        if (filter.sharedMesh.name == "Cube")
                        {
                            if (cube)
                                filter.sharedMesh = cube;
                        }
                        else if (filter.sharedMesh.name == "Capsule")
                        {
                            if (capsule)
                                filter.sharedMesh = capsule;
                        }
                        else if (filter.sharedMesh.name == "Cylinder")
                        {
                            if (cylinder)
                                filter.sharedMesh = cylinder;
                        }
                        else if (filter.sharedMesh.name == "Plane")
                        {
                            if (plane)
                                filter.sharedMesh = plane;
                        }
                        else if (filter.sharedMesh.name == "Quad")
                        {
                            if (quad)
                                filter.sharedMesh = quad;
                        }
                        else if (filter.sharedMesh.name == "Sphere")
                        {
                            if (sphere)
                                filter.sharedMesh = sphere;
                        }
                    }
                }

                var meshColliders = obj.GetComponentsInChildren<MeshCollider>(true);
                foreach (var filter in meshColliders)
                {
                    if (!filter) continue;
                    if (filter.sharedMesh == null)
                    {
                        //Debug.LogError("MeshFilter == null:" + prefab.name);
                        continue;
                    }
                    var assetPath = AssetDatabase.GetAssetPath(filter.sharedMesh);
                    if (CheckDefault(assetPath))
                    {
                        EditorUtility.SetDirty(filter.sharedMesh);
                        if (filter.sharedMesh.name == "Cube")
                        {
                            if (cube)
                                filter.sharedMesh = cube;
                        }
                        else if (filter.sharedMesh.name == "Capsule")
                        {
                            if (capsule)
                                filter.sharedMesh = capsule;
                        }
                        else if (filter.sharedMesh.name == "Cylinder")
                        {
                            if (cylinder)
                                filter.sharedMesh = cylinder;
                        }
                        else if (filter.sharedMesh.name == "Plane")
                        {
                            if (plane)
                                filter.sharedMesh = plane;
                        }
                        else if (filter.sharedMesh.name == "Quad")
                        {
                            if (quad)
                                filter.sharedMesh = quad;
                        }
                        else if (filter.sharedMesh.name == "Sphere")
                        {
                            if (sphere)
                                filter.sharedMesh = sphere;
                        }
                    }
                }
            }
        }
        
        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }

    private static void FindAllAnimatorInUIPrefabs()
	{
		int inc = 0;
		foreach (var prefabPath in AllPrefabPathList)
		{
			inc++;
			EditorUtility.DisplayProgressBar("正在检查Animator Controller", prefabPath,
				(float) inc / AllPrefabPathList.Count);
			if (!prefabPath.StartsWith("Assets/res/ui/prefabsx")) continue;
			try
			{
				var prefab = AssetDatabase.LoadMainAssetAtPath(prefabPath) as GameObject;
				if (prefab)
				{
					var components = prefab.GetComponentsInChildren<Component>(true);
					foreach (var comp in components)
					{
						var animator = comp as Animator;
						if (animator != null)
						{
							string path = animator.name;
							var tran = animator.transform;
							while (tran.parent != null)
							{
								path = tran.parent.name + "/" + path;
								tran = tran.parent;
							}

							Debug.LogWarning("<color=#FF8C00>Animator -> </color>" + prefabPath + " | tree --> " +
							                 path);
						}
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning(e.ToString());
				throw;
			}
		}

		EditorUtility.ClearProgressBar();
	}
}