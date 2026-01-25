using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public static class ComponentAttachmentUtils
{
    [MenuItem("Tools/Asset/Attach All Program Components")]
    public static void AttachAllProgramEffects()
    {
        try
        {
            foreach (var program in AssetUtils.LoadAllAssetsInDirectory<Program>(AssetPaths.programPath, true))
            {
                program.LinkComponents();
                EditorUtility.SetDirty(program.gameObject);
            }
            AssetDatabase.SaveAssets();
        }
        catch(Exception e)
        {
            Debug.LogError($"Link All Program Components Exception: {e.Message}");
        }
    }

    [MenuItem("Tools/Asset/Generate Asset Keys")]
    public static void GenerateProgramKeys()
    {
        try
        {
            foreach (var program in AssetUtils.LoadAllAssetsInDirectory<Program>(AssetPaths.programPath, true))
            {
                if (program.GenerateKey())
                {
                    EditorUtility.SetDirty(program.gameObject);
                }
            }
            foreach (var shell in AssetUtils.LoadAllAssetsInDirectory<Shell>(AssetPaths.shellPath, true))
            {
                if (shell.GenerateKey())
                {
                    EditorUtility.SetDirty(shell.gameObject);
                }
            }
            AssetDatabase.SaveAssets();
        }
        catch (Exception e)
        {
            Debug.LogError($"Generate Asset Keys Exception: {e.Message}");
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
