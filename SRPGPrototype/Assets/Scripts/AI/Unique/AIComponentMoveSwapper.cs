using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponentMoveSwapper : AIComponentBasic
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Color[] colors;
    [SerializeField] private Action[] actions;
    [SerializeField] private bool state;

    protected override Action MoveAction => state ? actions[1] : actions[0];

    public override void Initialize(AIUnit self)
    {
        base.Initialize(self);
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i] = actions[i].Validate(self.ActionTransform);
        }
        SetData();
        self.OnDamagedFn += OnDamaged;
        self.OnPhaseEndFn += OnPhaseEnd;
        self.OnPhaseStartFn += OnPhaseStart;
    }

    private void OnPhaseStart(BattleGrid grid, Unit unit)
    {
        foreach (var action in actions)
        {
            action.ResetUses(Action.Trigger.TurnStart);
        }
    }

    private void OnPhaseEnd(BattleGrid grid, Unit unit)
    {
        Swap();
    }

    private void OnDamaged(BattleGrid grid, Unit self, Unit source, int amount)
    {
        Swap();
    }

    private void Swap()
    {
        state = !state;
        SetData();
    }

    private void SetData()
    {
        int index = state ? 1 : 0;
        sprite.sprite = sprites[index];
        sprite.color = colors[index];
    }
}
