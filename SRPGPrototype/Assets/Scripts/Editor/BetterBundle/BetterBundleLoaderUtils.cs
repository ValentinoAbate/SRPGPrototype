using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class BetterBundleLoaderUtils
{
    public static void LoadBundle<T>(this BetterBundle<T> bundle) where T : UnityEngine.Object, IHasKey
    {
        bundle.Clear();
        foreach (var assetPath in bundle.assetPaths)
        {
            var items = AssetUtils.LoadAllAssetsInDirectory<T>(assetPath, true);
            foreach (var item in items)
            {
                bundle.Add(item);
            }
        }
        EditorUtility.SetDirty(bundle);
    }

    public static void LoadBundles(string path)
    {
        var bundles = AssetUtils.LoadAllAssetsInDirectory<ScriptableObject>(path);
        foreach(var bundle in bundles)
        {
            LoadBundle((dynamic)bundle);
        }
        AssetDatabase.SaveAssets();
    }
}
