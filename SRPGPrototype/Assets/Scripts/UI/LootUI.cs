using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootUI : MonoBehaviour
{
    public GameObject programDrawButtonPrefab;
    public GameObject shellDrawButtonPrefab;
    public GameObject programButtonPrefab;
    public GameObject shellButtonPrefab;

    public Canvas uiCanvas;
    public GameObject menuUI;
    public GameObject programDrawUI;
    public GameObject shellDrawUI;
    public Transform menuButtonContainer;
    public Transform programDrawButtonContainer;
    public Transform shellDrawButtonContainer;
    public Button exitButton;

    private void Start()
    {
        HideUI();
    }

    public void ShowUI(Inventory inv, LootData<Program> programDraws, LootData<Shell> shellDraws, System.Action onLootClose)
    {
        // Setup main menu
        foreach(var draw in programDraws)
        {
            var button = Instantiate(programDrawButtonPrefab, menuButtonContainer).GetComponent<Button>();
            button.onClick.AddListener(() => ShowProgDraw(inv, button, draw));
        }
        foreach (var draw in shellDraws)
        {
            var button = Instantiate(shellDrawButtonPrefab, menuButtonContainer).GetComponent<Button>();
            button.onClick.AddListener(() => ShowShellDraw(inv, button, draw));
        }
        // Set up exit button
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(HideUI);
        exitButton.onClick.AddListener(() => onLootClose());
        // Activate menu
        menuUI.SetActive(true);
        uiCanvas.gameObject.SetActive(true);
    }

    public void HideUI()
    {
        menuUI.SetActive(false);
        programDrawUI.SetActive(false);
        shellDrawUI.SetActive(false);
        menuButtonContainer.DestroyAllChildren();
        programDrawButtonContainer.DestroyAllChildren();
        shellDrawButtonContainer.DestroyAllChildren();
        uiCanvas.gameObject.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        menuUI.SetActive(true);
        programDrawUI.SetActive(false);
        shellDrawUI.SetActive(false);
    }

    public void FinishLootDraw(Button menuButton)
    {
        Destroy(menuButton.gameObject);
        ReturnToMainMenu();
    }

    public void ShowShellDraw(Inventory inv, Button menuButton, List<Shell> data)
    {
        menuUI.SetActive(false);
        shellDrawButtonContainer.DestroyAllChildren();
        foreach (var shell in data)
        {
            var lootButtonUI = Instantiate(shellButtonPrefab, shellDrawButtonContainer).GetComponent<LootButtonUI>();
            lootButtonUI.nameText.text = shell.DisplayName;
            lootButtonUI.button.onClick.AddListener(() => inv.AddShell(shell, true));
            lootButtonUI.button.onClick.AddListener(() => FinishLootDraw(menuButton));
        }
        shellDrawUI.SetActive(true);
    }

    public void ShowProgDraw(Inventory inv, Button menuButton, List<Program> data)
    {
        menuUI.SetActive(false);
        programDrawButtonContainer.DestroyAllChildren();
        foreach (var prog in data)
        {
            var lootButtonUI = Instantiate(programButtonPrefab, programDrawButtonContainer).GetComponent<LootButtonUI>();
            lootButtonUI.nameText.text = prog.DisplayName;
            lootButtonUI.button.onClick.AddListener(() => inv.AddProgram(prog, true));
            lootButtonUI.button.onClick.AddListener(() => FinishLootDraw(menuButton));
        }
        programDrawUI.SetActive(true);
    }
}
