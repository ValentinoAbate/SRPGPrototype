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

    public void PickupProgram(ProgramButton pButton, Program p)
    {
        if(pButton != null)
            pButton.Cancel();
        this.pButton = pButton;
        selectedProgram = p;

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("You clicked on: " + grid.GetPos(Camera.main.ScreenToWorldPoint(Input.mousePosition)).ToString());
        }
        if (selectedProgram != null)
        {
            if(Input.GetMouseButtonDown(0))
            {
                grid.Add(grid.GetPos(Camera.main.ScreenToWorldPoint(Input.mousePosition)), selectedProgram);
                selectedProgram = null;
            }
            else if(Input.GetMouseButtonDown(1))
            {
                pButton.Cancel();
                selectedProgram = null;
            }
        }
    }
}
