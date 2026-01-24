using Collections.Graphs;
using RandomUtils;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    [Header("Preview UI References")]
    public GameObject previewUI;
    public BattleGrid previewGrid;
    public Transform previewObjContainer;
    public Button confirmEncounterButton;
    public Button nextEncounterButton;
    public Button prevEncounterButton;
    public GameObject spawnPointPrefab;
    public BattleCursor cursor;

    private readonly List<Encounter> encounterChoices = new List<Encounter>();
    private Encounter currentPreviewEncounter = null;

    // Start is called before the first frame update
    void Start()
    {
        encounterChoices.Clear();
        encounterChoices.AddRange(PersistantData.main.mapManager.NextEncounters);
        ShowEncounterPreview(encounterChoices[0], 0);
        UIManager.main.HideAllDescriptionUI();
        SaveManager.Save();
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
        UIManager.main.TurnOrderUI.Initialize(previewGrid);
        UIManager.main.UnitUIViewer.Initialize(previewGrid);
        UIManager.main.UnitUIViewer.Show();
        UIManager.main.UnitUIViewer.SetInteractable(true);
        UIManager.main.TurnOrderUI.Show();
        UIManager.main.TurnOrderUI.SetInteractable(true);

        cursor.OnHighlight = HighlightUnit;
        cursor.OnUnHighlight = UnHighlightUnit;
    }

    private void HighlightUnit(Vector2Int pos)
    {
        if (!previewGrid.TryGet(pos, out var unit))
        {
            return;
        }
        UIManager.main.UnitDescriptionUI.Show(unit);
        if (!unit.ShowUIByDefault)
        {
            unit.UI.SetVisible(true);
        }
    }

    private void UnHighlightUnit(Vector2Int pos)
    {
        UIManager.main.HideUnitDescriptionUI(pos);
        if (previewGrid.TryGet(pos, out var unit) && !unit.ShowUIByDefault)
        {
            unit.UI.SetVisible(false);
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
        UIManager.main.TopBarUI.SetTitleText(encounter.nameOverride ?? "Encounter");
        SceneTransitionManager.main.TransitionToScene(SceneTransitionManager.EncounterSceneName);
    }

    public void ReturnToCust()
    {
        UIManager.main.UnitUIViewer.Hide();
        UIManager.main.TurnOrderUI.Hide();
        SceneTransitionManager.main.TransitionToScene(SceneTransitionManager.CustSceneName);
    }
}
