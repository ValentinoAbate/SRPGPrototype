using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEnemy : Combatant
{
    public override Team UnitTeam => Team.Enemy;

    public override int MaxHP => maxHP;
    [SerializeField] private int maxHP = 3;

    public override int HP { get; protected set; }

    public override int MaxAP => maxAP;
    [SerializeField] private int maxAP = 3;

    public override int AP { get; protected set; }

    [SerializeField] private string displayName = string.Empty;
    public override string DisplayName => displayName;

    public override Shell Shell => throw new NotImplementedException();

    // Start is called before the first frame update
    void Start()
    {
        ResetStats();
    }
}
