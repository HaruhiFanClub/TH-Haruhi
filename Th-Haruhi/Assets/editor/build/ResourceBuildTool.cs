﻿

using System;
using System.IO;
using System.Collections.Generic;

public static class ResourceBuildTool
{
    private static readonly ResourceType[] buildFilter;
    private static readonly ResourceType[] sceneFilterRemoves;
    private static readonly ResourceType[] directoryFilterRemoves;

    static ResourceBuildTool()
    {
        buildFilter = new[]
        {
            ResourceType.table,
            ResourceType.config,
            ResourceType.scene,
            ResourceType.prefab,
            ResourceType.spriteatlas,
            ResourceType.controller,
            ResourceType.fbx,
            ResourceType.audio,
            ResourceType.material,
            ResourceType.shader,
            ResourceType.shadervariants,
            ResourceType.speedTree,
            ResourceType.terrainLayer,
            ResourceType.font,
            ResourceType.mp4,
            ResourceType.texture,
            ResourceType.bytes,
            ResourceType.asset,
            ResourceType.physicMaterial,
            ResourceType.anim,
            ResourceType.mask,
        };

        sceneFilterRemoves = new[]
        {
            ResourceType.prefab,
            ResourceType.controller,
            ResourceType.fbx,
            ResourceType.audio,
            ResourceType.material,
            ResourceType.shadervariants,
            ResourceType.shader,
            ResourceType.font,
            ResourceType.mp4,
            ResourceType.texture,
            ResourceType.asset,
            ResourceType.terrainLayer,
        };

        directoryFilterRemoves = new[]
        {
            ResourceType.controller,
            ResourceType.fbx,
            ResourceType.material,
            ResourceType.shader,
            ResourceType.shadervariants,
            ResourceType.font,
            ResourceType.texture,
            ResourceType.scene,
        };
    }

    public static List<string> GetBuildResources(string pathname)
    {
        var resourceList = new List<string>();
        GetBuildResources(resourceList, pathname, buildFilter);
        return resourceList;
    }

    public static void GetBuildResources(List<string> resourceList, string pathname, params ResourceType[] filter)
    {
        pathname = PathUtility.FormatPath(pathname);

        if (!Directory.Exists(PathUtility.ProjectPathToFullPath(pathname)))
        {
            if (CollectionUtility.Contains(filter, ResourcesUtility.GetResourceTypeByPath(pathname)))
                resourceList.Add(pathname);
        }
        else
        {
            //SCENES
            if(pathname.Contains("res/scenes"))
            {
                filter = CollectionUtility.Remove(filter, sceneFilterRemoves);
            }

            //ALWAYS BUILD
            else if (
                pathname.Contains("res/player") ||
                pathname.Contains("res/bullet") ||
                pathname.Contains("res/enemy") ||
                pathname.Contains("res/effects_tex") ||
                pathname.Contains("res/drawing"))
            {
                filter = buildFilter;
            }
            //Only Prefabs, No Scenes
            else
            {
                filter = CollectionUtility.Remove(filter, directoryFilterRemoves);
            }



            string[] fileList = Directory.GetFiles(pathname, "*");
            foreach (string resource in fileList)
                if (CollectionUtility.Contains(
                    filter, ResourcesUtility.GetResourceTypeByPath(resource)))
                    resourceList.Add(PathUtility.FullPathToProjectPath(resource));

            string[] dictList = Directory.GetDirectories(pathname);
            foreach (string dictname in dictList)
                GetBuildResources(resourceList, dictname, filter);
        }
    }
}