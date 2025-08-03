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
        actionInstance.UseAll(grid, self, self.Pos, false);
        Destroy(actionInstance);
    }
}
