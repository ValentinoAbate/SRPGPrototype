using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenUI : MonoBehaviour
{
    [SerializeField] MapData mainMap;
    [SerializeField] MapData debugMap;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject debugButton;
    [SerializeField] private List<Program> debugPrograms;
    [SerializeField] private List<Shell> debugShells;
    [SerializeField] private int debugMoney;

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        continueButton.SetActive(SaveManager.HasRunData());
#if DEBUG
        debugButton.SetActive(true);
#endif
    }

    public void NewRun()
    {
        NewRunInternal(mainMap);
        SceneTransitionManager.main.TransitionToScene(SceneTransitionManager.StartingShopSceneName);
    }

    public void DebugRun()
    {
        NewRunInternal(debugMap);
        PersistantData.main.inventory.AddShells(debugShells, true);
        foreach(var program in debugPrograms)
        {
            PersistantData.main.inventory.AddProgram(program, true);
        }
        PersistantData.main.inventory.Money = debugMoney;
        SceneTransitionManager.main.TransitionToScene(SceneTransitionManager.CustSceneName);
    }

    private void NewRunInternal(MapData data)
    {
        var mapManager = PersistantData.main.mapManager;
        SaveManager.ClearRunData();
        mapManager.Setup(data);
    }

    public void ContinueRun()
    {
        SaveManager.Load();
        SceneTransitionManager.main.TransitionToScene(SceneTransitionManager.CustSceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
