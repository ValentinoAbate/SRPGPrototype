using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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

    [SerializeField] private bool allowLootSkipping = true;

    public void ShowUI(Inventory inv, LootData<Program> programDraws, LootData<Shell> shellDraws, UnityAction onLootClose)
    {
        // Setup Program Draws
        SetupDraws(inv, programDraws, programDrawButtonPrefab, ShowProgDraw);
        // Setup ShellDraws
        SetupDraws(inv, shellDraws, shellDrawButtonPrefab, ShowShellDraw);
        // Set up exit button
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(HideUI);
        exitButton.onClick.AddListener(onLootClose);
        // Activate menu
        ReturnToMainMenu();
        uiCanvas.gameObject.SetActive(true);
    }

    private void SetupDraws<T>(Inventory inv, LootData<T> draws, GameObject buttonPrefab, System.Action<Inventory, Button, IEnumerable<T>> onClick) where T : MonoBehaviour, ILootable
    {
        // Setup Program Draws
        foreach (var draw in draws)
        {
            var instantiatedDraw = new List<T>(draw.Count);
            foreach (var item in draw)
            {
                instantiatedDraw.Add(Instantiate(item.gameObject, transform).GetComponent<T>());
            }
            var button = Instantiate(buttonPrefab, menuButtonContainer).GetComponent<Button>();
            void OnClick()
            {
                onClick(inv, button, instantiatedDraw);
            }
            button.onClick.AddListener(OnClick);
        }
    }

    public void HideUI()
    {
        menuUI.SetActive(false);
        programDrawUI.SetActive(false);
        shellDrawUI.SetActive(false);
        UIManager.main.HideAllDescriptionUI();
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
        UIManager.main.HideAllDescriptionUI();
        UIManager.main.TopBarUI.SetTitleText("Loot");
        RefreshExitButton();
    }

    public void FinishLootDraw(Button menuButton)
    {
        menuButton.gameObject.SetActive(false);
        ReturnToMainMenu();
    }

    public void ShowShellDraw(Inventory inv, Button menuButton, IEnumerable<Shell> data)
    {
        menuUI.SetActive(false);
        shellDrawButtonContainer.DestroyAllChildren();
        foreach (var shell in data)
        {
            var lootButtonUI = Instantiate(shellButtonPrefab, shellDrawButtonContainer).GetComponent<LootButtonUI>();
            lootButtonUI.nameText.text = shell.DisplayName;
            lootButtonUI.button.onClick.AddListener(() => inv.AddShell(shell));
            lootButtonUI.button.onClick.AddListener(() => FinishLootDraw(menuButton));
            lootButtonUI.trigger.SetHoverCallbacks((_) => UIManager.main.ShellDescriptionUI.Show(shell), UIManager.main.HideShellDescriptionUI);
        }
        shellDrawUI.SetActive(true);
    }

    public void ShowProgDraw(Inventory inv, Button menuButton, IEnumerable<Program> data)
    {
        menuUI.SetActive(false);
        programDrawButtonContainer.DestroyAllChildren();
        foreach (var prog in data)
        {
            var lootButtonUI = Instantiate(programButtonPrefab, programDrawButtonContainer).GetComponent<LootButtonUI>();
            lootButtonUI.icon.color = prog.ColorValue;
            lootButtonUI.nameText.text = prog.DisplayName;
            lootButtonUI.button.onClick.AddListener(() => inv.AddProgram(prog));
            lootButtonUI.button.onClick.AddListener(() => FinishLootDraw(menuButton));
            lootButtonUI.trigger.SetHoverCallbacks((_) => UIManager.main.ProgramDescriptionUI.Show(prog), UIManager.main.HideProgramDescriptionUI);
        }
        programDrawUI.SetActive(true);
    }

    private void RefreshExitButton()
    {
        if (allowLootSkipping)
        {
            exitButton.interactable = true;
            return;
        }
        foreach(Transform go in menuButtonContainer)
        {
            if (go.gameObject.activeSelf)
            {
                exitButton.interactable = false;
                return;
            }
        }
        exitButton.interactable = true;
    }
}
