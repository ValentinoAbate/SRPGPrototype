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
            // Already in inventory
            if (value.transform.IsChildOf(shellContainer.transform))
                equippedShell = value;
            else // From asset
            {
                equippedShell = Instantiate(value.gameObject, shellContainer.transform).GetComponent<Shell>();
                //shells.Remove(value);
            }
        }
    }
    [SerializeField]
    private Shell equippedShell = null;
    [SerializeField]
    private Transform shellContainer = null;
    [SerializeField]
    private List<Shell> shells = new List<Shell>();
    [SerializeField]
    private Transform programContainer = null;
    [SerializeField]
    private List<Program> programs = new List<Program>();

    public IEnumerable<Shell> Shells => shells;
    public IEnumerable<Program> Programs => programs;

    public void AddProgram(Program p, bool fromAsset = false)
    {
        if(fromAsset)
        {
            programs.Add(Instantiate(p.gameObject, programContainer.transform).GetComponent<Program>());
        }
        else
        {
            p.transform.SetParent(programContainer.transform);
            programs.Add(p);
        }
    }

    public void RemoveProgram(Program p, bool destroy = false)
    {
        programs.Remove(p);
        if(destroy)
        {
            Destroy(p.gameObject);
        }
    }

    public void AddShell(Shell s, bool fromAsset = false)
    {
        if (fromAsset)
        {
            shells.Add(Instantiate(s.gameObject, shellContainer.transform).GetComponent<Shell>());
        }
        else
        {
            s.transform.SetParent(shellContainer.transform);
            shells.Add(s);
        }
    }

    public void RemoveShell(Shell s, bool destroy = false)
    {
        shells.Remove(s);
        if (destroy)
        {
            Destroy(s.gameObject);
        }
    }
}
