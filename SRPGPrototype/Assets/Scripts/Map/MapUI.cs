using Collections.Graphs;
using RandomUtils;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    public string encounterSceneName = "Encounter";
    [Header("Choice UI References")]
    public GameObject choiceUI;
    public Transform eventButtonContainer;
    public GameObject eventButtonPrefab;
    [Header("Preview UI References")]
    public GameObject previewUI;
    public BattleGrid previewGrid;
    public Transform previewObjContainer;
    public Button confirmEncounterButton;
    public Button backToEncounterSelectionButton;
    public Button nextEncounterButton;
    public Button prevEncounterButton;
    public GameObject spawnPointPrefab;
    public BattleCursor cursor;

    private readonly List<Encounter> encounterChoices = new List<Encounter>();
    private Encounter currentPreviewEncounter = null;

    // Start is called before the first frame update
    void Start()
    {
        backToEncounterSelectionButton.onClick.AddListener(HideEncounterPreview);
        backToEncounterSelectionButton.onClick.AddListener(ShowChoiceUI);
        HideEncounterPreview();
        HideChoiceUI();
        encounterChoices.Clear();
        encounterChoices.AddRange(PersistantData.main.mapManager.NextEncounters);
        InitializeChoiceButtons();
        ShowChoiceUI();
        UIManager.main.HideAllDescriptionUI();
    }

    private void Update()
    {
        if (currentPreviewEncounter == null)
            return;
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Tab))
        {
            NextEncounter(currentPreviewEncounter);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            NextEncounter(currentPreviewEncounter, true);
        }
    }

    private void InitializeChoiceButtons()
    {
        for(int index = 0; index < encounterChoices.Count; ++index)
        {
            var encounter = encounterChoices[index];
            var button = Instantiate(eventButtonPrefab, eventButtonContainer).GetComponent<Button>();
            var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = encounter.nameOverride;
            button.onClick.AddListener(HideChoiceUI);
            int displayIndex = index;
            button.onClick.AddListener(() => ShowEncounterPreview(encounter, displayIndex));
        }
    }

    private void ShowChoiceUI()
    {
        choiceUI.SetActive(true);
        UIManager.main.TopBarUI.SetTitleText("Choose an Encounter");
        UIManager.main.UnitDescriptionUI.Hide();
        cursor.NullAllActions();
    }

    private void HideChoiceUI()
    {
        choiceUI.SetActive(false);
    }

    private void ShowEncounterPreview(Encounter encounter, int index)
    {
        currentPreviewEncounter = encounter;
        var dimensions = encounter.dimensions;
        previewGrid.SetDimensions(dimensions.x, dimensions.y);
        previewGrid.CenterAtPosition(BattleGrid.DefaultCenter);
        previewUI.SetActive(true);
        confirmEncounterButton.onClick.RemoveAllListeners();
        confirmEncounterButton.onClick.AddListener(() => ConfirmEncounter(encounter));
        nextEncounterButton.onClick.RemoveAllListeners();
        nextEncounterButton.onClick.AddListener(() => NextEncounter(encounter));
        prevEncounterButton.onClick.RemoveAllListeners();
        prevEncounterButton.onClick.AddListener(() => NextEncounter(encounter, true));
        InitializePreviewObjects(encounter);
        UIManager.main.TopBarUI.SetTitleText($"{encounter.nameOverride ?? "Encounter"} ({index + 1}/{encounterChoices.Count})");

        cursor.OnHighlight = ShowUnitDescription;
        cursor.OnUnHighlight = UIManager.main.HideUnitDescriptionUI;
    }

    // Enable unit descriptions
    private void ShowUnitDescription(Vector2Int pos)
    {
        var unit = previewGrid.Get(pos);
        if (unit != null)
        {
            UIManager.main.UnitDescriptionUI.Show(unit);
        }
    }

    private void NextEncounter(Encounter current, bool backwards = false)
    {
        int index = encounterChoices.IndexOf(current);
        if (backwards)
        {
            if (--index < 0)
                index = encounterChoices.Count - 1;
        }
        else if (++index >= encounterChoices.Count)
        {
            index = 0;
        }

        previewObjContainer.DestroyAllChildren();
        ShowEncounterPreview(encounterChoices[index], index);
    }

    public void HideEncounterPreview()
    {
        previewObjContainer.DestroyAllChildren();
        previewUI.SetActive(false);
    }

    private void InitializePreviewObjects(Encounter e)
    {
        foreach (var entry in e.Units)
        {
            var unit = Instantiate(entry.unit, previewObjContainer).GetComponent<Unit>();
            previewGrid.Add(entry.pos, unit);
            unit.transform.position = previewGrid.GetSpace(unit.Pos);
        }
        foreach (var pos in e.spawnPositions)
        {
            var spawnPoint = Instantiate(spawnPointPrefab, previewObjContainer);
            spawnPoint.transform.position = previewGrid.GetSpace(pos);
        }
    }

    private void ConfirmEncounter(Encounter encounter)
    {
        PersistantData.main.mapManager.CurrentEncounter = encounter;
        SceneTransitionManager.main.TransitionToScene(encounterSceneName);
    }
}
