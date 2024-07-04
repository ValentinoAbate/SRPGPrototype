using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public static class ComponentAttachmentUtils
{
    [MenuItem("Tools/Attach All Program Effects")]
    public static void AttachAllProgramEffects()
    {
        try
        {
            foreach (var program in AssetUtils.LoadAllAssetsInDirectory<Program>(AssetPaths.programPath, true))
            {
                program.SetEffects();
                EditorUtility.SetDirty(program.gameObject);
            }
            AssetDatabase.SaveAssets();
        }
        catch(Exception e)
        {
            Debug.LogError($"Attach All Program Effects Exception: {e.Message}");
        }

    }
}
