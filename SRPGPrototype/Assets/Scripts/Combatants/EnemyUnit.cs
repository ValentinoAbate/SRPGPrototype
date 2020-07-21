using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : Unit
{
    public override Team UnitTeam => Team.Enemy;

    public override int MaxHP => maxHP;
    [SerializeField] private int maxHP = 3;

    public override int HP { get => hp; protected set { hp = value; unitUI.Hp = value; } }
    private int hp = 0;

    public override int MaxAP => maxAP;
    [SerializeField] private int maxAP = 3;

    public override int AP { get => ap; set { ap = value; unitUI.AP = value; } }
    private int ap = 0;

    [SerializeField] private string displayName = string.Empty;
    public override string DisplayName => displayName;

    public override Shell Shell => throw new NotImplementedException();

    public override List<Action> Actions => ai.Actions;

    public UnitUI unitUI;

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
}
