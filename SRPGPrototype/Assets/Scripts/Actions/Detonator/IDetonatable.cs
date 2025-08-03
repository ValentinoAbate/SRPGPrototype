using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDetonatable
{
    public Action DetonateAction { get; }
    public void Detonate(BattleGrid grid, Unit self, Unit detonator);
}
