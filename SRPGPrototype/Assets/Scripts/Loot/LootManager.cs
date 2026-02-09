using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public Loot<Shell> ShellLoot { get; private set; }
    public Loot<Program> ProgramLoot { get; private set; }

    public List<Shell> shells = new List<Shell>();
    public List<Program> programs = new List<Program>();
    [SerializeField] private LootUI lootUI = null;
    [SerializeField] private Transform itemContainer;

    private LootData<Program> programDraws;
    private LootData<Shell> shellDraws;
    private ICollection<LootUI.MoneyData> moneyData;

    private void Awake()
    {
        ShellLoot = new Loot<Shell>(shells);
        ProgramLoot = new Loot<Program>(programs);
    }

    public void ShowUI(Inventory inv, LootData<Program> progLootData, LootData<Shell> shellLootData, ICollection<LootUI.MoneyData> money, System.Action onLootClose)
    {
        progLootData?.Instantiate(itemContainer);
        shellLootData?.Instantiate(itemContainer);
        programDraws = progLootData;
        shellDraws = shellLootData;
        moneyData = money;
        SaveManager.Save(SaveManager.State.Loot);
        void Close()
        {
            Clear();
            onLootClose?.Invoke();
        }
        lootUI.ShowUI(inv, progLootData, shellLootData, money, Close);
    }

    public void ShowSaved(Inventory inv, System.Action onLootClose)
    {
        void Close()
        {
            Clear();
            onLootClose?.Invoke();
        }
        lootUI.ShowUI(inv, programDraws, shellDraws, moneyData, Close);
    }

    private void Clear()
    {
        programDraws = null;
        shellDraws = null;
        moneyData = null;
        itemContainer.DestroyAllChildren();
    }

    public List<SaveManager.LootData> Save(ref List<SaveManager.ProgramData> tempPrograms, ref List<SaveManager.ShellData> tempShells, ref List<SaveManager.ProgramData> fArgs)
    {
        var ret = new List<SaveManager.LootData>((programDraws?.Draws.Count ?? 0) + (shellDraws?.Draws.Count ?? 0) + (moneyData?.Count ?? 0));
        if(programDraws != null)
        {
            foreach(var draw in programDraws.Draws)
            {
                var lootData = GetLootData(draw);
                foreach(var prog in draw)
                {
                    tempPrograms.Add(prog.Save(false, ref fArgs));
                    lootData.d.Add(prog.Id);
                }
                ret.Add(lootData);
            }
        }
        if(shellDraws != null)
        {
            foreach (var draw in shellDraws.Draws)
            {
                var lootData = GetLootData(draw);
                foreach (var shell in draw)
                {
                    tempShells.Add(shell.Save(false, ref fArgs));
                    lootData.d.Add(shell.Id);
                }
                ret.Add(lootData);
            }
        }
        if(moneyData != null)
        {
            foreach(var data in moneyData)
            {
                ret.Add(new SaveManager.LootData()
                {
                    n = data.Name,
                    m = data.Amount,
                });
            }
        }
        return ret;
    }

    private static SaveManager.LootData GetLootData<T>(LootData<T>.Data draw) where T : MonoBehaviour, ILootable
    {
        return new SaveManager.LootData()
        {
            n = draw.Name,
            m = draw.DeclineBonus,
            d = new List<int>(draw.Count)
        };
    }

    public void Load(List<SaveManager.LootData> data, SaveManager.Loader loader)
    {
        programDraws = new LootData<Program>();
        shellDraws = new LootData<Shell>();
        var moneyDraws = new List<LootUI.MoneyData>();
        moneyData = moneyDraws;
        if (data == null)
            return;
        foreach(var lootData in data)
        {
            if(lootData.d.Count <= 0)
            {
                moneyDraws.Add(new LootUI.MoneyData(lootData.m, lootData.n));
            }
            else if(loader.LoadedPrograms.TryGetValue(lootData.d[0], out var program))
            {
                var programs = new List<Program>(lootData.d.Count);
                programs.Add(program);
                program.transform.SetParent(itemContainer);
                for (int i = 1; i < lootData.d.Count; i++)
                {
                    if (loader.LoadedPrograms.TryGetValue(lootData.d[i], out program))
                    {
                        programs.Add(program);
                        program.transform.SetParent(itemContainer);
                    }
                }
                programDraws.Add(programs, lootData.n, lootData.m);
            }
            else if(loader.LoadedShells.TryGetValue(lootData.d[0], out var shell))
            {
                var shells = new List<Shell>(lootData.d.Count);
                shells.Add(shell);
                shell.transform.SetParent(itemContainer);
                for (int i = 1; i < lootData.d.Count; i++)
                {
                    if (loader.LoadedShells.TryGetValue(lootData.d[i], out shell))
                    {
                        shells.Add(shell);
                        shell.transform.SetParent(itemContainer);
                    }
                }
                shellDraws.Add(shells, lootData.n, lootData.m);
            }
        }
    }
}
