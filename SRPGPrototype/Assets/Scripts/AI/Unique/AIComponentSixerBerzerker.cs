using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponentSixerBerzerker : AIComponentBasic
{
    private AIUnit self;
    public override void Initialize(AIUnit self)
    {
        this.self = self;
        base.Initialize(self);
        if (EncounterEventManager.main != null)
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
            self.MaxAP++;
            self.AP++;
        }
    }
}
