using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LootUI : MonoBehaviour
{
    public GameObject programDrawButtonPrefab;
    public GameObject shellDrawButtonPrefab;
    public GameObject moneyDrawButtonPrefab;
    public GameObject programButtonPrefab;
    public GameObject shellButtonPrefab;
    public GameObject moneyButtonPrefab;

    public Canvas uiCanvas;
    public GameObject menuUI;
    public GameObject programDrawUI;
    public GameObject shellDrawUI;
    public Transform menuButtonContainer;
    public Transform programDrawButtonContainer;
    public Transform shellDrawButtonContainer;
    public Button exitButton;

    [SerializeField] private bool allowLootSkipping = true;

    public void ShowUI(Inventory inv, LootData<Program> programDraws, LootData<Shell> shellDraws, IEnumerable<MoneyData> moneyData, UnityAction onLootClose)
    {
        // Setup ShellDraws
        SetupDraws(inv, shellDraws, shellDrawButtonPrefab, ShowShellDraw);
        // Setup Program Draws
        SetupDraws(inv, programDraws, programDrawButtonPrefab, ShowProgDraw);
        // Setup Money
        foreach(var data in moneyData)
        {
            SetupMoneyDrawButton(data, menuButtonContainer);
        }
        // Set up exit button
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(HideUI);
        exitButton.onClick.AddListener(onLootClose);
        // Activate menu
        ReturnToMainMenu();
        uiCanvas.gameObject.SetActive(true);
    }

    private void SetupDraws<T>(Inventory inv, LootData<T> lootData, GameObject buttonPrefab, System.Action<Inventory, Button, IEnumerable<T>, LootData<T>.Data> onClick) where T : MonoBehaviour, ILootable
    {
        // Setup Program Draws
        foreach (var draw in lootData.Draws)
        {
            var instantiatedDraw = new List<T>(draw.Count);
            foreach (var item in draw)
            {
                instantiatedDraw.Add(Instantiate(item.gameObject, transform).GetComponent<T>());
            }
            var button = Instantiate(buttonPrefab, menuButtonContainer).GetComponent<Button>();
            void OnClick()
            {
                onClick(inv, button, instantiatedDraw, draw);
                UIManager.main.TopBarUI.SetTitleText(string.IsNullOrEmpty(draw.Name) ? "Choose Loot" : draw.Name, false);
            }
            button.onClick.AddListener(OnClick);
            if (!string.IsNullOrEmpty(draw.Name))
            {
                var text = button.GetComponentInChildren<TextMeshProUGUI>();
                text.text = draw.Name;
            }
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

    public void ShowShellDraw(Inventory inv, Button menuButton, IEnumerable<Shell> shells, LootData<Shell>.Data data)
    {
        menuUI.SetActive(false);
        shellDrawButtonContainer.DestroyAllChildren();
        foreach (var shell in shells)
        {
            var lootButtonUI = Instantiate(shellButtonPrefab, shellDrawButtonContainer).GetComponent<LootButtonUI>();
            lootButtonUI.nameText.text = shell.DisplayName;
            void OnClick()
            {
                inv.AddShell(shell);
                FinishLootDraw(menuButton);
            }
            lootButtonUI.button.onClick.AddListener(OnClick);
            lootButtonUI.trigger.SetHoverCallbacks((_) => UIManager.main.ShellDescriptionUI.Show(shell), UIManager.main.HideShellDescriptionUI);
        }
        CreateDeclineButton(data.DeclineBonus, shellDrawButtonContainer, menuButton);
        shellDrawUI.SetActive(true);
    }

    public void ShowProgDraw(Inventory inv, Button menuButton, IEnumerable<Program> programs, LootData<Program>.Data data)
    {
        menuUI.SetActive(false);
        programDrawButtonContainer.DestroyAllChildren();
        foreach (var prog in programs)
        {
            var lootButtonUI = Instantiate(programButtonPrefab, programDrawButtonContainer).GetComponent<LootButtonUI>();
            lootButtonUI.icon.color = prog.ColorValue;
            lootButtonUI.nameText.text = prog.DisplayName;
            void OnClick()
            {
                inv.AddProgram(prog);
                FinishLootDraw(menuButton);
            }
            lootButtonUI.button.onClick.AddListener(OnClick);
            lootButtonUI.trigger.SetHoverCallbacks((_) => UIManager.main.ProgramDescriptionUI.Show(prog), UIManager.main.HideProgramDescriptionUI);
        }
        CreateDeclineButton(data.DeclineBonus, programDrawButtonContainer, menuButton);
        programDrawUI.SetActive(true);
    }

    private void CreateDeclineButton(int bonus, Transform container, Button drawButton)
    {
        if (bonus <= 0)
            return;
        var lootButtonUI = Instantiate(moneyButtonPrefab, container).GetComponent<LootButtonUI>();
        lootButtonUI.nameText.text = $"Skip (+${bonus})";
        void OnClick()
        {
            PersistantData.main.inventory.Money += bonus;
            FinishLootDraw(drawButton);
        }
        lootButtonUI.button.onClick.AddListener(OnClick);
    }

    private void SetupMoneyDrawButton(MoneyData data, Transform container)
    {
        if (data.Amount <= 0)
            return;
        var button = Instantiate(moneyDrawButtonPrefab, container).GetComponent<Button>();
        var text = button.GetComponentInChildren<TextMeshProUGUI>();
        text.text = data.Name;
        void OnClick()
        {
            PersistantData.main.inventory.Money += data.Amount;
            button.gameObject.SetActive(false);
        }
        button.onClick.AddListener(OnClick);
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

    public class MoneyData
    {
        public string Name { get; }
        public int Amount { get; }
        public MoneyData(int amount, string name = null)
        {
            Amount = amount;
            Name = string.IsNullOrEmpty(name) ? $"${amount}" : $"{name} (${amount})";
        }
    }
}
