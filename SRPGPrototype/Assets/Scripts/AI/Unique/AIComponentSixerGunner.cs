using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponentSixerGunner : AIComponentBasic
{

    [SerializeField] private Action[] upgradedActions;

    protected override Action StandardAction
    {
        get
        {
            if (actionIndex < 0)
                return base.StandardAction;
            return upgradedActions[System.Math.Min(actionIndex, upgradedActions.Length - 1)];
        }
    }

    private int actionIndex = -1;

    public override void Initialize(AIUnit self)
    {
        base.Initialize(self);
        for(int i = 0; i < upgradedActions.Length; ++i)
        {
            upgradedActions[i] = upgradedActions[i].Validate(self.ActionTransform);
        }
        if(EncounterEventManager.main != null)
        {
            EncounterEventManager.main.OnUnitDeath -= OnUnitDeath;
            EncounterEventManager.main.OnUnitDeath += OnUnitDeath;
        }
    }

    private void OnUnitDeath(BattleGrid grid, Unit unit, Unit killedBy)
    {
        if(unit == this)
        {
            if (EncounterEventManager.main != null)
            {
                EncounterEventManager.main.OnUnitDeath -= OnUnitDeath;
            }
            return;
        }
        if (unit.DisplayName.Contains("Sixer"))
        {
            ++actionIndex;
        }
    }

    public override bool CanSave => true;

    public override string Save()
    {
        return actionIndex.ToString();
    }

    public override void Load(string data)
    {
        if(int.TryParse(data, out int savedInd))
        {
            actionIndex = savedInd;
        }
    }
}
