using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : Unit, IEncounterUnit
{
    public override Team UnitTeam => Team.Enemy;

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

    public override string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;

    public override Shell Shell => throw new NotImplementedException();

    public override List<Action> Actions => ai.Actions;

    public UnitUI unitUI;

    #region IEncounterUnit implementation

    public EncounterUnitData EncounterData => encounterData;
    [SerializeField] private EncounterUnitData encounterData = null;

    #endregion

    public override CenterStat Power { get; } = new CenterStat();
    public override CenterStat Speed { get; } = new CenterStat();
    public override CenterStat Defense { get; } = new CenterStat();



    private AIComponent<Unit> ai;

    private void Awake()
    {
        ai = GetComponent<AIComponent<Unit>>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetStats();
    }

    public IEnumerator DoTurn(BattleGrid grid)
    {
        yield return StartCoroutine(ai.DoTurn(grid, this));
    }

    public override IEnumerator OnPhaseStart()
    {
        foreach(var action in Actions)
        {
            action.TimesUsedThisTurn = 0;
        }
        yield break;
    }
}
