using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class Unit : GridObject, System.IComparable<Unit>
{
    public delegate void OnBattleStartDel(BattleGrid grid, Unit unit);

    public delegate void OnPhaseStartDel(BattleGrid grid, Unit unit);

    public delegate void OnSubAction(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions, SubAction.Options options, SubAction.Type overrideSubType = SubAction.Type.None);

    public delegate void OnAfterAction(BattleGrid grid, Action action, Unit user, int cost);

    public delegate void OnDeath(BattleGrid grid, Unit unit, Unit killedBy);

    public delegate void OnDamaged(BattleGrid grid, Unit self, Unit source, int amount);

    public enum Team
    { 
        None,
        Enemy,
        Player,
        Obstacle,
        Data,
        NPC,
    }

    public enum Jamming
    { 
        None,
        Low,
        Full
    }

    public enum Priority
    {
        First,
        Earliest,
        Earlier,
        Early,
        Normal,
        Late,
        Later,
        Latest,
        Last
    }

    [System.Flags]
    public enum Tags
    {
        None = 0,
        Explosive = 1,
        Placeholder = 2,
    }

    public virtual bool Movable => true;
    public abstract Team UnitTeam { get; }
    public abstract Jamming InterferenceLevel { get; }
    public abstract Priority PriorityLevel { get; }
    public abstract Tags UnitTags { get; }
    public abstract int MaxHP { get; set; }
    public abstract int HP { get; protected set; }
    public virtual bool Dead => HP <= 0;
    public abstract int MaxAP { get; set; }
    public abstract int AP { get; set; }
    public abstract int Repair { get; set; }
    public abstract int BaseRepair { get; }
    public abstract CenterStat Power { get; }
    public abstract CenterStat Speed { get; }
    public abstract CenterStat Break { get; }
    public abstract OnSubAction OnBeforeSubActionFn { get; }
    public abstract OnSubAction OnAfterSubActionFn { get; }
    public abstract OnAfterAction OnAfterActionFn { get; }
    public abstract OnDeath OnDeathFn { get; }
    public abstract OnDamaged OnDamagedFn { get; }
    public abstract OnBattleStartDel OnBattleStartFn { get; }
    public abstract OnPhaseStartDel OnPhaseStartFn { get; }
    public abstract string DisplayName { get; }
    public abstract string Description { get; }
    public abstract UnitUI UI { get; }

    public abstract Shell Shell { get; }

    public abstract IEnumerable<Action> Actions { get; }
    public abstract IReadOnlyList<ModifierActionDamage> IncomingDamageModifiers { get; }
    public virtual IReadOnlyCollection<Action> GetContextualActions(Unit user, BattleGrid grid)
    {
        return System.Array.Empty<Action>();
    }

    public void ModifyHp(BattleGrid grid, int amount, Unit source)
    {
        if (amount > 0)
            Heal(amount, source);
        else if (amount < 0)
            Damage(grid, -amount, source);
    }

    public virtual void Heal(int amount, Unit source)
    {
        if (Dead || amount <= 0)
            return;
        HP = Mathf.Min(HP + amount, MaxHP);
    }

    public virtual void Damage(BattleGrid grid, int amount, Unit source)
    {
        if (Dead)
            return;
        int damage = amount;
        if(!Break.IsZero)
        {
            damage += Break.Value;
            Break.Use();
        }
        if (damage <= 0)
            return;
        if (damage >= HP)
        {
            Kill(grid, source);
        }
        else
        {
            HP -= damage;
            OnDamagedFn?.Invoke(grid, this, source, damage);
        }
    }

    public virtual void DoRepair()
    {
        HP += Repair;
        Repair = 0;
    }

    public virtual void Kill(BattleGrid grid, Unit killedBy)
    {
        HP = 0;
        OnDeathFn?.Invoke(grid, this, killedBy);
        // On Death call may prevent death somehow
        if(Dead)
        {
            EncounterEventManager.main.OnUnitDeath?.Invoke(grid, this, killedBy);
            if(Pos != BattleGrid.OutOfBounds)
                grid.Remove(this);
            // Temporary: move to OOB
            transform.position = grid.GetSpace(BattleGrid.OutOfBounds);
        }
    }

    public virtual void ResetStats()
    {
        HP = MaxHP;
        AP = MaxAP;
        Power.Value = 0;
        Speed.Value = 0;
        Break.Value = 0;
    }

    public void ModifyStat(BattleGrid grid, Stats.StatName stat, int value, Unit source)
    {
        switch (stat)
        {
            case Stats.StatName.HP:
                ModifyHp(grid, value, source);
                break;
            case Stats.StatName.MaxHP:
                MaxHP += value;
                break;
            case Stats.StatName.AP:
                AP += value;
                break;
            case Stats.StatName.MaxAP:
                MaxAP += value;
                break;
            case Stats.StatName.Repair:
                Repair += value;
                break;
            case Stats.StatName.Power:
                Power.Value += value;
                break;
            case Stats.StatName.Speed:
                Speed.Value += value;
                break;
            case Stats.StatName.Break:
                Break.Value += value;
                break;
        }
    }

    public int GetStat(Stats.StatName stat)
    {
        switch (stat)
        {
            case Stats.StatName.HP:
                return HP;
            case Stats.StatName.MaxHP:
                return MaxHP;
            case Stats.StatName.AP:
                return AP;
            case Stats.StatName.MaxAP:
                return MaxAP;
            case Stats.StatName.Repair:
                return Repair;
            case Stats.StatName.Power:
                return Power.Value;
            case Stats.StatName.Speed:
                return Speed.Value;
            case Stats.StatName.Break:
                return Break.Value;
        }
        throw new System.Exception("Attempt to get invalid stat: " + stat.ToString());
    }

    public void SetStat(Stats.StatName stat, int value)
    {
        switch (stat)
        {
            case Stats.StatName.HP:
                HP = Mathf.Min(value, MaxHP);
                break;
            case Stats.StatName.MaxHP:
                MaxHP = value;
                break;
            case Stats.StatName.AP:
                AP = value;
                break;
            case Stats.StatName.MaxAP:
                MaxAP = value;
                break;
            case Stats.StatName.Repair:
                Repair = value;
                break;
            case Stats.StatName.Power:
                Power.Value = value;
                break;
            case Stats.StatName.Speed:
                Speed.Value = value;
                break;
            case Stats.StatName.Break:
                Break.Value = value;
                break;
        }
    }

    public bool CanUseAction(Action action)
    {
        return AP >= (action.APCost - Speed.Value);
    }

    public int ActionUsesUntilNoAP(Action action, int apToSave = 0)
    {
        int apBudget = AP - apToSave;
        if (apBudget <= 0)
            return 0;
        int cost = action.APCost - Speed.Value;
        if (cost > apBudget)
            return 0;
        int uses = 1;
        while(uses < 100)
        {
            cost += action.APCostAfterXUses(uses) - Speed.ValueAfterXUses(uses);
            if (cost > apBudget)
                break;
            ++uses;
        }
        return uses;
    }

    public virtual IEnumerator OnPhaseStart(BattleGrid grid)
    {
        yield break;
    }

    public virtual IEnumerator OnPhaseEnd()
    {
        AP = MaxAP;
        Power.Value = 0;
        Speed.Value = 0;
        Break.Value = 0;
        yield break;
    }

    public virtual IEnumerator OnBattleStart(EncounterManager manager)
    {
        OnBattleStartFn?.Invoke(manager.Grid, this);
        yield break;
    }

    public virtual IEnumerator OnBattleEnd(EncounterManager manager)
    {
        yield break;
    }

    public int CompareTo(Unit other)
    {
        if(UnitTeam != other.UnitTeam)
        {
            return UnitTeam.CompareTo(other.UnitTeam);
        }
        return PriorityLevel.CompareTo(other.PriorityLevel);
    }
}
