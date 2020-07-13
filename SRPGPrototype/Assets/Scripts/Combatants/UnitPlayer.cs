using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlayer : Combatant
{
    public override Team CTeam => Team.Player;

    public override int MaxHp => maxHp;
    private int maxHp = 3;

    public override int Hp { get; protected set; }

    public override int MaxAP => maxAP;
    private int maxAP = 3;

    public override int AP { get; protected set; }

    public override string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;

    public override List<Action> Actions => actions;
    [SerializeField] private List<Action> actions = new List<Action>();

    private void Awake()
    {
        ResetStats();
    }
}
