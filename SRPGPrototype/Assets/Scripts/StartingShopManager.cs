using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingShopManager : MonoBehaviour
{
    [SerializeField] private DropComponent<Shell>[] startingShellDrops;
    [SerializeField] private float[] startingShellDropWeights;
    [SerializeField] private DropComponent<Program>[] startingProgramDrops;


    // Start is called before the first frame update
    void Start()
    {
        GenerateAndShowLoot();
    }


    private void GenerateAndShowLoot()
    {
        var inv = PersistantData.main.inventory;
        var loot = PersistantData.main.loot;

        // Generate Shell Loot
        var startingShellDrop = RandomUtils.RandomU.instance.Choice(startingShellDrops, startingShellDropWeights);
        var shellDraws = new LootData<Shell>(1);
        shellDraws.Add(loot.ShellLoot, startingShellDrop.GenerateDrop);

        // Generate Program loot
        var progDraws = new LootData<Program>(startingProgramDrops.Length);
        foreach(var startProgramDrop in startingProgramDrops)
        {
            progDraws.Add(loot.ProgramLoot, startProgramDrop.GenerateDrop);
        }

        loot.UI.ShowUI(inv, progDraws, shellDraws, System.Array.Empty<LootUI.MoneyData>(), EndScene);
    }

    public void EndScene()
    {
        SceneTransitionManager.main.TransitionToScene("Cust");
    }
}
