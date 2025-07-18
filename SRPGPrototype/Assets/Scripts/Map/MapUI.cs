﻿using Collections.Graphs;
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

    private List<Vertex<Encounter>> vertices = null;

    // Start is called before the first frame update
    void Start()
    {
        var map = PersistantData.main.mapManager.Map;
        backToEncounterSelectionButton.onClick.AddListener(HideEncounterPreview);
        backToEncounterSelectionButton.onClick.AddListener(ShowChoiceUI);
        HideEncounterPreview();
        HideChoiceUI();
        vertices = new List<Vertex<Encounter>>(map.NextEncounters);
        InitializeChoiceButtons();
        ShowChoiceUI();
        UIManager.main.HideAllDescriptionUI();
    }

    private void InitializeChoiceButtons()
    {
        for(int index = 0; index < vertices.Count; ++index)
        {
            var vertex = vertices[index];
            var encounter = vertex.value;
            var button = Instantiate(eventButtonPrefab, eventButtonContainer).GetComponent<Button>();
            var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = encounter.nameOverride;
            button.onClick.AddListener(HideChoiceUI);
            int displayIndex = index;
            button.onClick.AddListener(() => ShowEncounterPreview(vertex, displayIndex));
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

    private void ShowEncounterPreview(Vertex<Encounter> vertex, int index)
    {
        var dimensions = vertex.value.dimensions;
        previewGrid.SetDimensions(dimensions.x, dimensions.y);
        previewGrid.CenterAtPosition(BattleGrid.DefaultCenter);
        previewUI.SetActive(true);
        confirmEncounterButton.onClick.RemoveAllListeners();
        confirmEncounterButton.onClick.AddListener(() => ConfirmEncounter(vertex));
        nextEncounterButton.onClick.RemoveAllListeners();
        nextEncounterButton.onClick.AddListener(() => NextEncounter(vertex));
        prevEncounterButton.onClick.RemoveAllListeners();
        prevEncounterButton.onClick.AddListener(() => NextEncounter(vertex, true));
        var encounter = vertex.value;
        InitializePreviewObjects(encounter);
        UIManager.main.TopBarUI.SetTitleText($"{encounter.nameOverride ?? "Encounter"} ({index + 1}/{vertices.Count})");

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

    private void NextEncounter(Vertex<Encounter> current, bool backwards = false)
    {
        int index = vertices.IndexOf(current);
        if (backwards)
        {
            if (--index < 0)
                index = vertices.Count - 1;
        }
        else if (++index >= vertices.Count)
        {
            index = 0;
        }

        previewObjContainer.DestroyAllChildren();
        ShowEncounterPreview(vertices[index], index);
    }

    public void HideEncounterPreview()
    {
        previewObjContainer.DestroyAllChildren();
        previewUI.SetActive(false);
    }

    private void InitializePreviewObjects(Encounter e)
    {
        foreach (var entry in e.units)
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

    private void ConfirmEncounter(Vertex<Encounter> vertex)
    {
        PersistantData.main.mapManager.Map.Current = vertex;
        SceneTransitionManager.main.TransitionToScene(encounterSceneName);
    }
}
