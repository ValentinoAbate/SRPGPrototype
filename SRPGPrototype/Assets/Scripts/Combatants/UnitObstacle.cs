using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitObstacle : Combatant
{
    public override int MaxHP => maxHP;
    [SerializeField] private int maxHP = 3;

    public override int HP { get => hp; protected set { hp = value; unitUI.Hp = value; } }
    private int hp = 0;

    public override int MaxAP => 0;

    public override int AP { get => 0; protected set { } }

    [SerializeField] private string displayName = string.Empty;
    public override string DisplayName => displayName;

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
