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
        int upgradeCount = inv.AllPrograms.Count(HasReadyUpgrade);
        // Set button text
        showButtonText.text = "Upgrades (" + upgradeCount + ")";
    }

    private static bool HasReadyUpgrade(Program p)
    {
        foreach(var trigger in p.Upgrades)
        {
            if (trigger.Condition.Completed)
                return true;
        }
        return false;
    }

    public void EnterProgramSelectionUI(bool setTempTitle)
    {
        UIManager.main.TopBarUI.SetTitleText("Program Upgrades", setTempTitle);
        // Clear existing objects
        readyProgramContainer.DestroyAllChildren();
        notReadyProgramContainer.DestroyAllChildren();
        var inv = PersistantData.main.inventory;
        // Get all programs
        var programs = new List<Program>(inv.AllPrograms);
        // Setup program lists
        var readyPrograms = new List<System.Tuple<Program, int>>(programs.Count);
        var notReadyPrograms = new List<System.Tuple<Program, int>>(programs.Count);
        // Separate ready and non-ready programs
        foreach (var prog in programs)
        {
            if (prog.Upgrades.Count <= 0)
                continue;
            int completed = 0;
            foreach(var trigger in prog.Upgrades)
            {
                if (trigger.Condition.Completed)
                    ++completed;
            }
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
        readyPrograms.Sort(ProgramDisplayOrderComparer);
        InitializeProgramSelectButtons(readyPrograms, readyProgramContainer);
        notReadyPrograms.Sort(ProgramDisplayOrderComparer);
        InitializeProgramSelectButtons(notReadyPrograms, notReadyProgramContainer);
    }

    private static int ProgramDisplayOrderComparer(System.Tuple<Program, int> p1, System.Tuple<Program, int> p2) => p1.Item1.DisplayName.CompareTo(p2.Item1.DisplayName);

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
            hover.callback.AddListener((_) => UIManager.main.ProgramDescriptionUI.Show(prog));
            var hoverExit = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
            hoverExit.callback.AddListener(UIManager.main.HideProgramDescriptionUI);
            eventTrigger.triggers.Add(hover);
            eventTrigger.triggers.Add(hoverExit);
        }
    }

    public void ExitProgramSelectionUI()
    {
        UIManager.main.TopBarUI.EndTempTitleText();
        UpdateShowButtonText();
    }

    public void ShowProgramUpgradeUI(Program p)
    {
        UIManager.main.TopBarUI.SetTitleText($"{p.DisplayName} Upgrades", true);
        UIManager.main.ProgramDescriptionUI.Hide();
        var inv = PersistantData.main.inventory;
        upgradeButtonContainer.DestroyAllChildren();
        upgradeUI.SetActive(true);
        foreach(var trigger in p.Upgrades)
        {
            var upgradeButton = Instantiate(upgradeButtonPrefab, upgradeButtonContainer).GetComponent<Button>();
            var buttonText = upgradeButton.GetComponentInChildren<TextMeshProUGUI>();
            if(!trigger.Condition.Completed)
            {
                buttonText.text = trigger.TriggerName;
                upgradeButton.image.color = new Color(0.75f, 0.75f, 0.75f, 1);
            }
            else
            {
                buttonText.text = trigger.TriggerName + " (Ready)";
                upgradeButton.image.color = Color.white;
            }
            if(trigger is ProgramUpgrade)
            {
                var upgrade = trigger as ProgramUpgrade;
                if(upgrade.Condition.Completed)
                {
                    // TODO: Pull up confirmation window later 
                    upgradeButton.onClick.AddListener(() => p.Upgrade = upgrade);
                    upgradeButton.onClick.AddListener(ExitProgramUpgradeUI);
                    if (p.Shell != null && p.Shell.Compiled)
                    {
                        upgradeButton.onClick.AddListener(() => p.Shell.Compile());
                    }
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
        UIManager.main.TopBarUI.EndTempTitleText();
        UIManager.main.ProgramDescriptionUI.Hide();
        upgradeUI.SetActive(false);
        EnterProgramSelectionUI(false);
    }
}
