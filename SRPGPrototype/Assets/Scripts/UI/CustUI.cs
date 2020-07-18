using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustUI : MonoBehaviour
{
    public bool Compiled
    {
        get => !compileButton.interactable;
        private set
        {
            compileButton.interactable = !value;
            exitCustButton.interactable = value;
        }
    }

    public CustGrid grid;
    [Header("Program Button UI")]
    public GameObject programButtonPrefab;
    public GameObject programButtonContainer;

    [Header("Program Description UI")]
    public GameObject programDescWindow;
    public TextMeshProUGUI programNameText;
    public TextMeshProUGUI programDescText;
    public TextMeshProUGUI programAttrText;

    [Header("Compile UI")]
    public Button compileButton;
    public Button exitCustButton;

    private Inventory inventory;
    private ProgramButton pButton;
    private Program selectedProgram;

    private void Start()
    {
        HideProgramDescriptionWindow(descEnabledFromGrid);
        inventory = PersistantData.main.inventory;
        foreach(var program in inventory.AllPrograms)
        {
            var pButton = Instantiate(programButtonPrefab, programButtonContainer.transform);
            var progButtonComponent = pButton.GetComponent<ProgramButton>();
            progButtonComponent.Initialize(program, this);
        }
    }

    public void PickupProgram(ProgramButton button, Program p)
    {
        if(pButton != null)
            pButton.Cancel();
        pButton = button;
        selectedProgram = p;

    }

    private Vector3 previousMousePos = Vector3.zero;
    private Vector2Int previousGridPos = new Vector2Int(-100, -100);
    private void Update()
    {
        if (selectedProgram != null)
        {
            if(Input.GetMouseButtonDown(0))
            {
                var mousePos = GetMouseGridPos();
                if (grid.IsLegal(mousePos) && grid.Add(mousePos, selectedProgram))
                {
                    inventory.RemoveProgram(selectedProgram);
                    grid.Shell.Install(selectedProgram, mousePos);
                    selectedProgram = null;
                    Destroy(pButton.gameObject);
                    Compiled = false;
                }

            }
            else if(Input.GetMouseButtonDown(1))
            {
                pButton.Cancel();
                selectedProgram = null;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(1))
            {
                var mousePos = GetMouseGridPos();
                if (grid.IsLegal(mousePos))
                {
                    var prog = grid.Get(mousePos);
                    if (prog != null && !prog.attributes.HasFlag(Program.Attributes.Fixed))
                    {
                        var pButton = Instantiate(programButtonPrefab, programButtonContainer.transform);
                        var progButtonComponent = pButton.GetComponent<ProgramButton>();
                        progButtonComponent.Initialize(prog, this);
                        grid.Shell.Uninstall(prog, prog.Pos);
                        grid.Remove(prog);
                        inventory.AddProgram(prog);
                        Compiled = false;
                    }
                }
            }
            else if(Input.mousePosition != previousMousePos)
            {
                previousMousePos = Input.mousePosition;
                var mousePos = GetMouseGridPos();
                if(mousePos != previousGridPos)
                {
                    previousGridPos = mousePos;
                    if (grid.IsLegal(mousePos) && !grid.IsEmpty(mousePos))
                    {
                        ShowProgramDescriptionWindow(grid.Get(mousePos), true);
                    }
                    else
                    {
                        HideProgramDescriptionWindow(true);

                    }
                }
            }
        }

    }

    private Vector2Int GetMouseGridPos()
    {
        return grid.GetPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private bool descEnabledFromGrid = false;
    public void ShowProgramDescriptionWindow(Program p, bool fromGrid)
    {
        descEnabledFromGrid = fromGrid;
        programNameText.text = p.DisplayName;
        programDescText.text = p.Description;
        programAttrText.text = GetAttributesText(p);
        programDescWindow.SetActive(true);
    }

    private string GetAttributesText(Program p)
    {
        var attTexts = new List<string>();
        if (p.attributes.HasFlag(Program.Attributes.Fixed))
        {
            attTexts.Add("Fixed");
        }
        if(p.attributes.HasFlag(Program.Attributes.Transient))
        {
            var transientAttr = p.GetComponent<ProgramAttributeTransient>();
            string errorText = "Error: No Attribute Component found";
            attTexts.Add("Transient " + (transientAttr == null ? errorText : transientAttr.UsesLeft.ToString()));
        }
        if (attTexts.Count <= 0)
            return string.Empty;
        if (attTexts.Count == 1)
            return attTexts[0];
        return attTexts.Aggregate((s1, s2) => s1 + ", " + s2);
    }

    public void HideProgramDescriptionWindow(bool fromGrid)
    {
        if (fromGrid != descEnabledFromGrid)
            return;
        programDescWindow.SetActive(false);
    }

    public void Compile()
    {
        Compiled = grid.Shell.Compile();
    }
}
