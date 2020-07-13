using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEnemy : Combatant
{
    public override Team CTeam => Team.Enemy;

    public override int MaxHp => maxHp;
    [SerializeField] private int maxHp = 3;

    public override int Hp { get; protected set; }

    public override int MaxAP => maxAP;
    [SerializeField] private int maxAP = 3;

    public override int AP { get; protected set; }

    public override string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;

    public override List<Action> Actions => throw new System.NotImplementedException();

    private void Awake()
    {
        ResetStats();
    }
}
