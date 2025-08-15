using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProgramItemButton : MonoBehaviour
{
    [SerializeField] private ItemButton itemButton;
    public Program Program { get; private set; }
    
    public void Setup(Program p, UnityAction callback, Unit unit = null)
    {
        itemButton.SetupAsProgram(p, callback, unit);
        Program = p;
    }

    public void Show()
    {
        itemButton.Show();
    }

    public void Hide()
    {
        itemButton.Hide();
    }
}
