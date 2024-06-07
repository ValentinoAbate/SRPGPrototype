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
        Repair,
    }

    [SerializeField]
    private int min = int.MinValue;
    [SerializeField]
    private int max = int.MaxValue;
    [SerializeField]
    private int constant = 1;
    [SerializeField]
    private int baseAmount = 0;
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
        int modNumber = baseAmount + ((number + modifier) * multiplier);
        return Mathf.Clamp(modNumber, min, max);
    }
}
