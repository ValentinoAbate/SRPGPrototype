using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Step", menuName = "Validators/Arms Dealer")]
public class ArmsDealerValidator : UnitValidator
{
    [SerializeField] private int threshold = 2;
    public override bool IsValid(Unit item)
    {
        int numWeapons = 0;
        foreach(var program in PersistantData.main.inventory.AllPrograms)
        {
            if(ProgramFilters.GivesActionOfAnyType(program, Action.Type.Weapon, Action.Type.Hybrid) && ++numWeapons >= threshold)
            {
                return false;
            }
        }
        return true;
    }
}
