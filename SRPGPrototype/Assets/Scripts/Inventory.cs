﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public event System.Action<Shell> OnEquippedShellChanged;
    public Shell EquippedShell 
    {
        get => equippedShell;
        set
        {
            if(value == null)
            {
                equippedShell = null;
            }
            else if (value.transform.IsChildOf(shellContainer.transform)) // from inv
            {
                equippedShell = value;
            }
            else // From asset
            {
                equippedShell = Instantiate(value.gameObject, shellContainer.transform).GetComponent<Shell>();
                //shells.Remove(value);
            }
            OnEquippedShellChanged?.Invoke(equippedShell);
        }
    }
    private Shell equippedShell = null;
    [SerializeField]
    private Transform shellContainer = null;
    [SerializeField]
    private Transform programContainer = null;

    [Header("Initial Values")]
    [SerializeField]
    private Shell[] startingShells = new Shell[1];
    [SerializeField]
    private Program[] startingPrograms = new Program[2];

    public Shell[] DroneShells => Shells.Where((s) => s.HasSoulCore && s.Compiled).ToArray();
    public IEnumerable<Shell> Shells => shells;
    public IEnumerable<Program> Programs => programs;

    private readonly List<Program> programs = new List<Program>();
    private readonly List<Shell> shells = new List<Shell>();

    public void Initialize()
    {
        foreach (var prog in startingPrograms)
            AddProgram(prog, true);
        foreach (var shell in startingShells)
            AddShell(shell, true);
        if (startingShells.Length >= 1)
            EquippedShell = shells[0];
    }

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
        Shell addedShell;
        if (fromAsset)
        {
            addedShell = Instantiate(s.gameObject, shellContainer.transform).GetComponent<Shell>();
        }
        else
        {
            addedShell = s;
            s.transform.SetParent(shellContainer.transform);
        }
        shells.Add(addedShell);
        if(EquippedShell == null)
        {
            EquippedShell = addedShell;
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

    public void Clear()
    {
        EquippedShell = null;
        shells.Clear();
        programs.Clear();
        shellContainer.DestroyAllChildren();
        programContainer.DestroyAllChildren();
    }
}
