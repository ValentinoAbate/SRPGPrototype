using Collections.Graphs;
using RandomUtils;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    public string battleSceneName = "Battle";
    [Header("Choice UI References")]
    public GameObject choiceUI;
    public Transform eventButtonContainer;
    public GameObject eventButtonPrefab;
    public Button gambleButton;
    [Header("Preview UI References")]
    public GameObject previewUI;
    public BattleGrid previewGrid;
    public Transform previewObjContainer;
    public TextMeshProUGUI encounterNameText;
    public Button confirmEncounterButton;
    public Button backToEncounterSelectionButton;
    public Button nextEncounterButton;
    public Button prevEncounterButton;
    public GameObject spawnPointPrefab;

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
        InitializeChoiceButtons(vertices);
        gambleButton.onClick.AddListener(GambleEncounterChoice);
        ShowChoiceUI();
    }

    private void InitializeChoiceButtons(IEnumerable<Vertex<Encounter>> vertices)
    {
        foreach(var vertex in vertices)
        {
            var encounter = vertex.value;
            var button = Instantiate(eventButtonPrefab, eventButtonContainer).GetComponent<Button>();
            var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = encounter.name;
            button.onClick.AddListener(HideChoiceUI);
            button.onClick.AddListener(() => ShowEncounterPreview(vertex));
        }
    }

    private void GambleEncounterChoice()
    {
        ConfirmEncounter(RandomU.instance.Choice(vertices));
    }

    private void ShowChoiceUI()
    {
        choiceUI.SetActive(true);
    }

    private void HideChoiceUI()
    {
        choiceUI.SetActive(false);
    }

    private void ShowEncounterPreview(Vertex<Encounter> vertex)
    {
        previewUI.SetActive(true);
        confirmEncounterButton.onClick.RemoveAllListeners();
        confirmEncounterButton.onClick.AddListener(() => ConfirmEncounter(vertex));
        nextEncounterButton.onClick.RemoveAllListeners();
        nextEncounterButton.onClick.AddListener(() => NextEncounter(vertex));
        prevEncounterButton.onClick.RemoveAllListeners();
        prevEncounterButton.onClick.AddListener(() => NextEncounter(vertex, true));
        var encounter = vertex.value;
        InitializePreviewObjects(encounter);
        encounterNameText.text = encounter.name;
        // Add graph resizing when that's relvant
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
        ShowEncounterPreview(vertices[index]);
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
        SceneTransitionManager.main.TransitionToScene(battleSceneName);
    }
}
