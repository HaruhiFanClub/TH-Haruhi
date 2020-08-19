

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public static class EditorAssist
{
    public static RuntimeTarget BuildTargetToRuntimeTarget(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.StandaloneWindows:
                return RuntimeTarget.windows;
#if UNITY_2017_3_OR_NEWER
            case BuildTarget.StandaloneOSX :
#else
            case BuildTarget.StandaloneOSXUniversal:       
#endif
                return RuntimeTarget.mac;
            case BuildTarget.iOS:
                return RuntimeTarget.ios;
            case BuildTarget.Android:
                return RuntimeTarget.android;
            default:
                return RuntimeTarget.unknow;
        }
    }

    public static BuildTargetGroup BuildTargetToBuildTargetGroup(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.StandaloneWindows:
#if UNITY_2017_3_OR_NEWER
            case BuildTarget.StandaloneOSX :
#else
            case BuildTarget.StandaloneOSXUniversal:       
#endif
                return BuildTargetGroup.Standalone;
            case BuildTarget.iOS:
                return BuildTargetGroup.iOS;
            case BuildTarget.Android:
                return BuildTargetGroup.Android;
            default:
                return BuildTargetGroup.Unknown;
        }
    }

    public static string BuildExtension(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.StandaloneWindows :
                return "exe";
#if UNITY_2017_3_OR_NEWER
            case BuildTarget.StandaloneOSX :
#else
            case BuildTarget.StandaloneOSXUniversal:       
#endif
                return "app";
            case BuildTarget.iOS :
                return null;
            case BuildTarget.Android:
                return "apk";
            default :
                return string.Empty;
        }
    }

    public static void CreatedDirectory(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    public static void EmptyDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            foreach (string file in Directory.GetFiles(path))
                File.Delete(file);
            foreach (string directory in Directory.GetDirectories(path))
                Directory.Delete(directory, true);
        }
    }

    //manifest改名，前面加 . 这样打包的时候就会忽略
    public static void RenameAllManifestFiles(string path = "")
    {
        if (path == "")
            path = Application.streamingAssetsPath;
        string[] fileNames = Directory.GetFiles(path, "*.manifest", SearchOption.AllDirectories);
        int count = 0;
        foreach (string fileName in fileNames)
        {
            FileInfo file = new FileInfo(fileName);
            string dotName = Path.Combine(file.DirectoryName, "." + file.Name);
            if(File.Exists(dotName))
                File.Delete(dotName);
            File.Move(fileName, dotName);
            count++;
        }
        //ResourceBuildTool.OutputLog(string.Format("RenameAllManifestFiles in path:" + path + ", count:" + count));
    }

    //manifest改名再改回来
    public static void RestoreAllManifestFiles(string path = "")
    {
        if (path == "")
            path = Application.streamingAssetsPath;
        //ResourceBuildTool.OutputLog(string.Format("RestoreAllManifestFiles in path:[{0}]", path));
        string[] fileNames = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        int count = 0;
        foreach (string fileName in fileNames)
        {
            FileInfo file = new FileInfo(fileName);
            if (file.Name.StartsWith("."))
            {
                string newName = Path.Combine(file.DirectoryName, file.Name.Substring(1));
                if(File.Exists(newName))
                    File.Delete(newName);
                File.Move(fileName, newName);
                count++;
            }
        }
        //ResourceBuildTool.OutputLog(string.Format("RestoreAllManifestFiles in path:" + path + ", count:" + count));
    }


}