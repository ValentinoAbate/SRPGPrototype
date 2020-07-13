using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlayer : Combatant
{
    public override Team UnitTeam => Team.Player;

    public override int MaxHP => Stats.MaxHp;

    public override int HP { get => Stats.Hp; protected set => Stats.Hp = value; }

    public override int MaxAP => Stats.MaxAP;

    public override int AP { get; protected set; }

    [SerializeField] private string displayName = string.Empty;
    public override string DisplayName => displayName;

    public override Shell Shell => PersistantData.main.inventory.EquippedShell;

    private PlayerStats Stats => PersistantData.main.stats;

    public Transform actionContainer;

    // Start is called before the first frame update
    void Start()
    {
        AP = MaxAP;
    }
}
