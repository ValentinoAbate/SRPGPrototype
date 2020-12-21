using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private Button showButton = null;
    [SerializeField] private TextMeshProUGUI showButtonText = null;
    [Header("Program Selection UI")]
    [SerializeField] private Transform readyProgramContainer = null;
    [SerializeField] private Transform notReadyProgramContainer = null;
    [SerializeField] private GameObject programSelectButtonPrefab = null;
    [SerializeField] private ProgramDescriptionUI programDescriptionUI = null;
    [Header("Upgrade UI")]
    [SerializeField] private GameObject upgradeUI = null;
    [SerializeField] private Transform upgradeButtonContainer = null;
    [SerializeField] private GameObject upgradeButtonPrefab = null;
    [SerializeField] private ProgramDescriptionUI previewOldProgramUI = null;
    [SerializeField] private ProgramDescriptionUI previewNewProgramUI = null;

    private void Start()
    {
        UpdateShowButtonText();
    }

    private void UpdateShowButtonText()
    {
        var inv = PersistantData.main.inventory;
        // Count programs in the inventory
        int upgradeCount = inv.Programs.Count((p) => p.Triggers.Any((t) => t.Condition.Completed));
        // Count programs in installed shells
        upgradeCount += inv.Shells.Sum((s) => s.Programs.Count((p) => p.program.Triggers.Any((t) => t.Condition.Completed)));
        // Set button text
        showButtonText.text = "Upgrades (" + upgradeCount + ")";
    }

    public void EnterProgramSelectionUI()
    {
        // Clear existing objects
        readyProgramContainer.DestroyAllChildren();
        notReadyProgramContainer.DestroyAllChildren();
        var inv = PersistantData.main.inventory;
        var readyPrograms = new List<System.Tuple<Program, int>>(inv.Programs.Count());
        var notReadyPrograms = new List<System.Tuple<Program, int>>(inv.Programs.Count());
        // Start with programs in inventories
        var programs = new List<Program>(inv.Programs);
        // Add programs installed in shells
        programs.AddRange(inv.Shells.SelectMany((s) => s.Programs.Select((p) => p.program)));
        // Separate ready and non-ready programs
        foreach (var prog in programs)
        {
            if (prog.Triggers.Length <= 0)
                continue;
            int completed = prog.Triggers.Count((t) => t.Condition.Completed);
            if(completed > 0)
            {
                readyPrograms.Add(new System.Tuple<Program, int>(prog, completed));
            }
            else
            {
                notReadyPrograms.Add(new System.Tuple<Program, int>(prog, completed));
            }
        }
        // Initialize program select buttons
        readyPrograms.Sort((p1, p2) => p1.Item1.DisplayName.CompareTo(p2.Item1.DisplayName));
        InitializeProgramSelectButtons(readyPrograms, readyProgramContainer);
        notReadyPrograms.Sort((p1, p2) => p1.Item1.DisplayName.CompareTo(p2.Item1.DisplayName));
        InitializeProgramSelectButtons(notReadyPrograms, notReadyProgramContainer);
    }

    private void InitializeProgramSelectButtons(List<System.Tuple<Program, int>> programs, Transform parent)
    {
        foreach (var progTuple in programs)
        {
            var prog = progTuple.Item1;
            var button = Instantiate(programSelectButtonPrefab, parent).GetComponent<Button>();
            var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            var eventTrigger = button.GetComponent<EventTrigger>();
            // Set up on click
            button.onClick.AddListener(() => ShowProgramUpgradeUI(prog));
            // Set name text
            int completed = progTuple.Item2;
            buttonText.text = completed > 0 ? prog.DisplayName + "(" + progTuple.Item2 + ")" : prog.DisplayName;
            // Set up event triggers
            eventTrigger.triggers.Clear();
            var hover = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
            hover.callback.AddListener((data) => programDescriptionUI.Show(prog));
            var hoverExit = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
            hoverExit.callback.AddListener((data) => programDescriptionUI.Hide());
            eventTrigger.triggers.Add(hover);
            eventTrigger.triggers.Add(hoverExit);
        }
    }

    public void ExitProgramSelectionUI()
    {
        UpdateShowButtonText();
    }

    public void ShowProgramUpgradeUI(Program p)
    {
        var inv = PersistantData.main.inventory;
        upgradeButtonContainer.DestroyAllChildren();
        upgradeUI.SetActive(true);
        foreach(var trigger in p.Triggers)
        {
            var upgradeButton = Instantiate(upgradeButtonPrefab, upgradeButtonContainer).GetComponent<Button>();
            var buttonText = upgradeButton.GetComponentInChildren<TextMeshProUGUI>();
            if(!trigger.Condition.Completed)
            {
                buttonText.text = trigger.TriggerName;
                upgradeButton.interactable = false;
                continue;
            }
            buttonText.text = trigger.TriggerName + " (Ready)";
            if(trigger is ProgramUpgrade)
            {
                var upgrade = trigger as ProgramUpgrade;
                // TODO: Pull up confirmation window later 
                upgradeButton.onClick.AddListener(() => p.Upgrade = upgrade);
                upgradeButton.onClick.AddListener(ExitProgramUpgradeUI);
                if(p.Shell != null && p.Shell.Compiled)
                {
                    upgradeButton.onClick.AddListener(() => p.Shell.Compile());
                }
                // Add preview
                var eventTrigger = upgradeButton.GetComponent<EventTrigger>();
                // Set up event triggers
                eventTrigger.triggers.Clear();
                var hover = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
                hover.callback.AddListener((data) => ShowUpgradePreview(p, upgrade));
                var hoverExit = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
                hoverExit.callback.AddListener((data) => HideUpgradePreview());
                eventTrigger.triggers.Add(hover);
                eventTrigger.triggers.Add(hoverExit);
            }
        }
    }

    private void ShowUpgradePreview(Program p, ProgramUpgrade upgrade)
    {
        previewOldProgramUI.Show(p);
        var oldUpgrade = p.Upgrade;
        p.Upgrade = upgrade;
        if(upgrade.Hidden)// && !upgrade.Condition.Completed)
        {
            previewNewProgramUI.ShowHidden(p);
        }
        else
        {
            previewNewProgramUI.Show(p);
        }
        p.Upgrade = oldUpgrade;
    }

    private void HideUpgradePreview()
    {
        previewOldProgramUI.Hide();
        previewNewProgramUI.Hide();
    }

    public void ExitProgramUpgradeUI()
    {
        upgradeUI.SetActive(false);
        EnterProgramSelectionUI();
    }
}
