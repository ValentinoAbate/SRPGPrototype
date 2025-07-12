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

    public void ShowShop(ShopID id, System.Action onComplete = null)
    {
        ui.Show(GetShopData(id), onComplete);
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
            programs.Add(Instantiate(program.gameObject, container).GetComponent<Program>());
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
