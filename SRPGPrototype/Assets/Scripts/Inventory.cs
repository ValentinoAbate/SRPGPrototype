using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Shell EquippedShell 
    {
        get => equippedShell;
        set
        {
            if (value.transform.IsChildOf(transform))
                equippedShell = value;
            else
            {
                equippedShell = Instantiate(value.gameObject, transform).GetComponent<Shell>();
                shells.Remove(value);
            }
        }
    }
    [SerializeField]
    private Shell equippedShell = null;
    [SerializeField]
    private List<Shell> shells = new List<Shell>();
    [SerializeField]
    private List<Program> programs = new List<Program>();

    public IEnumerable<Program> AllPrograms => programs;

    public void AddProgram(Program p)
    {

    }

    public void RemoveProgram(Program p)
    {

    }

    public void AddShell(Shell s)
    {

    }

    public void RemoveShell(Shell s)
    {

    }
}
