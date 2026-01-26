using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public static class ComponentAttachmentUtils
{
    [MenuItem("Tools/Asset/Generate Asset Keys")]
    public static void GenerateAssetKeys()
    {
        try
        {
            foreach (var program in AssetUtils.LoadAllAssetsInDirectory<Program>(AssetPaths.programPath, true))
            {
                UpdateKey(program);
            }
            foreach (var shell in AssetUtils.LoadAllAssetsInDirectory<Shell>(AssetPaths.shellPath, true))
            {
                UpdateKey(shell);
            }
            foreach (var unit in AssetUtils.LoadAllAssetsInDirectory<Unit>(AssetPaths.unitPath, true))
            {
                UpdateKey(unit);
            }
            AssetDatabase.SaveAssets();
        }
        catch (Exception e)
        {
            Debug.LogError($"Generate Asset Keys Exception: {e.Message}");
        }
    }

    private static void UpdateKey<T>(T item) where T : UnityEngine.Object, IHasKey
    {
        if(item.GenerateKey())
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(item);
            EditorUtility.SetDirty(item);
        }
    }

    [MenuItem("Tools/Asset/Attach All Program Components")]
    public static void AttachAllProgramEffects()
    {
        try
        {
            foreach (var program in AssetUtils.LoadAllAssetsInDirectory<Program>(AssetPaths.programPath, true))
            {
                program.LinkComponents();
                PrefabUtility.RecordPrefabInstancePropertyModifications(program);
                EditorUtility.SetDirty(program.gameObject);
            }
            AssetDatabase.SaveAssets();
        }
        catch (Exception e)
        {
            Debug.LogError($"Link All Program Components Exception: {e.Message}");
        }
    }

    [MenuItem("Tools/Asset/Attach All Unit Abilities")]
    public static void AttachAllUnitAbilities()
    {
        try
        {
            foreach (var unit in AssetUtils.LoadAllAssetsInDirectory<AIUnit>(AssetPaths.unitPath, true))
            {
                unit.AttachAbilities();
                PrefabUtility.RecordPrefabInstancePropertyModifications(unit);
                EditorUtility.SetDirty(unit.gameObject);
            }
            AssetDatabase.SaveAssets();
        }
        catch (Exception e)
        {
            Debug.LogError($"Attach All Unit Abilities Exception: {e.Message}");
        }

    }
}
