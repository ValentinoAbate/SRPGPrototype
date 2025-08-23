using UnityEngine;
public class ProgramEffectGainStatOnBattleStart : ProgramEffectAddOnBattleStartAbility
{
    public Stats.StatName stat;
    public UnitNumber number;
    public override string AbilityName => abilityName;
    [SerializeField] private string abilityName = string.Empty;
    public override void Ability(BattleGrid grid, Unit unit)
    {
        unit.ModifyStat(grid, stat, number.Value(unit), unit);
    }
}
