using UnityEngine;

[System.Serializable]
public class UnitNumber : DynamicNumber
{
    public enum Type
    {
        Constant,
        HP,
        MaxHP,
        AP,
        MaxAP,
        Repair,
    }

    [SerializeField]
    private int constant = 1;
    [SerializeField]
    private Type type = Type.Constant;

    public int Value(Unit unit)
    {
        if (type == Type.Constant)
            return constant;
        if (unit == null)
            return 0;
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
        else if(type == Type.Repair)
        {
            number = unit.Repair;
        }
        return Value(number);
    }
}
