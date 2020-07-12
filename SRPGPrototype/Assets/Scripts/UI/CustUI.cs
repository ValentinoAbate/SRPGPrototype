using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustUI : MonoBehaviour
{
    public CustGrid grid;
    public Inventory inventory;

    public GameObject programButtonPrefab;

    public GameObject programButtonContainer;

    private ProgramButton pButton;
    private Program selectedProgram;

    private void Awake()
    {
        foreach(var program in inventory.programs)
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

    private void Update()
    {
        if (selectedProgram != null)
        {
            if(Input.GetMouseButtonDown(0))
            {
                var mousePos = grid.GetPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (grid.IsLegal(mousePos) && grid.Add(mousePos, selectedProgram))
                {
                    selectedProgram = null;
                    Destroy(pButton.gameObject);
                }

            }
            else if(Input.GetMouseButtonDown(1))
            {
                pButton.Cancel();
                selectedProgram = null;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            var mousePos = grid.GetPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if(grid.IsLegal(mousePos))
            {
                var prog = grid.Get(mousePos);
                if(prog != null && !prog.attributes.HasFlag(Program.Attributes.Fixed))
                {
                    var pButton = Instantiate(programButtonPrefab, programButtonContainer.transform);
                    var progButtonComponent = pButton.GetComponent<ProgramButton>();
                    progButtonComponent.Initialize(prog, this);
                    grid.Remove(prog);

                }
            }
        }
    }
}
