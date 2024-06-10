using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    public ProgramDescriptionUI programDesc;
    public ShellDescriptionUI shellDesc;

    [SerializeField] private bool allowLootSkipping = true;

    public void ShowUI(Inventory inv, LootData<Program> programDraws, LootData<Shell> shellDraws, System.Action onLootClose)
    {
        // Create loot objects
        var programDrawsInstantiated = programDraws.Select((list) => list.Select((prog) => Instantiate(prog.gameObject, transform).GetComponent<Program>()));
        var shellDrawsInstantiated = shellDraws.Select((list) => list.Select((shell) => Instantiate(shell.gameObject, transform).GetComponent<Shell>()));
        // Setup main menu
        foreach (var draw in programDrawsInstantiated)
        {
            var button = Instantiate(programDrawButtonPrefab, menuButtonContainer).GetComponent<Button>();
            button.onClick.AddListener(() => ShowProgDraw(inv, button, draw));
        }
        foreach (var draw in shellDrawsInstantiated)
        {
            var button = Instantiate(shellDrawButtonPrefab, menuButtonContainer).GetComponent<Button>();
            button.onClick.AddListener(() => ShowShellDraw(inv, button, draw));
        }
        // Set up exit button
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(HideUI);
        exitButton.onClick.AddListener(() => onLootClose());
        // Activate menu
        ReturnToMainMenu();
        uiCanvas.gameObject.SetActive(true);
    }

    public void HideUI()
    {
        menuUI.SetActive(false);
        programDrawUI.SetActive(false);
        shellDrawUI.SetActive(false);
        shellDesc.Hide();
        menuButtonContainer.DestroyAllChildren();
        programDrawButtonContainer.DestroyAllChildren();
        shellDrawButtonContainer.DestroyAllChildren();
        uiCanvas.gameObject.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        menuUI.SetActive(true);
        programDrawUI.SetActive(false);
        programDesc.Hide();
        shellDrawUI.SetActive(false);
        shellDesc.Hide();
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
            lootButtonUI.trigger.triggers.Clear();
            var hover = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
            hover.callback.AddListener((d) => shellDesc.Show(shell));
            var hoverExit = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
            hoverExit.callback.AddListener((d) => shellDesc.Hide());
            lootButtonUI.trigger.triggers.Add(hover);
            lootButtonUI.trigger.triggers.Add(hoverExit);
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
            lootButtonUI.trigger.triggers.Clear();
            var hover = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
            hover.callback.AddListener((d) => programDesc.Show(prog));
            var hoverExit = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
            hoverExit.callback.AddListener((d) => programDesc.Hide());
            lootButtonUI.trigger.triggers.Add(hover);
            lootButtonUI.trigger.triggers.Add(hoverExit);
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
