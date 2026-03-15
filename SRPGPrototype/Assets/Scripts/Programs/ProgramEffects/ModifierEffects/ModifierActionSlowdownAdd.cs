using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierActionSlowdownAdd : ModifierAction
{
    public int Amount => amount;
    [SerializeField] private int amount;
}
