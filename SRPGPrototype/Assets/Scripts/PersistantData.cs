using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is a singleton that shouldn't be destroyed
/// </summary>
//[ExecuteAlways]
public class PersistantData : MonoBehaviour
{
    public int CurrentId { get; private set; } = 0;
    public int NewId => ++CurrentId;
    public Encounter CurrentEncounter
    {
        get
        {
            if (BattleData.SelectedEncounterIndex < 0 || BattleData.SelectedEncounterIndex >= mapManager.NextEncounters.Count)
                return null;
            return mapManager.NextEncounters[BattleData.SelectedEncounterIndex];
        }
    }
    public BattleInitializationData BattleData { get; } = new BattleInitializationData();

    public static PersistantData main;
    public Inventory inventory;
    public MapManager mapManager;
    public PresetManager presetManager;
    public ShopManager shopManager;
    public LootManager loot;
    public ProgramFuser programFuser;

    private void Awake()
    {
        if(main == null)
        {
            main = this;
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(transform);
                ResetRunData();
            }
        }
        else if (Application.isPlaying)
        {
            Destroy(gameObject);
        }
    }

    public void ResetRunData()
    {
        presetManager.Clear();
        inventory.Clear();
        CurrentId = 0;
        shopManager.Initialize();
    }

    public void SaveRunData(SaveManager.RunData runData, SaveManager.State state)
    {
        bool isBattle = state == SaveManager.State.Battle;
        runData.currId = CurrentId;
        runData.inv = inventory.Save(isBattle, ref runData.fArgs);
        runData.map = mapManager.Save();
        runData.shops = shopManager.Save(ref runData.fArgs);
        runData.presets = presetManager.Save();
        runData.bInd = BattleData.SelectedEncounterIndex;
        runData.loot = loot.Save(ref runData.tPr, ref runData.tSh, ref runData.fArgs);
        if (isBattle)
        {
            runData.battle = EncounterManager.main.Save();
        }
    }

    public void LoadRunData(SaveManager.RunData data, SaveManager.Loader loader, SaveManager.State state)
    {
        bool isBattle = state == SaveManager.State.Battle;

        inventory.Load(data.inv, loader, isBattle);
        mapManager.Load(data.map);
        LoadBattleData(data.battle, loader);
        BattleData.SelectedEncounterIndex = data.bInd;
        inventory.FinishLoading(data.inv, loader, isBattle);
        presetManager.Load(data.presets, loader);
        loot.Load(data.loot, loader);
        shopManager.Load(data.shops, loader);

        CurrentId = data.currId;
    }

    private void LoadBattleData(SaveManager.BattleEncounterData battleData, SaveManager.Loader loader)
    {
        BattleData.LoadedUnits.Clear();
        if (battleData.data == null)
            return;
        foreach (var unitData in battleData.data)
        {
            if(loader.LoadUnit(unitData, transform, out var unit))
            {
                BattleData.LoadedUnits.Add(unit);
            }
        }
    }

    public class BattleInitializationData
    {
        public int SelectedEncounterIndex { get; set; }
        public List<Unit> LoadedUnits { get; } = new List<Unit>();
    }
}
