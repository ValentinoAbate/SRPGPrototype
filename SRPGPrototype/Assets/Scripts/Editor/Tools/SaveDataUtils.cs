using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SaveDataUtils
{
    [MenuItem("Tools/Save Data/Open Save Data Folder")]
    public static void OpenSaveDataFolder()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
}
