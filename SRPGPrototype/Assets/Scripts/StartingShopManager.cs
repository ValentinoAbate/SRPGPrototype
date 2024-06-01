using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingShopManager : MonoBehaviour
{
    [SerializeField] LootManager loot;
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

        // Generate Shell Loot
        var startingShellDrop = RandomUtils.RandomU.instance.Choice(startingShellDrops, startingShellDropWeights);
        var shellDraws = new LootData<Shell>(startingShellDrop.GenerateDrop(loot.ShellLoot));

        // Generate Program loot
        var progDraws = new LootData<Program>(startingProgramDrops.Length);
        foreach(var startProgramDrop in startingProgramDrops)
        {
            progDraws.Add(startProgramDrop.GenerateDrop(loot.ProgramLoot));
        }

        loot.UI.ShowUI(inv, progDraws, shellDraws, EndScene);
    }

    public void EndScene()
    {
        SceneTransitionManager.main.TransitionToScene("Cust");
    }
}
