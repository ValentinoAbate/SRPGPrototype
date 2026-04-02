using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseActionOnDetonate : MonoBehaviour, IDetonatable
{
    [SerializeField] private Action action;

    public Action DetonateAction => action;

    public void Detonate(BattleGrid grid, Unit self, Unit detonator)
    {
        var actionInstance = Instantiate(action.gameObject, transform).GetComponent<Action>();
        var targetPos = self.Pos;
        void UseAction()
        {
            actionInstance.UseAll(grid, self, targetPos, false);
            Destroy(actionInstance);
        }
        EncounterEventManager.EnqueueDelayedEffect(UseAction);
    }
}
