using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIUnit : Unit
{
    public override Team UnitTeam => team;
    [SerializeField] private Team team = Team.None;

    public override int MaxHP { get => maxHP; set { maxHP = value; HP = Mathf.Min(HP, value); } }
    [SerializeField] private int maxHP = 3;

    public override int HP { get => hp; protected set { hp = value; unitUI.Hp = value; } }
    private int hp = 0;

    public override int MaxAP { get => maxAP; set { maxAP = value; AP = Mathf.Min(AP, value); } }
    [SerializeField] private int maxAP = 3;

    public override int AP { get => ap; set { ap = value; unitUI.AP = value; } }
    private int ap = 0;

    public override int Repair { get; set; }

    public override OnAfterSubAction OnAfterSubActionFn { get; }
    public override OnDeath OnDeathFn { get => onDeathFn; }
    private OnDeath onDeathFn = null;
    public override OnBattleStartDel OnBattleStartFn { get; }

    public override string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;

    public override Shell Shell => throw new System.NotImplementedException();

    public override List<Action> Actions => AI == null ? new List<Action>() : AI.Actions;

    public UnitUI unitUI;

    public override CenterStat Power { get; } = new CenterStat();
    public override CenterStat Speed { get; } = new CenterStat();
    public override CenterStat Defense { get; } = new CenterStat();

    public abstract AIComponent<Unit> AI { get; }

    // Start is called before the first frame update
    void Start()
    {
        ResetStats();
        var onDeathEffects = GetComponents<ProgramEffectAddOnDeathAbility>();
        foreach (var effect in onDeathEffects)
        {
            onDeathFn += effect.Ability;
        }
    }

    public IEnumerator DoTurn(BattleGrid grid)
    {
        if (AI == null)
            yield break;
        yield return StartCoroutine(AI.DoTurn(grid, this));
    }

    public override IEnumerator OnPhaseStart()
    {
        foreach (var action in Actions)
        {
            action.TimesUsedThisTurn = 0;
        }
        yield break;
    }
}
