using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    private static readonly ShopData EmptyShopData = new ShopData();

    public enum ShopID
    {
        None,
        Debug,
        Standard,
        ShellUpgrade,
    }

    [SerializeField] private ShopUI ui;
    [SerializeField] private List<Program> debugShopPrograms;
    [SerializeField] private List<Shell> debugShopShells;
    [SerializeField] private Transform objectContainer;
    [SerializeField] private ShopGenerator[] shopGenerators;

    private readonly ShopData debugShopData = new ShopData();
    private readonly Dictionary<ShopID, ShopData> data = new Dictionary<ShopID, ShopData>();

    public void Initialize()
    {
        Clear();
        foreach(var generator in shopGenerators)
        {
            if (data.ContainsKey(generator.ShopID))
                continue;
            data.Add(generator.ShopID, generator.GenerateShopData(objectContainer));
        }
#if DEBUG
        debugShopData.AddProgramFromAsset(debugShopPrograms, objectContainer);
        debugShopData.AddShellsFromAssets(debugShopShells, objectContainer);
#endif
    }

    public void Clear()
    {
        data.Clear();
        objectContainer.DestroyAllChildren();
    }

    public void ShowShop(ShopID id, Unit shopper = null, System.Action onComplete = null)
    {
        ui.Show(GetShopData(id), shopper, onComplete);
    }

    private ShopData GetShopData(ShopID id)
    {
        return id switch 
        { 
            ShopID.None => EmptyShopData,
            ShopID.Debug => debugShopData,
            _ => data.ContainsKey(id) ? data[id] : EmptyShopData,
        };
    }

    public List<SaveManager.ShopData> Save(ref List<SaveManager.ProgramData> fArgs)
    {
        var list = new List<SaveManager.ShopData>(data.Count);
        foreach(var kvp in data)
        {
            var shop = new SaveManager.ShopData() { id = kvp.Key };
            var programs = kvp.Value.Programs;
            shop.prs = new List<SaveManager.ProgramData>(programs.Count);
            foreach (var program in programs)
            {
                shop.prs.Add(program.Save(false, ref fArgs));
            }
            var shells = kvp.Value.Shells;
            shop.shs = new List<SaveManager.ShellData>(shells.Count);
            foreach (var shell in shells)
            {
                shop.shs.Add(shell.Save(false, ref fArgs));
            }
            list.Add(shop);
        }
        return list;
    }

    public void Load(List<SaveManager.ShopData> saveData, SaveManager.Loader loader)
    {
        Clear();
        foreach(var savedShop in saveData)
        {
            var shop = new ShopData();
            foreach(var programData in savedShop.prs)
            {
                if(loader.LoadProgram(programData, objectContainer, false, null, out var program))
                {
                    shop.AddProgram(program);
                }
            }
            foreach (var shellData in savedShop.shs)
            {
                if (loader.LoadShell(shellData, objectContainer, false, null, out var shell))
                {
                    shop.AddShell(shell);
                }
            }
            data.Add(savedShop.id, shop);
        }
    }

    public class ShopData
    {
        private const string defaultShopName = "Shop";
        public IReadOnlyList<Program> Programs => programs;
        private readonly List<Program> programs = new List<Program>();
        public IReadOnlyList<Shell> Shells => shells;
        private readonly List<Shell> shells = new List<Shell>();

        public string DisplayName { get; set; } = defaultShopName;

        public void AddProgramFromAsset(Program program, Transform container)
        {
            programs.Add(program.InstantiateWithVariants(container));
        }
        public void AddProgramFromAsset(IEnumerable<Program> programs, Transform container)
        {
            foreach (var program in programs)
            {
                AddProgramFromAsset(program, container);
            }
        }

        public void AddProgram(Program program)
        {
            programs.Add(program);
        }

        public void AddPrograms(IEnumerable<Program> programs)
        {
            foreach(var program in programs)
            {
                AddProgram(program);
            }
        }

        public void RemoveProgram(Program program)
        {
            programs.Remove(program);
        }

        public void AddShellFromAsset(Shell shell, Transform container)
        {
            shells.Add(Instantiate(shell.gameObject, container).GetComponent<Shell>());
        }

        public void AddShellsFromAssets(IEnumerable<Shell> shells, Transform container)
        {
            foreach (var shell in shells)
            {
                AddShellFromAsset(shell, container);
            }
        }

        public void AddShell(Shell shell)
        {
            shells.Add(shell);
        }

        public void AddShells(IEnumerable<Shell> shells)
        {
            foreach (var shell in shells)
            {
                AddShell(shell);
            }
        }

        public void RemoveShell(Shell shell)
        {
            shells.Remove(shell);
        }
    }
}
