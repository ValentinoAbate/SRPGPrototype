using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public event System.Action<Shell> OnEquippedShellChanged;
    public Shell EquippedShell 
    {
        get => equippedShell;
        set
        {
            if(value == null)
            {
                equippedShell = null;
            }
            else if (value.transform.IsChildOf(shellContainer.transform)) // from inv
            {
                equippedShell = value;
            }
            else // From asset
            {
                equippedShell = Instantiate(value.gameObject, shellContainer.transform).GetComponent<Shell>();
                //shells.Remove(value);
            }
            OnEquippedShellChanged?.Invoke(equippedShell);
        }
    }
    private Shell equippedShell = null;
    [SerializeField]
    private Transform shellContainer = null;
    [SerializeField]
    private Transform programContainer = null;

    public IEnumerable<Shell> DroneShells
    {
        get
        {
            foreach(var shell in Shells)
            {
                if (shell.Compiled && shell.HasSoulCore)
                    yield return shell;
            }
        }
    }
    public IReadOnlyList<Shell> Shells => shells;
    public IReadOnlyList<Program> Programs => programs;
    public IEnumerable<Program> AllPrograms
    {
        get
        {
            foreach (var program in Programs)
                yield return program;
            foreach (var shell in Shells)
            {
                foreach (var install in shell.Programs)
                {
                    yield return install.program;
                }
            }
        }
    }
    // Get all programs, including programs that could be removed from current shells
    public IEnumerable<Program> AllRemovablePrograms
    {
        get
        {
            foreach (var program in Programs)
                yield return program;
            var forbiddenAttributes = Program.Attributes.Fixed | Program.Attributes.SoulCore;
            foreach (var shell in Shells)
            {
                foreach(var install in shell.Programs)
                {
                    if (ProgramFilters.HasAnyAttributes(install.program, forbiddenAttributes))
                        continue;
                    var capacityMod = install.program.GetComponent<ProgramEffectModifyCapacity>();
                    if (capacityMod != null && capacityMod.amount > 0)
                        continue;
                    yield return install.program;
                }
            }
        }
    }

    private readonly List<Program> programs = new List<Program>();
    private readonly List<Shell> shells = new List<Shell>();

    public int Money 
    { 
        get => money;
        set
        {
            money = Mathf.Max(0, value);
            UIManager.main.TopBarUI.SetMoneyText(money);
        } 
    }
    private int money;

    public void Clear()
    {
        EquippedShell = null;
        shells.Clear();
        programs.Clear();
        shellContainer.DestroyAllChildren();
        programContainer.DestroyAllChildren();
        Money = 0;
    }

    public void AddProgram(Program p, bool fromAsset = false)
    {
        if(fromAsset)
        {
            programs.Add(p.InstantiateWithVariants(programContainer.transform));
        }
        else
        {
            p.transform.SetParent(programContainer.transform);
            programs.Add(p);
        }
    }

    public void RemoveProgram(Program p, bool destroy = false)
    {
        programs.Remove(p);
        if(destroy)
        {
            Destroy(p.gameObject);
        }
    }

    public bool HasProgram(Program p)
    {
        return programs.Contains(p);
    }

    public void AddShells(IEnumerable<Shell> shells, bool fromAsset = false)
    {
        if (!shells.Any())
            return;
        foreach(var shell in shells)
        {
            AddShellInternal(shell, fromAsset);
        }
        SortShells();
    }

    public void AddShell(Shell s, bool fromAsset = false)
    {
        AddShellInternal(s, fromAsset);
        SortShells();
    }

    private void AddShellInternal(Shell s, bool fromAsset)
    {
        Shell addedShell;
        if (fromAsset)
        {
            addedShell = Instantiate(s.gameObject, shellContainer.transform).GetComponent<Shell>();
        }
        else
        {
            addedShell = s;
            s.transform.SetParent(shellContainer.transform);
        }
        shells.Add(addedShell);
        if (EquippedShell == null)
        {
            EquippedShell = addedShell;
        }
    }

    private void SortShells()
    {
        shells.Sort(ShellComparer);
    }

    private int ShellComparer(Shell s1, Shell s2)
    {
        if (s1 == s2)
            return 0;
        if (s1 == EquippedShell)
            return -1;
        if (s1 == EquippedShell)
            return 1;
        return s1.HasSoulCore.CompareTo(s2.HasSoulCore);
    }

    public void RemoveShell(Shell s, bool destroy = false)
    {
        shells.Remove(s);
        if (destroy)
        {
            Destroy(s.gameObject);
        }
    }

    public bool CanAfford(int cost) => Money >= cost;

    public SaveManager.InventoryData Save(bool isBattle, ref List<SaveManager.ProgramData> fArgs)
    {
        var invData = new SaveManager.InventoryData()
        {
            money = Money,
            equipShId = EquippedShell != null ? EquippedShell.Id : -1,
            prs = new List<SaveManager.ProgramData>(Programs.Count),
            shs = new List<SaveManager.ShellData>(Shells.Count),
        };
        foreach (var program in Programs)
        {
            invData.prs.Add(program.Save(isBattle, ref fArgs));
        }
        foreach (var shell in Shells)
        {
            invData.shs.Add(shell.Save(isBattle, ref fArgs));
        }
        return invData;
    }

    public void Load(SaveManager.InventoryData data, SaveManager.Loader loader, bool isBattle)
    {
        Clear();
        Money = data.money;
        foreach (var programData in data.prs)
        {
            if (loader.LoadProgram(programData, isBattle, null, out var program))
            {
                AddProgram(program);
            }
        }
        foreach (var shellData in data.shs)
        {
            if (loader.CreateUnloadedShell(shellData, out var shell))
            {
                AddShellInternal(shell, false);
            }
        }
        if (loader.UnloadedShells.TryGetValue(data.equipShId, out var equippedShell))
        {
            EquippedShell = equippedShell.Shell;
        }
    }

    public void FinishLoading(SaveManager.InventoryData data, SaveManager.Loader loader, bool isBattle)
    {
        foreach (var shellData in data.shs)
        {
            loader.LoadUnloadedShell(shellData.id, isBattle, null);
        }
        SortShells();
    }
}
