using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlayer : Combatant
{
    public override Team UnitTeam => Team.Player;

    public override int MaxHP => maxHP;
    private int maxHP = 3;

    public override int HP { get; protected set; }

    public override int MaxAP => maxAP;
    private int maxAP = 3;

    public override int AP { get; protected set; }

    [SerializeField] private string displayName = string.Empty;
    public override string DisplayName => displayName;

    public override Shell Shell => shell;
    [SerializeField] private Shell shell = null;

    // Start is called before the first frame update
    void Start()
    {
        ResetStats();
    }
}
