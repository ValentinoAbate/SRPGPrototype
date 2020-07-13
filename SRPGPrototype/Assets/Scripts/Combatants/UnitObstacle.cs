using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitObstacle : Combatant
{
    public override Team CTeam => Team.None;

    public override int MaxHp => maxHp;
    [SerializeField] private int maxHp = 3;

    public override int Hp { get; protected set; }

    public override int MaxAP => maxAP;
    [SerializeField] private int maxAP = 3;

    public override int AP { get => 0; protected set { } }

    public override string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;

    public override List<Action> Actions => throw new System.NotImplementedException();

    private void Awake()
    {
        ResetStats();
    }
}
