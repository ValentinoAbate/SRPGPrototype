using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleUnit : Unit
{
    public override int MaxHP { get => maxHP; set { maxHP = value; HP = Mathf.Min(HP, value); } }
    [SerializeField] private int maxHP = 3;

    public override int HP { get => hp; protected set { hp = value; unitUI.Hp = value; } }
    private int hp = 0;

    public override int MaxAP { get => 0; set { } }

    public override int AP { get => 0; set { } }
    public override int Repair { get; set; }
    public override CenterStat Power { get; } = new CenterStat();
    public override CenterStat Speed { get; } = new CenterStat();
    public override CenterStat Defense { get; } = new CenterStat();

    public override OnAfterSubAction OnAfterSubActionFn { get; }
    public override OnDeath OnDeathFn { get; }

    public override string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;

    public override Team UnitTeam => Team.None;

    public override Shell Shell => throw new System.NotImplementedException();

    public override List<Action> Actions => throw new System.NotImplementedException();

    public UnitUI unitUI;

    // Start is called before the first frame update
    void Start()
    {
        ResetStats();
    }
}
