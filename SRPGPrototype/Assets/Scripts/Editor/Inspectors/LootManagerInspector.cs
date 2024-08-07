﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(LootManager))]
public class LootManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var data = target as LootManager;
        if(GUILayout.Button(new GUIContent("Refresh Loot List")))
        {
            Undo.RecordObject(target, "Refreshing Loot List");
            var programPrefabs = AssetUtils.LoadAllAssetsInDirectory<Program>(AssetPaths.programPath, true);
            var shellPrefabs = AssetUtils.LoadAllAssetsInDirectory<Shell>(AssetPaths.shellPath, true);
            data.programs = new List<Program>(programPrefabs.Where((s) => s.DisplayName != string.Empty));
            data.shells = new List<Shell>(shellPrefabs.Where((s) => s.DisplayName != string.Empty));
        }
        base.OnInspectorGUI();
    }
}
