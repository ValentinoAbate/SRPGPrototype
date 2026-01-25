using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    private static string RunFilePath() => Path.Combine(Application.persistentDataPath, $"runSaveData.dat");
    public static string GlobalSaveFilePath() => Path.Combine(Application.persistentDataPath, "globalSaveData.dat");
    public static void Save()
    {
        var runData = new RunData();
        PersistantData.main.SaveRunData(runData);
        SaveFile(runData, RunFilePath());
    }

    public static void Load()
    {
        var runData = LoadFile<RunData>(RunFilePath());
        var loader = new Loader();
        PersistantData.main.LoadRunData(runData, loader);
    }

    public static void ClearRunData()
    {
        File.Delete(RunFilePath());
    }

    public static bool HasRunData()
    {
        return File.Exists(RunFilePath());
    }

    public static void SaveFile(object saveData, string path)
    {
        try
        {
            //convert to JSON, then to bytes
#if DEBUG
            string jsonData = JsonUtility.ToJson(saveData, true);
#else
            string jsonData = JsonUtility.ToJson(saveData, false);
#endif
            byte[] jsonByte = Encoding.ASCII.GetBytes(jsonData);

            //create the save directory if it doesn't exist
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            File.WriteAllBytes(path, jsonByte);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving file: {e}");
        }
    }

    public static T LoadFile<T>(string path) where T : new()
    {
        if (File.Exists(path))
        {
            try
            {
                string jsonData = Encoding.ASCII.GetString(File.ReadAllBytes(path));
                return JsonUtility.FromJson<T>(jsonData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading file: {e}");
            }
        }
        return default;
    }

    public class Loader
    {
        public Dictionary<int, Shell> LoadedShells { get; } = new Dictionary<int, Shell>();
        public Dictionary<int, Program> LoadedPrograms { get; } = new Dictionary<int, Program>();
        public Dictionary<int, Action> LoadedActions { get; } = new Dictionary<int, Action>();

        public bool CreateShell(ShellData data, out Shell shell) => CreateShell(data, null, out shell);

        public bool CreateShell(ShellData data, Transform parent, out Shell shell)
        {
            if (PersistantData.main.loot.TryGetShell(data.key, out var prefab))
            {
                shell = Create<Shell>(prefab.gameObject, parent);
                shell.Load(data, this);
                LoadedShells.Add(shell.Id, shell);
                return true;
            }
            shell = null;
            return false;
        }

        public bool CreateProgram(ProgramData data, out Program program) => CreateProgram(data, null, out program);

        public bool CreateProgram(ProgramData data, Transform parent, out Program program)
        {
            if(PersistantData.main.loot.TryGetProgram(data.key, out var prefab))
            {
                program = Create<Program>(prefab.gameObject, parent);
                program.Load(data);
                LoadedPrograms.Add(program.Id, program);
                return true;
            }
            program = null;
            return false;
        }

        private T Create<T>(GameObject prefab, Transform parent) where T : MonoBehaviour
        {
            if(parent == null)
            {
                return Instantiate(prefab).GetComponent<T>();
            }
            else
            {
                return Instantiate(prefab, parent).GetComponent<T>();
            }
        }
    }

    [Serializable]
    public class RunData
    {
        public int currId;
        public InventoryData inv;

        // Presets
        // Map data
        // Shops
        // Battle State
    }

    [Serializable]
    public class InventoryData
    {
        public int money;
        public int equipShId;
        public List<ShellData> shells;
        public List<ProgramData> progs;
    }

    [Serializable]
    public class ShellData
    {
        public int id;
        public string key;
        public int level;
        public int hp;
        public List<InstalledProgramData> progs;
    }

    [Serializable]
    public class InstalledProgramData
    {
        public ProgramData prog;
        public Vector2Int pos;
    }

    [Serializable]
    public class ProgramData
    {
        public int id;
        public string key;
        public List<DataPair> data = new List<DataPair>();
        // TODO: updrade data

        public void AddData(int id, string value)
        {
            data.Add(new DataPair(id, value));
        }
    }

    [Serializable]
    public class DataPair
    {
        public int t; // type
        public string v; // value
        public DataPair(int id, string value)
        {
            t = id;
            v = value;
        }
    }

    [Serializable]
    public class ActionData
    {
        public int id;
        public int used;
        public int usedFree;
        public int usedBattle;
        public int usedBattleFree;
        public int usedTurn;
        public int usedTurnFree;
        public List<string> effectData;
    }
}
