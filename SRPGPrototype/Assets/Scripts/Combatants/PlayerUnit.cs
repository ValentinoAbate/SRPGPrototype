using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerUnit : Combatant
{
    public override Team UnitTeam => Team.Player;

    public override int MaxHP => Stats.MaxHp;

    public override int HP { get => Stats.Hp; protected set { Stats.Hp = value; unitUI.Hp = value; } }

    public override int MaxAP => Stats.MaxAP;

    public override int AP { get => ap; set { ap = value; unitUI.AP = value; } }
    private int ap = 0;

    [SerializeField] private string displayName = string.Empty;
    public override string DisplayName => displayName;

    public override Shell Shell => PersistantData.main.inventory.EquippedShell;

    private PlayerStats Stats => PersistantData.main.player.stats;

    public override List<Action> Actions => PersistantData.main.player.Actions.ToList();

    public UnitUI unitUI;

    // Start is called before the first frame update
    void Start()
    {
        AP = MaxAP;
        if (HP <= 0)
            HP = MaxHP;
        // Make sure UI gets updated
        HP = HP;
    }

    // Reset uses per turn
    public override IEnumerator OnPhaseStart()
    {
        foreach (var action in Actions)
            action.TimesUsedThisTurn = 0;
        yield break;
    }
}
