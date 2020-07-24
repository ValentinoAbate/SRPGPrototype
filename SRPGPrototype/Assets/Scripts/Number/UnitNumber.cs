using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitNumber
{
    public enum Type
    {
        Constant,
        HP,
        MaxHP,
        AP,
        MaxAP,
    }

    [SerializeField]
    private int min = int.MinValue;
    [SerializeField]
    private int max = int.MaxValue;
    [SerializeField]
    private int constant = 1;
    [SerializeField]
    private int modifier = 0;
    [SerializeField]
    private int multiplier = 1;
    [SerializeField]
    private Type type = Type.Constant;

    public int Value(Unit unit)
    {
        if (type == Type.Constant)
            return constant;
        int number = 0;
        if(type == Type.AP)
        {
            number = unit.AP;
        }
        else if(type == Type.MaxAP)
        {
            number = unit.MaxAP;
        }
        else if(type == Type.HP)
        {
            number = unit.HP;
        }
        else if(type == Type.MaxHP)
        {
            number = unit.MaxHP;
        }
        int modNumber = (number + modifier) * multiplier;
        return Mathf.Clamp(modNumber, min, max);
    }
}
