using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class SaveManager
{
    public const char separator = ',';
    private static string RunFilePath() => Path.Combine(Application.persistentDataPath, $"runSaveData.dat");
    public static string GlobalSaveFilePath() => Path.Combine(Application.persistentDataPath, "globalSaveData.dat");

    public enum State
    {
        Cust,
        UnitPlacement,
        Battle,
        Loot,
        Map,
    }

    public static void Save(State state)
    {
        var runData = new RunData()
        {
            state = state,
        };
        PersistantData.main.SaveRunData(runData, state);
        SaveFile(runData, RunFilePath());
    }

    public static void Load()
    {
        var runData = LoadFile<RunData>(RunFilePath());
        var loader = new Loader(runData);
        foreach(var prog in runData.tPr)
        {
            loader.CreateProgram(prog, false, out _);
        }
        foreach(var shell in runData.tSh)
        {
            loader.CreateShell(shell, false, out _);
        }
        PersistantData.main.LoadRunData(runData, loader, runData.state);
        if(runData.state == State.UnitPlacement || runData.state == State.Battle)
        {
            SceneTransitionManager.main.TransitionToScene(SceneTransitionManager.EncounterSceneName);
        }
        else if(runData.state == State.Loot)
        {
            PersistantData.main.loot.ShowSaved(PersistantData.main.inventory, GoToCust);
        }
        else if(runData.state == State.Map)
        {
            SceneTransitionManager.main.TransitionToScene(SceneTransitionManager.MapSceneName);
        }
        else
        {
            SceneTransitionManager.main.TransitionToScene(SceneTransitionManager.CustSceneName);
        }
    }

    private static void GoToCust()
    {
        SceneTransitionManager.main.TransitionToScene(SceneTransitionManager.CustSceneName);
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
        public Dictionary<int, ProgramData> FusionArgs { get; } = new Dictionary<int, ProgramData>();

        public Loader(RunData data)
        {
            foreach(var fusionArg in data.fArgs)
            {
                FusionArgs.Add(fusionArg.id, fusionArg);
            }
        }

        public bool CreateShell(ShellData data, bool isBattle, out Shell shell) => CreateShell(data, null, isBattle, out shell);

        public bool CreateShell(ShellData data, Transform parent, bool isBattle, out Shell shell)
        {
            if (Lookup.TryGetShell(data.k, out var prefab))
            {
                shell = Create<Shell>(prefab.gameObject, parent);
                shell.Load(data, this, isBattle);
                LoadedShells.Add(shell.Id, shell);
                return true;
            }
            shell = null;
            return false;
        }

        public bool CreateProgram(ProgramData data, bool isBattle, out Program program) => CreateProgram(data, null, isBattle, out program);

        public bool CreateProgram(ProgramData data, Transform parent, bool isBattle, out Program program)
        {
            if(Lookup.TryGetProgram(data.k, out var prefab))
            {
                program = Create<Program>(prefab.gameObject, parent);
                if (program.IsFusion)
                {
                    CreateFusion(program, data, parent, isBattle);
                }
                program.Load(data, isBattle);
                LoadedPrograms.Add(program.Id, program);
                return true;
            }
            program = null;
            return false;
        }

        private bool CreateFusion(Program fusion, ProgramData data, Transform parent, bool isBattle)
        {
            if (!data.TryFindEntry(Program.fusionId, out var entry))
                return false;
            int pid1;
            int pid2;
            string fusionName;
            Pattern fusionPattern;
            try
            {
                pid1 = int.Parse(entry.d[0]);
                pid2 = int.Parse(entry.d[1]);
                fusionName = entry.d[2];
                fusionPattern = new Pattern();
                fusionPattern.Load(entry.d[3]);
            }
            catch 
            {
                return false;
            }
            if (!FusionArgs.TryGetValue(pid1, out var pData1) || !FusionArgs.TryGetValue(pid2, out var pData2))
                return false;
            if (CreateProgram(pData1, parent, isBattle, out var p1) && CreateProgram(pData2, parent, isBattle, out var p2))
            {
                PersistantData.main.programFuser.FusePrograms(fusion, p1, p2, fusionPattern, fusionName);
                return true;
            }
            return false;
        }

        public bool CreateUnit(UnitData data, out Unit unit) => CreateUnit(data, null, out unit);

        public bool CreateUnit(UnitData data, Transform parent, out Unit unit)
        {
            if (Lookup.TryGetUnit(data.k, out var prefab))
            {
                unit = Create<Unit>(prefab.gameObject, parent);
                unit.Load(data, this);
                return true;
            }
            unit = null;
            return false;
        }

        private T Create<T>(GameObject prefab, Transform parent) where T : MonoBehaviour
        {
            if(parent == null)
            {
                return UnityEngine.Object.Instantiate(prefab).GetComponent<T>();
            }
            else
            {
                return UnityEngine.Object.Instantiate(prefab, parent).GetComponent<T>();
            }
        }
    }

    [Serializable]
    public class RunData
    {
        public State state;
        public int currId;
        public int bInd;
        public InventoryData inv;
        public MapManagerData map;
        public List<ShopData> shops;
        public List<PresetData> presets;
        public List<ProgramData> fArgs = new List<ProgramData>();
        public BattleEncounterData battle;
        public List<ProgramData> tPr = new List<ProgramData>();
        public List<ShellData> tSh = new List<ShellData>();
        public List<LootData> loot;
        // Battle State
    }

    [Serializable]
    public class MapManagerData
    {
        public int depth;
        public List<SavedMap> maps;
        public List<EncounterData> next;
    }

    [Serializable]
    public class EncounterData
    {
        public string name = string.Empty;
        public Vector2Int dim;
        public List<GridPrefab> units;
        public List<GridPrefab> ambush;
        public List<Vector2Int> spawns;
        public bool money;
        public int moneyBase;
        public int moneyVar;
    }

    [Serializable]
    public class SavedMap
    {
        public string k;
        public int depth;
        public bool isBase;
    }

    [Serializable]
    public class GridPrefab
    {
        public string k;
        public Vector2Int p;
    }

    [Serializable]
    public class BattleEncounterData
    {
        public List<UnitData> data;
    }

    [Serializable]
    public class UnitData : HasData
    {
        public string k;
        public Vector2Int p;
        public int hp;
        public int mHp;
        public int ap;
        public int mAp;
        public int r;
        public int brk;
        public int pow;
        public int hk;
        public Vector2Int sp;
    }

    [Serializable]
    public class InventoryData
    {
        public int money;
        public int equipShId;
        public List<ShellData> shs;
        public List<ProgramData> prs;
    }

    [Serializable]
    public class ShellData
    {
        public int id;
        public string k;
        public int lv;
        public int hp;
        public List<InstalledProgramData> prs;
    }

    [Serializable]
    public class InstalledProgramData
    {
        public ProgramData pr;
        public Vector2Int p;
    }

    [Serializable]
    public class ProgramData : HasData
    {
        public int id;
        public string k;
        public string u;
    }

    [Serializable]
    public class HasData
    {
        public List<DataEntry> d = new List<DataEntry>();

        public void AddData(int type, params string[] args)
        {
            d.Add(new DataEntry()
            {
                t = type,
                d = new List<string>(args)
            });
        }

        public void AddData(int type, List<string> data)
        {
            this.d.Add(new DataEntry()
            {
                t = type,
                d = data
            });
        }

        public bool TryFindEntry(int type, out DataEntry entry)
        {
            foreach (var d in d)
            {
                if (d.t == type)
                {
                    entry = d;
                    return true;
                }
            }
            entry = null;
            return false;
        }
    }

    [Serializable]
    public class DataEntry : IReadOnlyList<string>
    {
        public int Count => d.Count;

        public int t; // type
        public List<string> d; // data

        public string this[int index] => d[index];

        public IEnumerator<string> GetEnumerator()
        {
            return d.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return d.GetEnumerator();
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

    [Serializable]
    public class ShopData
    {
        public ShopManager.ShopID id;
        public List<ShellData> shs;
        public List<ProgramData> prs;
    }

    [Serializable]
    public class PresetData
    {
        public string k;
        public bool load;
        public string name;
        public int lv;
        public int ind;
        public List<InstalledProgramId> prs;
    }

    [Serializable]
    public class InstalledProgramId
    {
        public Vector2Int p;
        public int id;
    }

    [Serializable]
    public class LootData
    {
        public string n;
        public List<int> d;
        public int m;
    }
}
